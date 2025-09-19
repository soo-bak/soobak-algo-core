using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace Soobak.Algo.Sorting.Tests
{
    public class SortingRunnerTests
    {
        [Test]
        public async Task RunAsync_InvokesVisualizerAndReturnsSortedSnapshot()
        {
            var algorithm = new InsertionSortAlgorithm();
            var visualizer = new RecordingVisualizer();
            var runner = new SortingRunner(algorithm, visualizer);
            var items = new[]
            {
                new SortItem(4, "A"),
                new SortItem(1, "B"),
                new SortItem(3, "C"),
                new SortItem(2, "D")
            };

            var result = await runner.RunAsync(items, CancellationToken.None);

            Assert.That(visualizer.InitialSnapshot.Select(i => i.Value), Is.EqualTo(new[] { 4, 1, 3, 2 }));
            Assert.That(visualizer.AppliedOperations.Count, Is.GreaterThan(0));
            Assert.That(result.Select(i => i.Value), Is.EqualTo(new[] { 1, 2, 3, 4 }));
            Assert.That(visualizer.CompletedSnapshot.Select(i => i.Value), Is.EqualTo(new[] { 1, 2, 3, 4 }));
        }

        [Test]
        public void RunAsync_ThrowsWhenCanceledBeforeStart()
        {
            var algorithm = new InsertionSortAlgorithm();
            var visualizer = new RecordingVisualizer();
            var runner = new SortingRunner(algorithm, visualizer);
            var items = new[]
            {
                new SortItem(2, "A"),
                new SortItem(1, "B")
            };

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsAsync<OperationCanceledException>(async () => await runner.RunAsync(items, cts.Token));
        }

        private sealed class RecordingVisualizer : IBarVisualizer
        {
            public IReadOnlyList<SortItem> InitialSnapshot { get; private set; } = new List<SortItem>();

            public IReadOnlyList<SortItem> CompletedSnapshot { get; private set; } = new List<SortItem>();

            public List<SortOp> AppliedOperations { get; } = new List<SortOp>();

            public UniTask InitializeAsync(IReadOnlyList<SortItem> items, CancellationToken cancellationToken)
            {
                InitialSnapshot = Clone(items);
                return UniTask.CompletedTask;
            }

            public UniTask ApplyAsync(SortOp operation, IReadOnlyList<SortItem> items, CancellationToken cancellationToken)
            {
                AppliedOperations.Add(operation);
                CompletedSnapshot = Clone(items);
                return UniTask.CompletedTask;
            }

            public UniTask CompleteAsync(IReadOnlyList<SortItem> items, CancellationToken cancellationToken)
            {
                CompletedSnapshot = Clone(items);
                return UniTask.CompletedTask;
            }

            private static ReadOnlyCollection<SortItem> Clone(IReadOnlyList<SortItem> items)
            {
                return new ReadOnlyCollection<SortItem>(items.Select(item => new SortItem(item.Value, item.Label)).ToList());
            }
        }
    }
}
