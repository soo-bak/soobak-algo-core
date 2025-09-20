using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace Soobak.Algo.Core.Tests {
  public class AlgorithmRunnerTests {
    [Test]
    public async Task ExecuteAsync_ClonesStateAndBroadcastsSteps() {
      var algorithm = new RecordingAlgorithm();
      var sink = new RecordingSink();
      var runner = new AlgorithmRunner<List<int>, string>(Clone, sink);
      var input = new List<int> { 1, 2, 3 };

      var result = await runner.ExecuteAsync(algorithm, input, CancellationToken.None);

      Assert.That(input, Is.EqualTo(new[] { 1, 2, 3 }));
      Assert.That(result, Is.EqualTo(new[] { 1, 2, 3, RecordingAlgorithm.Added }));
      Assert.That(sink.InitialSnapshots.Single(), Is.EqualTo(new[] { 1, 2, 3 }));
      Assert.That(sink.PublishedSteps.Select(step => step.Event), Is.EqualTo(new[] { RecordingAlgorithm.Step }));
      Assert.That(sink.CompletedSnapshots.Single(), Is.EqualTo(new[] { 1, 2, 3, RecordingAlgorithm.Added }));
      Assert.That(sink.PublishedSteps.Single().Snapshot, Is.Not.SameAs(result));
    }

    [Test]
    public async Task ExecuteAsync_ThrowsWhenClonerReturnsNull() {
      var algorithm = new RecordingAlgorithm();
      var sink = new RecordingSink();
      var runner = new AlgorithmRunner<List<int>, string>(_ => null!, sink);

      try {
        await runner.ExecuteAsync(algorithm, new List<int>(), CancellationToken.None);
        Assert.Fail("Expected InvalidOperationException");
      }
      catch (InvalidOperationException) {
      }
      catch (Exception ex) {
        Assert.Fail($"Unexpected exception: {ex}");
      }
    }

    [Test]
    public async Task ExecuteAsync_PropagatesCancellationBeforeRun() {
      var algorithm = new DelayAlgorithm();
      var sink = new RecordingSink();
      var runner = new AlgorithmRunner<List<int>, string>(Clone, sink);

      using var cts = new CancellationTokenSource();
      cts.Cancel();

      try {
        await runner.ExecuteAsync(algorithm, new List<int>(), cts.Token);
        Assert.Fail("Expected OperationCanceledException");
      }
      catch (OperationCanceledException) {
      }
      catch (Exception ex) {
        Assert.Fail($"Unexpected exception: {ex}");
      }
    }

    [Test]
    public async Task ExecuteAsync_PropagatesCancellationDuringRun() {
      var algorithm = new DelayAlgorithm();
      var sink = new RecordingSink();
      var runner = new AlgorithmRunner<List<int>, string>(Clone, sink);

      using var cts = new CancellationTokenSource();
      var job = runner.ExecuteAsync(algorithm, new List<int>(), cts.Token);
      cts.Cancel();

      try {
        await job;
        Assert.Fail("Expected OperationCanceledException");
      }
      catch (OperationCanceledException) {
      }
      catch (Exception ex) {
        Assert.Fail($"Unexpected exception: {ex}");
      }
    }

    static List<int> Clone(List<int> source) {
      return source.ToList();
    }

    sealed class RecordingAlgorithm : IAlgorithm<List<int>, string> {
      public const string Step = "step";
      public const int Added = 42;

      public string Id => "recording";

      public async UniTask ExecuteAsync(List<int> state, IAlgorithmStepSink<List<int>, string> sink, CancellationToken cancellationToken) {
        state.Add(Added);
        var snapshot = state.ToList();
        await sink.PublishAsync(new AlgorithmStep<List<int>, string>(snapshot, Step), cancellationToken);
      }
    }

    sealed class DelayAlgorithm : IAlgorithm<List<int>, string> {
      public string Id => "delay";

      public async UniTask ExecuteAsync(List<int> state, IAlgorithmStepSink<List<int>, string> sink, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
      }
    }

    sealed class RecordingSink : IAlgorithmStepSink<List<int>, string> {
      public List<List<int>> InitialSnapshots { get; } = new();
      public List<AlgorithmStep<List<int>, string>> PublishedSteps { get; } = new();
      public List<List<int>> CompletedSnapshots { get; } = new();

      public UniTask InitializeAsync(List<int> initialState, CancellationToken cancellationToken) {
        InitialSnapshots.Add(initialState);
        return UniTask.CompletedTask;
      }

      public UniTask PublishAsync(AlgorithmStep<List<int>, string> step, CancellationToken cancellationToken) {
        PublishedSteps.Add(step);
        return UniTask.CompletedTask;
      }

      public UniTask CompleteAsync(List<int> finalState, CancellationToken cancellationToken) {
        CompletedSnapshots.Add(finalState);
        return UniTask.CompletedTask;
      }
    }
  }
}
