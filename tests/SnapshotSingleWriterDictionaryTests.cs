using FsCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                var dict = new SnapshotSingleWriterDictionary<int, int>();

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
                var dict = new SnapshotSingleWriterDictionary<int, int>();

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

        [TestMethod]
        public void SSWDReadSameKeyMultipleValues()
        {
            Prop.ForAll<int[]>(xs =>
            {
                var dict = new SnapshotSingleWriterDictionary<int, int>();

                Array.Sort(xs);

                int lastValue = -1;
                foreach (var x in xs)
                {
                    if (lastValue == x)
                    {
                        continue;
                    }

                    lastValue = x;

                    // we should not already have this value.
                    int actualValue = 0;
                    if (dict.TryGetValue(1, out actualValue))
                    {
                        Assert.AreNotEqual(x, actualValue);
                    }

                    dict.Add(1, x);

                    // we should have this value.
                    Assert.IsTrue(dict.TryGetValue(1, out actualValue));
                    Assert.AreEqual(x, actualValue);
                }
            }).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void SSWDSnapshotViewNotAffectedByNewWrites()
        {
            Prop.ForAll<Tuple<int, int>[]>(kvs => {
                var dict = new SnapshotSingleWriterDictionary<int, int>();

                foreach (var kv in kvs)
                {
                    dict.Add(kv.Item1, kv.Item2);
                }

                var view = dict.GetSnapshot();

                foreach (var kv in kvs)
                {
                    dict.Add(kv.Item1, kv.Item2 + 1);
                    int actualValue;

                    Assert.IsTrue(view.TryGetValue(kv.Item1, out actualValue));
                    Assert.AreNotEqual(kv.Item2, actualValue);

                    Assert.IsTrue(dict.TryGetValue(kv.Item1, out actualValue));
                    Assert.AreNotEqual(kv.Item2 + 1, actualValue);
                }
            });
        }
    }

    [TestClass]
    public class SnapshotSingleWriterDictionaryTestsMultiThread
    {
        [TestMethod]
        public void SSWDReadersSeeLatestValues()
        {
            Prop.ForAll<int[]>(xs =>
            {
                var dict = new SnapshotSingleWriterDictionary<int, int>();

                Array.Sort(xs);

                var writerTask = new Task(() => {
                    foreach (var x in xs)
                    {
                        dict.Add(1, x);
                        int actualValue = 0;
                        Assert.IsTrue(dict.TryGetValue(1, out actualValue));
                        Assert.AreEqual(x, actualValue);
                    }
                });

                Action readerFn = () => {
                    var lastValueSeen = -1;
                    if (xs.Length > 0)
                    {
                        lastValueSeen = xs[0];
                    }

                    foreach (var x in xs)
                    {
                        int actualValue = 0;
                        if (dict.TryGetValue(1, out actualValue))
                        {
                            // every reader should see value in increasing order.
                            if (lastValueSeen > actualValue)
                            {
                                Assert.Fail("Reader thread");
                            }

                            lastValueSeen = actualValue;
                        }
                    }
                };

                var allTasks = new List<Task>(10);
                for (int i = 0; i < 4; ++i)
                {
                    allTasks.Add(new Task(readerFn));
                }

                allTasks.Add(writerTask);

                for (int i = 0; i < 5; ++i)
                {
                    allTasks.Add(new Task(readerFn));
                }

                foreach (var t in allTasks)
                {
                    t.Start();
                }

                Task.WaitAll(allTasks.ToArray());
            }).QuickCheckThrowOnFailure();
        }
    }
}