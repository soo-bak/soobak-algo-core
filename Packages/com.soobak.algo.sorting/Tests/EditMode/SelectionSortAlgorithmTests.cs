using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Soobak.Algo.Core;

namespace Soobak.Algo.Sorting.Tests {
  public class SelectionSortAlgorithmTests {
    [Test]
    public async Task SortsAscendingValues() {
      var algorithm = new SelectionSortAlgorithm();
      var state = SortingState.FromValues(new[] { 5, 1, 4, 2, 8 });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 4, 5, 8 }));
      Assert.That(sink.Steps.Any(step => step.Event.Type == SortOpType.Swap), Is.True);
      Assert.That(sink.Steps.Last().Event.Type, Is.EqualTo(SortOpType.Finalize));
    }

    [Test]
    public async Task HandlesAlreadySortedSequence() {
      var algorithm = new SelectionSortAlgorithm();
      var state = SortingState.FromValues(new[] { 1, 2, 3, 4 });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 4 }));
      Assert.That(sink.Steps.Count(step => step.Event.Type == SortOpType.Swap), Is.EqualTo(0));
      Assert.That(sink.Steps.Last().Event.Type, Is.EqualTo(SortOpType.Finalize));
    }

    [Test]
    public void ThrowsOnNullState() {
      var algorithm = new SelectionSortAlgorithm();

      Assert.ThrowsAsync<ArgumentNullException>(async () => await algorithm.ExecuteAsync(null!, new RecordingSink(), CancellationToken.None));
    }

    [Test]
    public void ThrowsOnNullSink() {
      var algorithm = new SelectionSortAlgorithm();
      var state = SortingState.FromValues(new[] { 2, 1 });

      Assert.ThrowsAsync<ArgumentNullException>(async () => await algorithm.ExecuteAsync(state, null!, CancellationToken.None));
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
