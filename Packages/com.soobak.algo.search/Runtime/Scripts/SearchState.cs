using System;
using System.Collections.Generic;

namespace Soobak.Algo.Search {
  public sealed class SearchState<TItem, TKey> {
    public SearchState(IReadOnlyList<TItem> items, TKey searchKey, int currentIndex = 0, bool isComplete = false, SearchResult<TItem>? result = null) {
      Items = items;
      SearchKey = searchKey;
      CurrentIndex = currentIndex;
      IsComplete = isComplete;
      Result = result;
    }

    public IReadOnlyList<TItem> Items { get; }

    public TKey SearchKey { get; }

    public int CurrentIndex { get; }

    public bool IsComplete { get; }

    public SearchResult<TItem>? Result { get; }

    public SearchState<TItem, TKey> With(int? currentIndex = null, bool? isComplete = null, SearchResult<TItem>? result = null) {
      return new SearchState<TItem, TKey>(
        Items,
        SearchKey,
        currentIndex ?? CurrentIndex,
        isComplete ?? IsComplete,
        result ?? Result
      );
    }
  }
}
