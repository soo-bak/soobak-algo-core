using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Soobak.Algo.Core {
  public interface IAlgorithm<TState, TEvent> {
    string Id { get; }

    UniTask ExecuteAsync(TState state, Func<TEvent, UniTask> publishEvent, CancellationToken cancellationToken);
  }
}
