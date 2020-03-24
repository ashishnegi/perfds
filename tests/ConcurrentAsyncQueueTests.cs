using FsCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
        public void ConcurrentEnqueAndDequeueHappensInOrderMultiReaders()
        {
            Prop.ForAll<int[]>(xs =>
            {
                Array.Sort(xs);

                var queue = new ConcurrentAsyncQueue<int>();
                Task writerTask = new Task(() =>
                {
                    foreach (var x in xs)
                    {
                        queue.Enqueue(x);
                    }
                });

                var cancelSource = new CancellationTokenSource();
                CancellationToken cancelToken = cancelSource.Token;

                long remainingToRead = xs.Length;

                Action readerAction = async () =>
                {
                    int lastValueRead = -1;
                    if (xs.Length > 0)
                    {
                        lastValueRead = xs[0];
                    }

                    while (!cancelToken.IsCancellationRequested)
                    {
                        int curValue = lastValueRead;
                        try
                        {
                            curValue = await queue.DequeueAsync(cancelToken);
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }

                        Interlocked.Decrement(ref remainingToRead);

                        Assert.IsTrue(curValue >= lastValueRead);
                        lastValueRead = curValue;
                    }
                };

                var stopTask = new Task(async () =>
                {
                    while (Interlocked.Read(ref remainingToRead) > 0)
                    {
                        await Task.Delay(10);
                    }

                    cancelSource.Cancel();
                });

                var allTasks = new List<Task>();
                allTasks.Add(new Task(readerAction));
                allTasks.Add(new Task(readerAction));
                allTasks.Add(writerTask);
                allTasks.Add(new Task(readerAction));
                allTasks.Add(new Task(readerAction));
                allTasks.Add(stopTask);

                foreach (var t in allTasks)
                {
                    t.Start();
                }

                Task.WaitAll(allTasks.ToArray());
            }).Check(testConfig);
        }

        Configuration testConfig = Configuration.QuickThrowOnFailure;
    }
}
