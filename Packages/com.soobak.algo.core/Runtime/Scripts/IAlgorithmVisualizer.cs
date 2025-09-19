using System.Threading;
using Cysharp.Threading.Tasks;

namespace Soobak.Algo.Core {
  public interface IAlgorithmVisualizer<TState, TEvent> {
    UniTask InitializeAsync(TState state, CancellationToken cancellationToken);

    UniTask ApplyAsync(TEvent evt, TState state, CancellationToken cancellationToken);

    UniTask CompleteAsync(TState state, CancellationToken cancellationToken);
  }
}
