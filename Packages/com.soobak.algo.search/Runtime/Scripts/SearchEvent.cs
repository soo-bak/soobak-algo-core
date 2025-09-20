namespace Soobak.Algo.Search {
  public sealed class SearchEvent<TItem, TKey> {
    public SearchEvent(SearchEventType type, TItem? item, int index, TKey searchKey, bool isMatch = false) {
      Type = type;
      Item = item;
      Index = index;
      SearchKey = searchKey;
      IsMatch = isMatch;
    }

    public SearchEventType Type { get; }

    public TItem? Item { get; }

    public int Index { get; }

    public TKey SearchKey { get; }

    public bool IsMatch { get; }
  }

  public enum SearchEventType {
    Examine,
    Found,
    NotFound,
    Complete
  }
}
