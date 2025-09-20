using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Soobak.Algo.Core;

namespace Soobak.Algo.Sorting.Tests {
  public class BubbleSortAlgorithmTests {
    [Test]
    public async Task ExecuteAsync_SortsAscendingAndMaintainsStability() {
      var algorithm = new BubbleSortAlgorithm();
      var state = SortingState.FromLabeledValues(new[] {
        (4, "A"),
        (2, "B"),
        (2, "C"),
        (3, "D"),
        (1, "E")
      });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 2, 3, 4 }));
      Assert.That(sink.LastSnapshot.Items.Where(item => item.Value == 2).Select(item => item.StableId), Is.EqualTo(new[] { "B", "C" }));
      Assert.That(sink.Steps.Any(step => step.Event.Type == SortOpType.Swap), Is.True);
      Assert.That(sink.Steps.Last().Event.Type, Is.EqualTo(SortOpType.Finalize));
    }

    [Test]
    public async Task ExecuteAsync_ShortCircuitsOnSortedSequence() {
      var algorithm = new BubbleSortAlgorithm();
      var state = SortingState.FromValues(new[] { 1, 2, 3, 4 });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.Steps.Count(step => step.Event.Type == SortOpType.Swap), Is.EqualTo(0));
      Assert.That(sink.Steps.Count(step => step.Event.Type == SortOpType.Compare), Is.GreaterThan(0));
      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 4 }));
    }

    [Test]
    public async Task ExecuteAsync_RespectsCancellation() {
      var algorithm = new BubbleSortAlgorithm();
      var state = SortingState.FromValues(new[] { 3, 2, 1 });
      var sink = new RecordingSink();
      using var cts = new CancellationTokenSource();
      cts.Cancel();

      try {
        await algorithm.ExecuteAsync(state, sink, cts.Token);
        Assert.Fail("Expected OperationCanceledException");
      }
      catch (OperationCanceledException) {
      }
      catch (Exception ex) {
        Assert.Fail($"Unexpected exception: {ex}");
      }
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
