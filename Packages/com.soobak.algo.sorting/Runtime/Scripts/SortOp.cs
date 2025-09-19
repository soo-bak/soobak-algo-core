using System;

namespace Soobak.Algo.Sorting {
  public sealed class SortOp {
    SortOp(SortOpType type, int primaryIndex, int secondaryIndex, SortItem? item) {
      Type = type;
      PrimaryIndex = primaryIndex;
      SecondaryIndex = secondaryIndex;
      Item = item;
    }

    public SortOpType Type { get; }

    public int PrimaryIndex { get; }

    public int SecondaryIndex { get; }

    public SortItem? Item { get; }

    public static SortOp Highlight(int index) {
      ValidateIndex(index, nameof(index));
      return new SortOp(SortOpType.Highlight, index, -1, null);
    }

    public static SortOp Compare(int leftIndex, int rightIndex) {
      ValidateIndex(leftIndex, nameof(leftIndex));
      ValidateIndex(rightIndex, nameof(rightIndex));
      return new SortOp(SortOpType.Compare, leftIndex, rightIndex, null);
    }

    public static SortOp Shift(int fromIndex, int toIndex, SortItem item) {
      ValidateIndex(fromIndex, nameof(fromIndex));
      ValidateIndex(toIndex, nameof(toIndex));
      return new SortOp(SortOpType.Shift, fromIndex, toIndex, item);
    }

    public static SortOp Insert(int index, SortItem item) {
      ValidateIndex(index, nameof(index));
      return new SortOp(SortOpType.Insert, index, -1, item);
    }

    static void ValidateIndex(int value, string name) {
      if (value < 0)
        throw new ArgumentOutOfRangeException(name, value, "Index must be non-negative");
    }
  }
}
