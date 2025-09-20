using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace Soobak.Algo.Sorting.Tests {
  public class SortingRunnerTests {
    [Test]
    public async Task RunAsync_InvokesVisualizerAndReturnsSortedSnapshot() {
      var algorithm = new InsertionSortAlgorithm();
      var visualizer = new RecordingVisualizer();
      var runner = new SortingRunner(algorithm, visualizer);
      var items = new[] {
        new SortItem(4, "A"),
        new SortItem(1, "B"),
        new SortItem(3, "C"),
        new SortItem(2, "D")
      };

      var result = await runner.RunAsync(items, CancellationToken.None);

      Assert.That(visualizer.InitSnapshot.Select(i => i.Value), Is.EqualTo(new[] { 4, 1, 3, 2 }));
      Assert.That(visualizer.Events.Count, Is.GreaterThan(0));
      Assert.That(result.Select(i => i.Value), Is.EqualTo(new[] { 1, 2, 3, 4 }));
      Assert.That(visualizer.CompletedSnapshot.Select(i => i.Value), Is.EqualTo(new[] { 1, 2, 3, 4 }));
    }

    [Test]
    public void RunAsync_ThrowsWhenCanceledBeforeStart() {
      var algorithm = new InsertionSortAlgorithm();
      var visualizer = new RecordingVisualizer();
      var runner = new SortingRunner(algorithm, visualizer);
      var items = new[] {
        new SortItem(2, "A"),
        new SortItem(1, "B")
      };

      using var cts = new CancellationTokenSource();
      cts.Cancel();

      Assert.ThrowsAsync<OperationCanceledException>(async () => await runner.RunAsync(items, cts.Token));
    }

    [Test]
    public async Task RunAsync_DoesNotMutateInputSequence() {
      var algorithm = new InsertionSortAlgorithm();
      var visualizer = new RecordingVisualizer();
      var runner = new SortingRunner(algorithm, visualizer);
      var items = new[] {
        new SortItem(5, "A"),
        new SortItem(3, "B"),
        new SortItem(4, "C")
      };

      var snapshot = items.Select(i => i.Value).ToArray();
      _ = await runner.RunAsync(items, CancellationToken.None);

      Assert.That(items.Select(i => i.Value), Is.EqualTo(snapshot));
    }

    sealed class RecordingVisualizer : IBarVisualizer {
      public IReadOnlyList<SortItem> InitSnapshot { get; private set; } = new List<SortItem>();
      public IReadOnlyList<SortItem> CompletedSnapshot { get; private set; } = new List<SortItem>();
      public List<SortOp> Events { get; } = new List<SortOp>();

      public UniTask InitializeAsync(IReadOnlyList<SortItem> items, CancellationToken cancellationToken) {
        InitSnapshot = Clone(items);
        return UniTask.CompletedTask;
      }

      public UniTask ApplyAsync(SortOp op, IReadOnlyList<SortItem> items, CancellationToken cancellationToken) {
        Events.Add(op);
        CompletedSnapshot = Clone(items);
        return UniTask.CompletedTask;
      }

      public UniTask CompleteAsync(IReadOnlyList<SortItem> items, CancellationToken cancellationToken) {
        CompletedSnapshot = Clone(items);
        return UniTask.CompletedTask;
      }

      static ReadOnlyCollection<SortItem> Clone(IReadOnlyList<SortItem> items) {
        return new ReadOnlyCollection<SortItem>(items.Select(i => new SortItem(i.Value, i.Label)).ToList());
      }
    }
  }
}
