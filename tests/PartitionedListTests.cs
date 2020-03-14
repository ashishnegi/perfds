using FsCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace PerfDS
{
    [TestClass]
    public class PartitionedListTests
    {
        [TestMethod]
        public void CountTestWith0InitialCapacity()
        {
            Prop.ForAll<int[]>(xs =>
            {
                var pList = new PartitionedList<int>(0, 1000);
                foreach (var x in xs)
                {
                    pList.Append(x);
                }

                return xs.Length == pList.Count();
            }).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void CountTestWithExactInitialCapacity()
        {
            Prop.ForAll<int[]>(xs =>
            {
                var pList = new PartitionedList<int>(xs.Length, 1000);
                foreach (var x in xs)
                {
                    pList.Append(x);
                }

                return xs.Length == pList.Count();
            }).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void InsertionAndRetrievalOrderMatch()
        {
            Prop.ForAll<int[]>(xs =>
            {
                var pList = new PartitionedList<int>(xs.Length, 1000);
                foreach (var x in xs)
                {
                    pList.Append(x);
                }

                int i = 0;
                foreach (var y in pList)
                {
                    Assert.AreEqual(xs[i], y);
                    i++;
                }
            }).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void NoOfBucketsInControl()
        {
            Prop.ForAll<int[], int>((xs, bucketSize) =>
            {
                if (bucketSize <= 0)
                {
                    bucketSize = 1;
                }

                var pList = new PartitionedList<int>(xs.Length, bucketSize);
                foreach (var x in xs)
                {
                    pList.Append(x);
                }

                int expectedBuckets = (xs.Length / bucketSize) + ((xs.Length % bucketSize) > 0 ? 1 : 0);
                return pList.NoOfBuckets() == expectedBuckets;
            }).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void NoOfBucketsInControl2()
        {
            var rand = new System.Random();
            Prop.ForAll<int[]>(xs =>
            {
                var pList = new PartitionedList<int>(0, 1000);
                foreach (var x in xs)
                {
                    pList.Append(x);
                }

                int expectedBuckets = (xs.Length / 1000) + ((xs.Length % 1000) > 0 ? 1 : 0);
                return pList.NoOfBuckets() == expectedBuckets;
            }).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void BucketSize()
        {
            var rand = new System.Random();
            Prop.ForAll<int[]>(xs =>
            {
                var pList = new PartitionedList<int>(0, 1000);
                foreach (var x in xs)
                {
                    pList.Append(x);
                }

                foreach (var bucket in pList.Buckets())
                {
                    if (1000 < bucket.Count())
                    {
                        Assert.Fail();
                    }
                }
            }).QuickCheckThrowOnFailure();
        }
    }
}
