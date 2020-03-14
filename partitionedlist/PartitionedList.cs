using System.Collections;
using System.Collections.Generic;

namespace partitionedlist
{
    public class PartitionedList<T> : IEnumerable<T>
    {
        public PartitionedList(int capacity, int bucketSize)
        {
            this.bucketSize = bucketSize;
            int extraBucket = ((capacity % bucketSize) == 0) ? 0 : 1;
            this.buckets = new List<T[]>((capacity / bucketSize) + extraBucket);

            for (int i = this.buckets.Count; i > 0; --i)
            {
                this.buckets.Add(new T[this.bucketSize]);
            }

            this.currentBucketIndex = 0;
            this.noOfItemsInCurrBucket = 0;

            if (this.buckets.Count > 0)
            {
                this.currentBucket = this.buckets[0];
            }
        }

        public void Append(T a)
        {
            EnsureCapacity();
            this.currentBucket[this.noOfItemsInCurrBucket] = a;
            this.noOfItemsInCurrBucket += 1;
        }

        public int Count()
        {
            return (this.bucketSize * currentBucketIndex) + this.noOfItemsInCurrBucket;
        }

        internal int NoOfBuckets()
        {
            return this.buckets.Count;
        }

        internal IEnumerable<IEnumerable<T>> Buckets()
        {
            return this.buckets;
        }

        void EnsureCapacity()
        {
            if (this.noOfItemsInCurrBucket == this.bucketSize)
            {
                this.noOfItemsInCurrBucket = 0;
                this.currentBucketIndex += 1;
            }
            
            if (this.currentBucketIndex >= this.buckets.Count)
            {
                this.currentBucket = new T[this.bucketSize];
                this.buckets.Add(this.currentBucket);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.currentBucketIndex; ++i)
            {
                var bucket = this.buckets[i];
                for (int j = 0; j < this.bucketSize; ++j)
                {
                    yield return bucket[j];
                }
            }

            for (int j = 0; j < this.noOfItemsInCurrBucket; ++j)
            {
                yield return this.currentBucket[j];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly int bucketSize;
        private int currentBucketIndex;
        private int noOfItemsInCurrBucket;
        T[] currentBucket;
        private readonly List<T[]> buckets;
    }
}
