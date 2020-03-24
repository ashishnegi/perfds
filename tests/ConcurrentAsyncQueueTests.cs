using FsCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PerfDS
{
    [TestClass]
    public class ConcurrentAsyncQueueTests
    {
        public ConcurrentAsyncQueueTests()
        {
            testConfig.MaxNbOfTest = 100;
            testConfig.EndSize = 1000;
        }

        [TestMethod]
        public void ConcurrentEnqueAndDequeueHappensInOrder()
        {            
            Prop.ForAll<int[]>(xs =>
            {
                var queue = new ConcurrentAsyncQueue<int>();
                Task writerTask = new Task(() =>
                {
                    foreach (var x in xs)
                    {
                        queue.Enqueue(x);
                    }
                });

                bool allSuccess = true;
                Task readerTask = new Task(async () =>
                {
                    foreach (var expected in xs)
                    {
                        var actual = await queue.DequeueAsync();
                        Assert.AreEqual(expected, actual);
                        allSuccess = allSuccess && (expected == actual);
                    }
                });

                readerTask.Start();
                writerTask.Start();

                Task.WaitAll(writerTask, readerTask);

                return allSuccess;
            }).Check(testConfig);
        }

        [TestMethod]
        public void ConcurrentEnqueAndDequeueMultiReaders()
        {
            Prop.ForAll<int[]>(expectedList =>
            {
                Array.Sort(expectedList);

                var queue = new ConcurrentAsyncQueue<int>();
                Task writerTask = new Task(() =>
                {
                    foreach (var x in expectedList)
                    {
                        queue.Enqueue(x);
                    }
                });

                var cancelSource = new CancellationTokenSource();
                CancellationToken cancelToken = cancelSource.Token;

                long remainingToRead = expectedList.Length;

                var stopTask = new Task(async () =>
                {
                    while (Interlocked.Read(ref remainingToRead) > 0)
                    {
                        await Task.Delay(10);
                    }

                    cancelSource.Cancel();
                });

                int startValue = 0;
                if (expectedList.Length > 0)
                {
                    startValue = expectedList[0];
                }

                var allTasks = new List<Task>();

                // can't move to separate fn because of `ref`
                var reader1 = ReaderTask(startValue, cancelToken, queue, (a, b) =>
                {
                    Interlocked.Decrement(ref remainingToRead);
                    Assert.IsTrue(a >= b);
                });

                allTasks.Add(reader1);

                var reader2 = ReaderTask(startValue, cancelToken, queue, (a, b) =>
                {
                    Interlocked.Decrement(ref remainingToRead);
                    Assert.IsTrue(a >= b);
                });

                allTasks.Add(reader2);

                allTasks.Add(writerTask);
                writerTask.Start();

                var reader3 = ReaderTask(startValue, cancelToken, queue, (a, b) =>
                {
                    Interlocked.Decrement(ref remainingToRead);
                    Assert.IsTrue(a >= b);
                });

                allTasks.Add(reader3);

                var reader4 = ReaderTask(startValue, cancelToken, queue, (a, b) =>
                {
                    Interlocked.Decrement(ref remainingToRead);
                    Assert.IsTrue(a >= b);
                });

                allTasks.Add(reader4);

                allTasks.Add(stopTask);
                stopTask.Start();

                Task.WaitAll(allTasks.ToArray());

                var actualList = MergeSortList(new List<int>[] { reader1.Result, reader2.Result, reader3.Result, reader4.Result });
                AssertIfNotEqual(expectedList, actualList);
            }).Check(testConfig);
        }

        [TestMethod]
        public void ConcurrentEnqueAndDequeueMultiWriters()
        {
            Prop.ForAll<int[]>(expectedList =>
            {
                Array.Sort(expectedList);

                var queue = new ConcurrentAsyncQueue<int>();

                int numWriters = 4;

                var cancelSource = new CancellationTokenSource();
                CancellationToken cancelToken = cancelSource.Token;

                long remainingToRead = expectedList.Length;

                int startValue = 0;
                if (expectedList.Length > 0)
                {
                    startValue = expectedList[0];
                }

                var allTasks = new List<Task>();

                // can't move to separate fn because of `ref`
                var reader1 = ReaderTask(startValue, cancelToken, queue, (a, b) =>
                {
                    Interlocked.Decrement(ref remainingToRead);
                });

                allTasks.Add(reader1);

                for (int i = 0; i < numWriters; ++i)
                {
                    int myIndex = i;
                    allTasks.Add(Task.Run(() =>
                    {
                        for (int j = myIndex; j < expectedList.Length; j += numWriters)
                        {
                            queue.Enqueue(expectedList[j]);
                        }
                    }));
                }

                allTasks.Add(Task.Run(async () =>
                {
                    while (Interlocked.Read(ref remainingToRead) > 0)
                    {
                        await Task.Delay(10);
                    }

                    cancelSource.Cancel();
                }));
                
                Task.WaitAll(allTasks.ToArray());

                var actualList = reader1.Result;
                AssertIfNotEqual(expectedList, actualList);
            }).Check(testConfig);
        }

        [TestMethod]
        public void ConcurrentEnqueAndDequeueMultiReadersWriters()
        {
            Prop.ForAll<int[]>(expectedList =>
            {
                Array.Sort(expectedList);

                var queue = new ConcurrentAsyncQueue<int>();

                int numWriters = 4;

                var cancelSource = new CancellationTokenSource();
                CancellationToken cancelToken = cancelSource.Token;

                long remainingToRead = expectedList.Length;

                int startValue = 0;
                if (expectedList.Length > 0)
                {
                    startValue = expectedList[0];
                }

                var allTasks = new List<Task>();

                var readers = new List<Task<List<int>>>();

                // can't move to separate fn because of `ref`
                readers.Add(ReaderTask(startValue, cancelToken, queue, (a, b) =>
                {
                    Interlocked.Decrement(ref remainingToRead);
                }));

                readers.Add(ReaderTask(startValue, cancelToken, queue, (a, b) =>
                {
                    Interlocked.Decrement(ref remainingToRead);
                }));

                for (int i = 0; i < numWriters; ++i)
                {
                    int myIndex = i;
                    allTasks.Add(Task.Run(() =>
                    {
                        for (int j = myIndex; j < expectedList.Length; j += numWriters)
                        {
                            queue.Enqueue(expectedList[j]);
                        }
                    }));
                }

                readers.Add(ReaderTask(startValue, cancelToken, queue, (a, b) =>
                {
                    Interlocked.Decrement(ref remainingToRead);
                }));

                readers.Add(ReaderTask(startValue, cancelToken, queue, (a, b) =>
                {
                    Interlocked.Decrement(ref remainingToRead);
                }));

                allTasks.Add(Task.Run(async () =>
                {
                    while (Interlocked.Read(ref remainingToRead) > 0)
                    {
                        await Task.Delay(10);
                    }

                    cancelSource.Cancel();
                }));

                allTasks.AddRange(readers);
                Task.WaitAll(allTasks.ToArray());

                var actualList = MergeSortList(readers.Select(r => r.Result).ToArray());
                AssertIfNotEqual(expectedList, actualList);
            }).Check(testConfig);
        }

        static private void AssertIfNotEqual(int[] expectedList, List<int> actualList)
        {
            Assert.AreEqual(expectedList.Length, actualList.Count);

            actualList.Sort();
            int index = 0;
            foreach (var actual in actualList)
            {
                Assert.AreEqual(expectedList[index], actual);
                index += 1;
            }
        }

        static List<T> MergeSortList<T>(List<T>[] lists)
        {
            var list = lists.SelectMany(i => i).ToList();
            list.Sort();
            return list;
        }

        static async Task<List<T>> ReaderTask<T>(T startValueRead, CancellationToken cancelToken, ConcurrentAsyncQueue<T> queue, Action<T, T> afterReadValue)
        {
           T lastValueRead = startValueRead;

            var list = new List<T>();
            while (!cancelToken.IsCancellationRequested)
            {
                T curValue = lastValueRead;
                try
                {
                    curValue = await queue.DequeueAsync(cancelToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                list.Add(curValue);

                afterReadValue(curValue, lastValueRead);

                lastValueRead = curValue;
            }

            return list;
        }

        Configuration testConfig = Configuration.QuickThrowOnFailure;
    }
}
