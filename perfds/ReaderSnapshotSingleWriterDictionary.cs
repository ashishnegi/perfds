using System.Collections.Generic;
using System.Collections.Concurrent;

namespace PerfDS
{
    /// Multiple Readers. Single Writer. Add only Dictionary.
    /// Readers get a snapshot view of the Dictionary.
    /// No background garbage collection of old values. Blocking cleanup of old values.
    /// typeparam name="TKey"
    /// typeparam name="TValue"
    public class ReaderSnapshotSingleWriterDictionary<TKey, TValue>
    {
        public ReaderSnapshotSingleWriterDictionary()
        {
            this.version = 0;
            this.data = new ConcurrentDictionary<TKey, LinkedList<ValueVersion<TValue>>>();
        }

        public void Add(TKey key, TValue value)
        {
            // we know we have single writer.
            if (!this.data.ContainsKey(key))
            {
                this.data[key] = new LinkedList<ValueVersion<TValue>>();
            }

            this.data[key].AddFirst(new ValueVersion<TValue>(this.version + 1, value));
            // version is incremented after so that readers who start while `Add` has not finished
            // don't observe new value.
            // disallow reordering of next statement ? volatile `version`.
            this.version += 1;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            long myVersion = this.version;

            if (this.data.ContainsKey(key))
            {
                // an already added key can't be deleted. Not handling GC as of now.
                // values are added in desc order.
                foreach (var versionedValue in this.data[key])
                {
                    if (versionedValue.version <= myVersion)
                    {
                        value = versionedValue.value;
                        return true;
                    }
                }
            }

            value = default(TValue);
            return false;
        }

        long version;
        private ConcurrentDictionary<TKey, LinkedList<ValueVersion<TValue>>> data;

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
    }
}