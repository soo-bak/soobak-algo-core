using System;

namespace Soobak.Algo.Sorting {
  public enum SortOpType {
    Highlight,
    Compare,
    Swap,
    Insert,
    Pivot,
    Partition,
    Finalize
  }

  public sealed class SortOp {
    SortOp(SortOpType type, int primaryIndex, int secondaryIndex, SortingItem item, string message, DateTimeOffset timestamp) {
      Type = type;
      PrimaryIndex = primaryIndex;
      SecondaryIndex = secondaryIndex;
      Item = item;
      Message = message;
      Timestamp = timestamp;
    }

    public SortOpType Type { get; }

    public int PrimaryIndex { get; }

    public int SecondaryIndex { get; }

    public SortingItem Item { get; }

    public string Message { get; }

    public DateTimeOffset Timestamp { get; }

    public static SortOp Highlight(int index, string message) {
      ValidateIndex(index, nameof(index));
      return new SortOp(SortOpType.Highlight, index, -1, null, message, DateTimeOffset.UtcNow);
    }

    public static SortOp Compare(int leftIndex, int rightIndex) {
      ValidateIndex(leftIndex, nameof(leftIndex));
      ValidateIndex(rightIndex, nameof(rightIndex));
      return new SortOp(SortOpType.Compare, leftIndex, rightIndex, null, "Compare", DateTimeOffset.UtcNow);
    }

    public static SortOp Swap(int leftIndex, int rightIndex) {
      ValidateIndex(leftIndex, nameof(leftIndex));
      ValidateIndex(rightIndex, nameof(rightIndex));
      return new SortOp(SortOpType.Swap, leftIndex, rightIndex, null, "Swap", DateTimeOffset.UtcNow);
    }

    public static SortOp Insert(int sourceIndex, int targetIndex, SortingItem item) {
      ValidateIndex(sourceIndex, nameof(sourceIndex));
      ValidateIndex(targetIndex, nameof(targetIndex));
      if (item == null)
        throw new ArgumentNullException(nameof(item));
      return new SortOp(SortOpType.Insert, sourceIndex, targetIndex, item.Clone(), "Insert", DateTimeOffset.UtcNow);
    }

    public static SortOp Pivot(int index) {
      ValidateIndex(index, nameof(index));
      return new SortOp(SortOpType.Pivot, index, -1, null, "Pivot", DateTimeOffset.UtcNow);
    }

    public static SortOp Partition(int leftIndex, int rightIndex) {
      ValidateIndex(leftIndex, nameof(leftIndex));
      ValidateIndex(rightIndex, nameof(rightIndex));
      return new SortOp(SortOpType.Partition, leftIndex, rightIndex, null, "Partition", DateTimeOffset.UtcNow);
    }

    public static SortOp Finalize(string message) {
      return new SortOp(SortOpType.Finalize, -1, -1, null, message, DateTimeOffset.UtcNow);
    }

    static void ValidateIndex(int index, string name) {
      if (index < 0)
        throw new ArgumentOutOfRangeException(name, index, "Index must be non-negative.");
    }
  }
}
