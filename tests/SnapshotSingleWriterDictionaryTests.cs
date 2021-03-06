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

                using (var view = dict.TakeSnapshot())
                {
                    foreach (var kv in kvs)
                    {
                        dict.Add(kv.Item1, kv.Item2 + 1);
                        int actualValue;

                        Assert.IsTrue(view.TryGetValue(kv.Item1, out actualValue));
                        Assert.AreNotEqual(kv.Item2, actualValue);

                        Assert.IsTrue(dict.TryGetValue(kv.Item1, out actualValue));
                        Assert.AreNotEqual(kv.Item2 + 1, actualValue);
                    }
                }
            });
        }

        [TestMethod]
        public void SSWDPruneReduceOldValues()
        {
            Prop.ForAll<Tuple<int, int>[]>(kvs => {
                var dict = new SnapshotSingleWriterDictionary<int, int>();

                foreach (var kv in kvs)
                {
                    dict.Add(kv.Item1, kv.Item2);
                }

                Assert.AreEqual(0, dict.Test_OldValueCount());

                foreach (var kv in kvs)
                {
                    dict.Add(kv.Item1, kv.Item2 + 1);
                }

                Assert.AreEqual(kvs.Length, dict.Test_OldValueCount());

                dict.BlockingPruneOldValues();

                Assert.AreEqual(0, dict.Test_OldValueCount());
            });
        }

        [TestMethod]
        public void SSWDNoPruneDuringSnapshot()
        {
            var dict = new SnapshotSingleWriterDictionary<int, int>();

            for (int i = 0; i < 10; ++i)
            {
                dict.Add(i, i);
            }

            var view = dict.TakeSnapshot();

            var t = new Task(() => dict.BlockingPruneOldValues());
            t.Start();

            for (int i = 0; i < 10; ++i)
            {
                int actualValue = 0;
                Assert.IsTrue(view.TryGetValue(i, out actualValue));
                Assert.AreEqual(i, actualValue);
            }

            Assert.IsFalse(t.IsCompleted);

            view.Dispose();
            t.Wait();
        }
    }

    [TestClass]
    public class SnapshotSingleWriterDictionaryTestsMultiThread
    {
        public SnapshotSingleWriterDictionaryTestsMultiThread()
        {
            testConfiguration.MaxNbOfTest = 5000;
        }

        [TestMethod]
        public void SSWDMTReadersSeeLatestValuesSameKey()
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

                StartAndWaitAll(allTasks);
            }).Check(testConfiguration);
        }

        [TestMethod]
        public void SSWDMTReadersSeeLatestValuesMultipleKeysMultiWrites()
        {
            Prop.ForAll<Tuple<int, int>[]>(kvs =>
            {
                var dict = new SnapshotSingleWriterDictionary<int, int>();

                Array.Sort(kvs);

                int iterationsPerKey = 10;

                var writerTask = new Task(() => {
                    int lastKey = -1;
                    foreach (var kv in kvs)
                    {
                        if (kv.Item1 == lastKey)
                        {
                            // don't want to add lower value for this key after writing higher value.
                            continue;
                        }

                        lastKey = kv.Item1;

                        for (int i = 0; i < iterationsPerKey; ++i)
                        {
                            dict.Add(kv.Item1, kv.Item2 + i);

                            int actualValue = 0;
                            Assert.IsTrue(dict.TryGetValue(kv.Item1, out actualValue));
                            Assert.AreEqual(kv.Item2 + i, actualValue);
                        }
                    }
                });

                Action readerFn = () => {
                    int lastValueSeen = -1, lastKeySeen = -1;
                    if (kvs.Length > 0)
                    {
                        lastKeySeen = kvs[0].Item1;
                        lastValueSeen = kvs[0].Item2;
                    }

                    foreach (var kv in kvs)
                    {
                        int newValue = 0;

                        for (int i = 0; i < iterationsPerKey; ++i)
                        {
                            if (dict.TryGetValue(kv.Item1, out newValue))
                            {
                                // every reader should see value in increasing order.
                                if ((lastKeySeen == kv.Item1) && (lastValueSeen > newValue))
                                {
                                    Assert.Fail("Reader thread");
                                }

                                lastKeySeen = kv.Item1;
                                lastValueSeen = newValue;
                            }
                        }
                    }
                };

                int numTasks = 10;
                int firstReaders = 2;

                var allTasks = new List<Task>(numTasks);

                for (int i = 0; i < firstReaders; ++i)
                {
                    allTasks.Add(new Task(readerFn));
                }

                allTasks.Add(writerTask);

                for (int i = 0; i < numTasks - 1 - firstReaders; ++i)
                {
                    allTasks.Add(new Task(readerFn));
                }

                StartAndWaitAll(allTasks);
            }).Check(testConfiguration);
        }

        void StartAndWaitAll(List<Task> allTasks)
        {
            foreach (var t in allTasks)
            {
                t.Start();
            }

            Task.WaitAll(allTasks.ToArray());
        }

        Configuration testConfiguration = Configuration.QuickThrowOnFailure;
    }
}