using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;

namespace PerfDS
{
    /// Multiple Readers. Single Writer. Add only Dictionary.
    /// Readers get a snapshot view of the Dictionary.
    /// No background garbage collection of old values. Blocking cleanup of old values.
    /// typeparam name="TKey"
    /// typeparam name="TValue"
    public class SnapshotSingleWriterDictionary<TKey, TValue>
    {
        public SnapshotSingleWriterDictionary()
        {
            this.version = 0;
            this.pruneLock = new ReaderWriterLockSlim();
            this.data = new ConcurrentDictionary<TKey, SyncVersionList<ValueVersion<TValue>>>();
        }

        public void Add(TKey key, TValue value)
        {
            SyncVersionList<ValueVersion<TValue>> list;
            bool found = this.data.TryGetValue(key, out list);
            if (!found)
            {
                list = SyncVersionList<ValueVersion<TValue>>.Create();
            }

            list.Add(new ValueVersion<TValue>(this.version + 1, value));

            if (!found)
            {
                // TODO need to make SyncVersionList class for this to be atomic.
                this.data[key] = list;
            }

            // version is incremented after so that readers who start while `Add` has not finished
            // don't observe new value.
            // disallow reordering of next statement ? volatile `version`.
            this.version += 1;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (this.data.ContainsKey(key))
            {
                // an already added key can't be deleted. Not handling GC as of now.
                // values are added in desc order.
                value = this.data[key].Last().value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public void BlockingPruneOldValues()
        {
            // Write lock Waits for all snapshot views to finish.
            // TODO : optimize to prune only what can be done
            // with more info about ongoing snapshot views.

            this.pruneLock.EnterWriteLock();
            try
            {
                foreach (var kv in this.data)
                {
                    kv.Value.PruneOld();
                }
            }
            finally
            {
                this.pruneLock.ExitWriteLock();
            }
        }

        /// Should call Dispose on SnapshotView to release the snapshot.
        public SnapshotView TakeSnapshot()
        {
            this.pruneLock.EnterReadLock();
            return new SnapshotView(this);
        }

        internal void ReleaseSnapshot()
        {
            this.pruneLock.ExitReadLock();
        }

        internal int Test_OldValueCount()
        {
            int count = 0;

            foreach (var kv in this.data)
            {
                var vCount = kv.Value.Count();
                if (vCount > 1)
                {
                    count += vCount - 1;
                }
            }

            return count;
        }


        public class SnapshotView : IDisposable
        {
            internal SnapshotView(SnapshotSingleWriterDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
                this.myVersion = this.dictionary.GetReadVersion();
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                return this.dictionary.TryGetValue(key, out value, this.myVersion);
            }

            public void Dispose()
            {
                this.dictionary.ReleaseSnapshot();
            }

            long myVersion;
            private SnapshotSingleWriterDictionary<TKey, TValue> dictionary;
        }

        private bool TryGetValue(TKey key, out TValue value, long readVersion)
        {
            if (this.data.ContainsKey(key))
            {
                // an already added key can't be deleted. Not handling GC as of now.
                // values are added in desc order.
                foreach (var versionedValue in this.data[key])
                {
                    if (versionedValue.version <= readVersion)
                    {
                        value = versionedValue.value;
                        return true;
                    }
                }
            }

            value = default(TValue);
            return false;
        }

        private long GetReadVersion()
        {
            return this.version;
        }

        private long version;
        private ReaderWriterLockSlim pruneLock;
        private ConcurrentDictionary<TKey, SyncVersionList<ValueVersion<TValue>>> data;

        private struct ValueVersion<T>
        {
            public readonly long version;
            public readonly T value;

            public ValueVersion(long version, T value)
            {
                this.version = version;
                this.value = value;
            }
        }

        private struct SyncVersionList<T> : IEnumerable<T>
        {
            static public SyncVersionList<T> Create()
            {
                var ret = new SyncVersionList<T>();
                ret.Initliaze();
                return ret;
            }

            void Initliaze()
            {
                this.rwl = new ReaderWriterLockSlim();
                this.values = new List<T>();
            }

            internal void Add(T v)
            {
                this.rwl.EnterWriteLock();
                try
                {
                    values.Add(v);
                }
                finally
                {
                    this.rwl.ExitWriteLock();
                }
            }

            internal T Last()
            {
                this.rwl.EnterReadLock();
                try
                {
                    return this.values[this.values.Count - 1];
                }
                finally
                {
                    this.rwl.ExitReadLock();
                }
            }

            internal void PruneOld()
            {
                this.rwl.EnterWriteLock();
                try
                {
                    this.values.RemoveRange(0, values.Count - 1);
                }
                finally
                {
                    this.rwl.ExitWriteLock();
                }
            }


            public IEnumerator<T> GetEnumerator()
            {
                this.rwl.EnterReadLock();

                try
                {
                    for (int i = this.values.Count - 1; i >= 0; --i)
                    {
                        yield return this.values[i];
                    }
                }
                finally
                {
                    this.rwl.ExitReadLock();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            internal int Count()
            {
                this.rwl.EnterReadLock();

                try
                {
                    return this.values.Count;
                }
                finally
                {
                    this.rwl.ExitReadLock();
                }
            }

            private ReaderWriterLockSlim rwl;
            private List<T> values;
        }
    }
}