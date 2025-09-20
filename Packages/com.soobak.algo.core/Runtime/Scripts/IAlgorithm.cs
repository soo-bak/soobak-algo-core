using System.Threading;
using Cysharp.Threading.Tasks;

namespace Soobak.Algo.Core {
  public interface IAlgorithm<TState, TEvent> {
    string Id { get; }

    UniTask ExecuteAsync(TState state, IAlgorithmStepSink<TState, TEvent> sink, CancellationToken cancellationToken);
  }
}
