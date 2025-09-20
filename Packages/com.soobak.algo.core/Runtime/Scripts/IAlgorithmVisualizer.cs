using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Soobak.Algo.Core {
  public readonly struct AlgorithmStep<TState, TEvent> {
    public AlgorithmStep(TState snapshot, TEvent evt, string message = "", DateTimeOffset? timestamp = null) {
      Snapshot = snapshot;
      Event = evt;
      Message = message;
      Timestamp = timestamp ?? DateTimeOffset.UtcNow;
    }

    public TState Snapshot { get; }

    public TEvent Event { get; }

    public string Message { get; }

    public DateTimeOffset Timestamp { get; }
  }

  public interface IAlgorithmStepSink<TState, TEvent> {
    UniTask InitializeAsync(TState initialState, CancellationToken cancellationToken);

    UniTask PublishAsync(AlgorithmStep<TState, TEvent> step, CancellationToken cancellationToken);

    UniTask CompleteAsync(TState finalState, CancellationToken cancellationToken);
  }

  public interface IAlgorithmVisualizer<TState, TEvent> : IAlgorithmStepSink<TState, TEvent> {
  }
}
