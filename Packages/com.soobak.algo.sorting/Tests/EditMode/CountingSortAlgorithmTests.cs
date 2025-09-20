using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Soobak.Algo.Core;

namespace Soobak.Algo.Sorting.Tests {
  public class CountingSortAlgorithmTests {
    [Test]
    public async Task ExecuteAsync_SortsAscendingValues() {
      var algorithm = new CountingSortAlgorithm();
      var state = SortingState.FromValues(new[] { 3, 1, 4, 1, 5, 9 });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 1, 3, 4, 5, 9 }));
      Assert.That(sink.Steps.Any(step => step.Event.Type == SortOpType.Highlight), Is.True);
      Assert.That(sink.Steps.Last().Event.Type, Is.EqualTo(SortOpType.Finalize));
    }

    [Test]
    public async Task ExecuteAsync_MaintainsStability() {
      var algorithm = new CountingSortAlgorithm();
      var state = SortingState.FromLabeledValues(new[] {
        (2, "A"),
        (1, "B"),
        (2, "C"),
        (1, "D"),
        (3, "E")
      });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 1, 2, 2, 3 }));
      Assert.That(sink.LastSnapshot.Items.Where(item => item.Value == 1).Select(item => item.StableId), Is.EqualTo(new[] { "B", "D" }));
      Assert.That(sink.LastSnapshot.Items.Where(item => item.Value == 2).Select(item => item.StableId), Is.EqualTo(new[] { "A", "C" }));
    }

    [Test]
    public async Task ExecuteAsync_RespectsCancellation() {
      var algorithm = new CountingSortAlgorithm();
      var state = SortingState.FromValues(new[] { 2, 1, 3 });
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
