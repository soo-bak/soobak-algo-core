using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;

namespace Soobak.Algo.Search {
  public interface ISearchAlgorithm<TItem, TKey> : IAlgorithm<SearchState<TItem, TKey>, SearchEvent<TItem, TKey>> {
    UniTask<SearchResult<TItem>?> SearchAsync(SearchState<TItem, TKey> state, TKey key, CancellationToken token);
  }
}
