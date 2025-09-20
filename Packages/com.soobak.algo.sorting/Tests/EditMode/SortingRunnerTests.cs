using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Soobak.Algo.Core;

namespace Soobak.Algo.Sorting.Tests {
  public class SortingRunnerTests {
    [Test]
    public async Task ExecuteAsync_InvokesVisualizerAndReturnsSortedState() {
      var algorithm = new InsertionSortAlgorithm();
      var visualizer = new RecordingVisualizer();
      var runner = new SortingRunner(visualizer);
      var initial = SortingState.FromValues(new[] { 4, 1, 3, 2 });
      var original = initial.Clone();

      var result = await runner.ExecuteAsync(algorithm, initial, CancellationToken.None);

      Assert.That(original.Items.Select(item => item.Value), Is.EqualTo(new[] { 4, 1, 3, 2 }));
      Assert.That(result.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 4 }));
      Assert.That(visualizer.InitialSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 4, 1, 3, 2 }));
      Assert.That(visualizer.CompletedSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 4 }));
      Assert.That(visualizer.Events.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task ExecuteAsync_ThrowsWhenCancelledBeforeStart() {
      var algorithm = new InsertionSortAlgorithm();
      var visualizer = new RecordingVisualizer();
      var runner = new SortingRunner(visualizer);
      var initial = SortingState.FromValues(new[] { 2, 1 });

      using var cts = new CancellationTokenSource();
      cts.Cancel();

      try {
        await runner.ExecuteAsync(algorithm, initial, cts.Token);
        Assert.Fail("Expected OperationCanceledException");
      }
      catch (OperationCanceledException) {
      }
      catch (Exception ex) {
        Assert.Fail($"Unexpected exception: {ex}");
      }
    }

    [Test]
    public async Task ExecuteAsync_PropagatesIntermediateStepsToAllVisualizers() {
      var algorithm = new InsertionSortAlgorithm();
      var first = new RecordingVisualizer();
      var second = new RecordingVisualizer();
      var runner = new SortingRunner(first, second);
      var initial = SortingState.FromValues(new[] { 5, 3, 4 });

      await runner.ExecuteAsync(algorithm, initial, CancellationToken.None);

      Assert.That(first.Events.Count, Is.EqualTo(second.Events.Count));
      Assert.That(first.Events.Select(step => step.Event.Type), Is.EqualTo(second.Events.Select(step => step.Event.Type)));
    }

    sealed class RecordingVisualizer : IBarVisualizer {
      public SortingState InitialSnapshot { get; private set; } = SortingState.FromValues(Array.Empty<int>());
      public SortingState CompletedSnapshot { get; private set; } = SortingState.FromValues(Array.Empty<int>());
      public List<AlgorithmStep<SortingState, SortOp>> Events { get; } = new();

      public UniTask InitializeAsync(SortingState initialState, CancellationToken cancellationToken) {
        InitialSnapshot = initialState.Clone();
        return UniTask.CompletedTask;
      }

      public UniTask PublishAsync(AlgorithmStep<SortingState, SortOp> step, CancellationToken cancellationToken) {
        Events.Add(step);
        return UniTask.CompletedTask;
      }

      public UniTask CompleteAsync(SortingState finalState, CancellationToken cancellationToken) {
        CompletedSnapshot = finalState.Clone();
        return UniTask.CompletedTask;
      }
    }
  }
}
