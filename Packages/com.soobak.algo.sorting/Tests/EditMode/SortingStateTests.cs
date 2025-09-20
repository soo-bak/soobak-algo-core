using System;
using System.Linq;
using NUnit.Framework;
using Soobak.Algo.Sorting;

namespace Soobak.Algo.Sorting.Tests {
  public class SortingStateTests {
    [Test]
    public void Constructor_NullItems_ThrowsArgumentNullException() {
      Assert.Throws<ArgumentNullException>(() => new SortingState(null!));
    }

    [Test]
    public void Constructor_ClonesItems() {
      var original = new SortingItem(3, "origin");
      var state = new SortingState(new[] { original });

      Assert.That(state.Items[0], Is.Not.SameAs(original));
      Assert.That(state.Items[0].Value, Is.EqualTo(original.Value));
    }

    [Test]
    public void Clone_ReturnsDeepCopy() {
      var state = SortingState.FromValues(new[] { 2, 1 });
      var clone = state.Clone();

      Assert.That(clone, Is.Not.SameAs(state));
      Assert.That(clone.Items.Select(item => item.Value), Is.EqualTo(state.Items.Select(item => item.Value)));
      Assert.That(clone.Items[0], Is.Not.SameAs(state.Items[0]));
    }

    [Test]
    public void Replace_InvalidIndex_ThrowsArgumentOutOfRangeException() {
      var state = SortingState.FromValues(new[] { 1 });

      Assert.Throws<ArgumentOutOfRangeException>(() => state.Replace(-1, new SortingItem(2, "a")));
      Assert.Throws<ArgumentOutOfRangeException>(() => state.Replace(1, new SortingItem(2, "a")));
    }

    [Test]
    public void Replace_NullItem_ThrowsArgumentNullException() {
      var state = SortingState.FromValues(new[] { 1 });

      Assert.Throws<ArgumentNullException>(() => state.Replace(0, null!));
    }

    [Test]
    public void Replace_UpdatesSlot() {
      var state = SortingState.FromValues(new[] { 1, 3 });
      var replacement = new SortingItem(2, "new");

      state.Replace(1, replacement);

      Assert.That(state.Items[1].Value, Is.EqualTo(2));
      Assert.That(state.Items[1], Is.SameAs(replacement));
    }

    [Test]
    public void Move_InvalidIndices_ThrowArgumentOutOfRangeException() {
      var state = SortingState.FromValues(new[] { 1, 2 });

      Assert.Throws<ArgumentOutOfRangeException>(() => state.Move(-1, 1));
      Assert.Throws<ArgumentOutOfRangeException>(() => state.Move(2, 1));
      Assert.Throws<ArgumentOutOfRangeException>(() => state.Move(0, -1));
      Assert.Throws<ArgumentOutOfRangeException>(() => state.Move(0, 3));
    }

    [Test]
    public void Move_ReordersItems() {
      var state = SortingState.FromValues(new[] { 1, 2, 3 });

      state.Move(2, 0);

      Assert.That(state.Items.Select(item => item.Value), Is.EqualTo(new[] { 3, 1, 2 }));
    }

    [Test]
    public void Swap_InvalidIndices_ThrowArgumentOutOfRangeException() {
      var state = SortingState.FromValues(new[] { 1, 2 });

      Assert.Throws<ArgumentOutOfRangeException>(() => state.Swap(-1, 0));
      Assert.Throws<ArgumentOutOfRangeException>(() => state.Swap(0, -1));
      Assert.Throws<ArgumentOutOfRangeException>(() => state.Swap(0, 2));
      Assert.Throws<ArgumentOutOfRangeException>(() => state.Swap(2, 0));
    }

    [Test]
    public void Swap_ExchangesValues() {
      var state = SortingState.FromValues(new[] { 4, 1, 3 });

      state.Swap(0, 2);

      Assert.That(state.Items.Select(item => item.Value), Is.EqualTo(new[] { 3, 1, 4 }));
    }
  }

  public class SortOpTests {
    [Test]
    public void Highlight_NegativeIndex_ThrowsArgumentOutOfRangeException() {
      Assert.Throws<ArgumentOutOfRangeException>(() => SortOp.Highlight(-1, "negative"));
    }

    [Test]
    public void Compare_NegativeIndex_ThrowsArgumentOutOfRangeException() {
      Assert.Throws<ArgumentOutOfRangeException>(() => SortOp.Compare(-1, 0));
      Assert.Throws<ArgumentOutOfRangeException>(() => SortOp.Compare(0, -1));
    }

    [Test]
    public void Insert_NullItem_ThrowsArgumentNullException() {
      Assert.Throws<ArgumentNullException>(() => SortOp.Insert(0, 0, null!));
    }

    [Test]
    public void Insert_NegativeIndices_ThrowArgumentOutOfRangeException() {
      var item = new SortingItem(1, "id");

      Assert.Throws<ArgumentOutOfRangeException>(() => SortOp.Insert(-1, 0, item));
      Assert.Throws<ArgumentOutOfRangeException>(() => SortOp.Insert(0, -1, item));
    }

    [Test]
    public void Insert_ClonesSortingItem() {
      var item = new SortingItem(5, "stable");

      var op = SortOp.Insert(1, 0, item);

      Assert.That(op.Item, Is.Not.Null);
      Assert.That(op.Item, Is.Not.SameAs(item));
      Assert.That(op.Item.Value, Is.EqualTo(item.Value));
      Assert.That(op.Item.StableId, Is.EqualTo(item.StableId));
    }

    [Test]
    public void Swap_UsesProvidedIndices() {
      var op = SortOp.Swap(1, 2);

      Assert.That(op.PrimaryIndex, Is.EqualTo(1));
      Assert.That(op.SecondaryIndex, Is.EqualTo(2));
      Assert.That(op.Type, Is.EqualTo(SortOpType.Swap));
    }
  }
}
