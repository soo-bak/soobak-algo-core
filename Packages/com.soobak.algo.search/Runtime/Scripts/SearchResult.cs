namespace Soobak.Algo.Search {
  public sealed class SearchResult<TItem> {
    public SearchResult(TItem item, int index, int comparisons = 0) {
      Item = item;
      Index = index;
      Comparisons = comparisons;
    }

    public TItem Item { get; }

    public int Index { get; }

    public int Comparisons { get; }
  }
}
