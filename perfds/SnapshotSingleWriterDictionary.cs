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

        public SnapshotView GetSnapshot()
        {
            return new SnapshotView(this);
        }

        public class SnapshotView
        {
            public SnapshotView(SnapshotSingleWriterDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
                this.myVersion = this.dictionary.GetReadVersion();
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                return this.dictionary.TryGetValue(key, out value, this.myVersion);
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

            public void Add(T v)
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

            public T Last()
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

            private ReaderWriterLockSlim rwl;
            private List<T> values;
        }
    }
}