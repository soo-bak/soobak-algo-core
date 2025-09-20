using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Soobak.Algo.Core;

namespace Soobak.Algo.Sorting.Tests {
  public class QuickSortAlgorithmTests {
    [Test]
    public async Task SortsAscendingValues() {
      var algorithm = new QuickSortAlgorithm();
      var state = SortingState.FromValues(new[] { 10, 7, 8, 9, 1, 5 });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 5, 7, 8, 9, 10 }));
      Assert.That(sink.Steps.Any(step => step.Event.Type == SortOpType.Pivot), Is.True);
      Assert.That(sink.Steps.Any(step => step.Event.Type == SortOpType.Partition), Is.True);
    }

    [Test]
    public async Task HandlesEqualValues() {
      var algorithm = new QuickSortAlgorithm();
      var state = SortingState.FromLabeledValues(new[] {
        (3, "A"),
        (3, "B"),
        (2, "C"),
        (2, "D"),
        (1, "E")
      });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 2, 3, 3 }));
      Assert.That(sink.LastSnapshot.Items.Where(item => item.Value == 2).Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task HandlesAlreadySortedSequence() {
      var algorithm = new QuickSortAlgorithm();
      var state = SortingState.FromValues(new[] { 1, 2, 3, 4, 5 });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
      Assert.That(sink.Steps.Any(step => step.Event.Type == SortOpType.Partition), Is.True);
      Assert.That(sink.Steps.Last().Event.Type, Is.EqualTo(SortOpType.Finalize));
    }

    [Test]
    public void ThrowsOnNullState() {
      var algorithm = new QuickSortAlgorithm();

      Assert.ThrowsAsync<ArgumentNullException>(async () => await algorithm.ExecuteAsync(null!, new RecordingSink(), CancellationToken.None));
    }

    [Test]
    public void ThrowsOnNullSink() {
      var algorithm = new QuickSortAlgorithm();
      var state = SortingState.FromValues(new[] { 3, 1 });

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
