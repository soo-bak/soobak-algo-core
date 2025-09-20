using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Soobak.Algo.Core;

namespace Soobak.Algo.Sorting.Tests {
  public class HeapSortAlgorithmTests {
    [Test]
    public async Task ExecuteAsync_SortsAscendingValues() {
      var algorithm = new HeapSortAlgorithm();
      var state = SortingState.FromValues(new[] { 9, 4, 6, 2, 3, 8 });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 2, 3, 4, 6, 8, 9 }));
      Assert.That(sink.Steps.Any(step => step.Event.Type == SortOpType.Swap), Is.True);
      Assert.That(sink.Steps.Last().Event.Type, Is.EqualTo(SortOpType.Finalize));
    }

    [Test]
    public async Task ExecuteAsync_HandlesDuplicateValues() {
      var algorithm = new HeapSortAlgorithm();
      var state = SortingState.FromLabeledValues(new[] {
        (5, "A"),
        (3, "B"),
        (5, "C"),
        (1, "D"),
        (2, "E")
      });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 5, 5 }));
      Assert.That(sink.Steps.Any(step => step.Event.Type == SortOpType.Swap), Is.True);
    }

    [Test]
    public async Task ExecuteAsync_RespectsCancellation() {
      var algorithm = new HeapSortAlgorithm();
      var state = SortingState.FromValues(new[] { 4, 3, 2, 1 });
      var sink = new RecordingSink();
      using var cts = new CancellationTokenSource();
      cts.Cancel();

      Assert.ThrowsAsync<TaskCanceledException>(async () => await algorithm.ExecuteAsync(state, sink, cts.Token));
    }

    sealed class RecordingSink : IAlgorithmStepSink<SortingState, SortOp> {
      public List<AlgorithmStep<SortingState, SortOp>> Steps { get; } = new();
      public SortingState LastSnapshot { get; private set; } = SortingState.FromValues(Array.Empty<int>());

      public UniTask InitializeAsync(SortingState initialState, CancellationToken cancellationToken) {
        LastSnapshot = initialState.Clone();
        return UniTask.CompletedTask;
      }

      public UniTask PublishAsync(AlgorithmStep<SortingState, SortOp> step, CancellationToken cancellationToken) {
        Steps.Add(step);
        LastSnapshot = step.Snapshot.Clone();
        return UniTask.CompletedTask;
      }

      public UniTask CompleteAsync(SortingState finalState, CancellationToken cancellationToken) {
        LastSnapshot = finalState.Clone();
        return UniTask.CompletedTask;
      }
    }
  }
}
