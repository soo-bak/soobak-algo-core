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
    public async Task RunAsync_ClonesStateAndInvokesVisualizer() {
      var algorithm = new RecordingAlgorithm();
      var visualizer = new RecordingVisualizer();
      var runner = new AlgorithmRunner<List<int>, string>(algorithm, visualizer, Clone);

      var input = new List<int> { 1, 2, 3 };
      var result = await runner.RunAsync(input, CancellationToken.None);

      Assert.That(input, Is.EqualTo(new[] { 1, 2, 3 }));
      Assert.That(result, Is.EqualTo(new[] { 1, 2, 3, RecordingAlgorithm.Added }));
      Assert.That(visualizer.InitSnapshots.Single(), Is.EqualTo(new[] { 1, 2, 3 }));
      Assert.That(visualizer.Events, Is.EqualTo(new[] { RecordingAlgorithm.Step }));
      Assert.That(visualizer.CompleteSnapshots.Last(), Is.EqualTo(new[] { 1, 2, 3, RecordingAlgorithm.Added }));
    }

    [Test]
    public void RunAsync_ThrowsWhenStateClonerReturnsNull() {
      var algorithm = new RecordingAlgorithm();
      var visualizer = new RecordingVisualizer();
      var runner = new AlgorithmRunner<List<int>, string>(algorithm, visualizer, _ => null!);

      Assert.That(async () => await runner.RunAsync(new List<int>(), CancellationToken.None), Throws.InvalidOperationException);
    }

    [Test]
    public void RunAsync_CancelsWhenTokenIsTriggered() {
      var algorithm = new DelayAlgorithm();
      var visualizer = new RecordingVisualizer();
      var runner = new AlgorithmRunner<List<int>, string>(algorithm, visualizer, Clone);

      using var cts = new CancellationTokenSource();
      cts.Cancel();

      Assert.ThrowsAsync<OperationCanceledException>(async () => await runner.RunAsync(new List<int>(), cts.Token));
    }

    static List<int> Clone(List<int> source) {
      return source.Select(v => v).ToList();
    }

    sealed class RecordingAlgorithm : IAlgorithm<List<int>, string> {
      public const string Step = "step";
      public const int Added = 42;

      public string Id => "recording";

      public async UniTask ExecuteAsync(List<int> state, Func<string, UniTask> publishEvent, CancellationToken cancellationToken) {
        state.Add(Added);
        await publishEvent(Step);
      }
    }

    sealed class DelayAlgorithm : IAlgorithm<List<int>, string> {
      public string Id => "delay";

      public async UniTask ExecuteAsync(List<int> state, Func<string, UniTask> publishEvent, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
      }
    }

    sealed class RecordingVisualizer : IAlgorithmVisualizer<List<int>, string> {
      public List<List<int>> InitSnapshots { get; } = new();
      public List<string> Events { get; } = new();
      public List<List<int>> CompleteSnapshots { get; } = new();

      public UniTask InitializeAsync(List<int> state, CancellationToken cancellationToken) {
        InitSnapshots.Add(state.ToList());
        return UniTask.CompletedTask;
      }

      public UniTask ApplyAsync(string evt, List<int> state, CancellationToken cancellationToken) {
        Events.Add(evt);
        CompleteSnapshots.Add(state.ToList());
        return UniTask.CompletedTask;
      }

      public UniTask CompleteAsync(List<int> state, CancellationToken cancellationToken) {
        CompleteSnapshots.Add(state.ToList());
        return UniTask.CompletedTask;
      }
    }
  }
}
