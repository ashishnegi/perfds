using FsCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace PerfDS
{
    [TestClass]
    public class SnapshotSingleWriterDictionaryTestsSingleThread
    {
        [TestMethod]
        public void SSWDLatestDataAvailableAfterWrite()
        {
            Prop.ForAll<Tuple<int, int>[]>(xs =>
            {
                var dict = new ReaderSnapshotSingleWriterDictionary<int, int>();

                foreach (var x in xs)
                {
                    dict.Add(x.Item1, x.Item2);
                    int actualValue = 0;
                    Assert.IsTrue(dict.TryGetValue(x.Item1, out actualValue));
                    Assert.AreEqual(x.Item2, actualValue);
                }
            }).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void SSWDLatestDataAvailableAfterWriteUniqueKeyValues()
        {
            Prop.ForAll<Tuple<int, int>[]>(xs =>
            {
                var dict = new ReaderSnapshotSingleWriterDictionary<int, int>();

                Array.Sort(xs);

                int lastKey = -1, lastValue = -1;
                foreach (var x in xs)
                {
                    if (lastKey == x.Item1 && lastValue == x.Item2)
                    {
                        continue;
                    }

                    lastKey = x.Item1;
                    lastValue = x.Item2;

                    // we should not already have this value.
                    int actualValue = 0;
                    if (dict.TryGetValue(x.Item1, out actualValue))
                    {
                        Assert.AreNotEqual(x.Item2, actualValue);
                    }

                    dict.Add(x.Item1, x.Item2);

                    // we should have this value.
                    Assert.IsTrue(dict.TryGetValue(x.Item1, out actualValue));
                    Assert.AreEqual(x.Item2, actualValue);
                }
            }).QuickCheckThrowOnFailure();
        }
    }

    // [TestClass]
    // public class SnapshotSingleWriterDictionaryTestsMultiThread
    // {
    //     [TestMethod]
    //     public void SSWDLatestDataAvailableAfterWrite()
    //     {
    //         Prop.ForAll<Tuple<int, int>[]>(xs =>
    //         {

    //             var dict = new ReaderSnapshotSingleWriterDictionary<int, int>();

    //             foreach (var x in xs)
    //             {
    //                 dict.Add(x.Item1, x.Item2);
    //                 int actualValue = 0;
    //                 Assert.IsTrue(dict.TryGetValue(x.Item1, out actualValue));
    //                 Assert.AreEqual(x.Item2, actualValue);
    //             }
    //         }).QuickCheckThrowOnFailure();
    //     }
    // }
}