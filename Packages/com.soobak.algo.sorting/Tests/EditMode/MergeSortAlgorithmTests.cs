using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Soobak.Algo.Core;

namespace Soobak.Algo.Sorting.Tests {
  public class MergeSortAlgorithmTests {
    [Test]
    public async Task SortsAscendingAndMaintainsStability() {
      var algorithm = new MergeSortAlgorithm();
      var state = SortingState.FromLabeledValues(new[] {
        (4, "A"),
        (1, "B"),
        (3, "C"),
        (3, "D"),
        (2, "E")
      });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 3, 4 }));
      Assert.That(sink.LastSnapshot.Items.Where(item => item.Value == 3).Select(item => item.StableId), Is.EqualTo(new[] { "C", "D" }));
      Assert.That(sink.Steps.Any(step => step.Event.Type == SortOpType.Insert), Is.True);
      Assert.That(sink.Steps.Last().Event.Type, Is.EqualTo(SortOpType.Finalize));
    }

    [Test]
    public async Task HandlesDescendingSequence() {
      var algorithm = new MergeSortAlgorithm();
      var state = SortingState.FromValues(new[] { 6, 5, 4, 3, 2, 1 });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 4, 5, 6 }));
      Assert.That(sink.Steps.Count(step => step.Event.Type == SortOpType.Compare), Is.GreaterThan(0));
    }

    [Test]
    public async Task AllowsAlreadySortedSequence() {
      var algorithm = new MergeSortAlgorithm();
      var state = SortingState.FromValues(new[] { 1, 2, 3, 4 });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 4 }));
      Assert.That(sink.Steps.Last().Event.Type, Is.EqualTo(SortOpType.Finalize));
    }

    [Test]
    public void ThrowsOnNullState() {
      var algorithm = new MergeSortAlgorithm();

      Assert.ThrowsAsync<ArgumentNullException>(async () => await algorithm.ExecuteAsync(null!, new RecordingSink(), CancellationToken.None));
    }

    [Test]
    public void ThrowsOnNullSink() {
      var algorithm = new MergeSortAlgorithm();
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
