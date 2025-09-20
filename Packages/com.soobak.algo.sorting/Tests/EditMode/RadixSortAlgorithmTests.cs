using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Soobak.Algo.Core;
using UnityEngine;
using UnityEngine.TestTools;

namespace Soobak.Algo.Sorting.Tests {
  public class RadixSortAlgorithmTests {
    [Test]
    public async Task ExecuteAsync_SortsAscendingValues() {
      var algorithm = new RadixSortAlgorithm();
      var state = SortingState.FromValues(new[] { 170, 45, 75, 90, 802, 24, 2, 66 });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 2, 24, 45, 66, 75, 90, 170, 802 }));
      Assert.That(sink.Steps.Any(step => step.Event.Type == SortOpType.Highlight), Is.True);
      Assert.That(sink.Steps.Last().Event.Type, Is.EqualTo(SortOpType.Finalize));
    }

    [Test]
    public async Task ExecuteAsync_MaintainsStability() {
      var algorithm = new RadixSortAlgorithm();
      var state = SortingState.FromLabeledValues(new[] {
        (21, "A"),
        (4, "B"),
        (21, "C"),
        (4, "D"),
        (3, "E")
      });
      var sink = new RecordingSink();

      await algorithm.ExecuteAsync(state, sink, CancellationToken.None);

      Assert.That(sink.LastSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 3, 4, 4, 21, 21 }));
      Assert.That(sink.LastSnapshot.Items.Where(item => item.Value == 4).Select(item => item.StableId), Is.EqualTo(new[] { "B", "D" }));
      Assert.That(sink.LastSnapshot.Items.Where(item => item.Value == 21).Select(item => item.StableId), Is.EqualTo(new[] { "A", "C" }));
    }

    [Test]
    public async Task ExecuteAsync_ThrowsForNegativeValues() {
      var algorithm = new RadixSortAlgorithm();
      var state = SortingState.FromValues(new[] { 3, -1, 2 });

      LogAssert.Expect(LogType.Error, "RadixSortAlgorithm: Execution failed. System.NotSupportedException: RadixSortAlgorithm currently supports non-negative integers only.");
      Assert.ThrowsAsync<NotSupportedException>(async () => await algorithm.ExecuteAsync(state, new NoOpSink(), CancellationToken.None));
    }

    [Test]
    public async Task ExecuteAsync_RespectsCancellation() {
      var algorithm = new RadixSortAlgorithm();
      var state = SortingState.FromValues(new[] { 3, 1, 2 });
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

    sealed class NoOpSink : IAlgorithmStepSink<SortingState, SortOp> {
      public UniTask InitializeAsync(SortingState initialState, CancellationToken cancellationToken) => UniTask.CompletedTask;
      public UniTask PublishAsync(AlgorithmStep<SortingState, SortOp> step, CancellationToken cancellationToken) => UniTask.CompletedTask;
      public UniTask CompleteAsync(SortingState finalState, CancellationToken cancellationToken) => UniTask.CompletedTask;
    }
  }
}
