using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Soobak.Algo.Core;

namespace Soobak.Algo.Sorting.Tests {
  public class ShellSortAlgorithmTests {
    [Test]
    public async Task ExecuteAsync_SortsAscendingValues() {
      var algorithm = new ShellSortAlgorithm();
      var state = SortingState.FromValues(new[] { 12, 7, 3, 1, 9, 4 });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 3, 4, 7, 9, 12 }));
      Assert.That(sink.Steps.Any(step => step.Event.Type == SortOpType.Insert), Is.True);
      Assert.That(sink.Steps.Last().Event.Type, Is.EqualTo(SortOpType.Finalize));
    }

    [Test]
    public async Task ExecuteAsync_HandlesDuplicateValues() {
      var algorithm = new ShellSortAlgorithm();
      var state = SortingState.FromLabeledValues(new[] {
        (4, "A"),
        (2, "B"),
        (2, "C"),
        (5, "D"),
        (1, "E")
      });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 2, 4, 5 }));
      Assert.That(sink.Steps.Count(step => step.Event.Type == SortOpType.Insert), Is.GreaterThan(0));
    }

    [Test]
    public async Task ExecuteAsync_RespectsCancellation() {
      var algorithm = new ShellSortAlgorithm();
      var state = SortingState.FromValues(new[] { 3, 2, 1 });
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
