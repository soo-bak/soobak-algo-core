using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace Soobak.Algo.Sorting.Tests {
  public class InsertionSortAlgorithmTests {
    [Test]
    public async Task ExecuteAsync_SortsAscendingAndPreservesStability() {
      var algorithm = new InsertionSortAlgorithm();
      var items = new List<SortItem> {
        new SortItem(3, "A"),
        new SortItem(1, "B"),
        new SortItem(2, "C"),
        new SortItem(2, "D"),
        new SortItem(1, "E")
      };

      var types = new List<SortOpType>();

      await algorithm.ExecuteAsync(items, op => {
        types.Add(op.Type);
        return UniTask.CompletedTask;
      }, CancellationToken.None);

      Assert.That(items.Select(i => i.Value), Is.EqualTo(new[] { 1, 1, 2, 2, 3 }));
      Assert.That(items.Where(i => i.Value == 2).Select(i => i.Label), Is.EqualTo(new[] { "C", "D" }));
      Assert.That(types, Does.Contain(SortOpType.Insert));
    }

    [Test]
    public async Task ExecuteAsync_SortsDescendingSequence() {
      var algorithm = new InsertionSortAlgorithm();
      var items = new List<SortItem> {
        new SortItem(9, "A"),
        new SortItem(5, "B"),
        new SortItem(1, "C"),
        new SortItem(7, "D"),
        new SortItem(3, "E")
      };

      var types = new List<SortOpType>();

      await algorithm.ExecuteAsync(items, op => {
        types.Add(op.Type);
        return UniTask.CompletedTask;
      }, CancellationToken.None);

      Assert.That(items.Select(i => i.Value), Is.EqualTo(new[] { 1, 3, 5, 7, 9 }));
      Assert.That(types.Count(t => t == SortOpType.Shift), Is.GreaterThan(0));
      Assert.That(types.Count(t => t == SortOpType.Compare), Is.GreaterThan(0));
    }

    [Test]
    public async Task ExecuteAsync_AllowsEmptySequence() {
      var algorithm = new InsertionSortAlgorithm();
      var items = new List<SortItem>();
      var count = 0;

      await algorithm.ExecuteAsync(items, _ => {
        count++;
        return UniTask.CompletedTask;
      }, CancellationToken.None);

      Assert.That(items, Is.Empty);
      Assert.That(count, Is.EqualTo(0));
    }
  }
}
