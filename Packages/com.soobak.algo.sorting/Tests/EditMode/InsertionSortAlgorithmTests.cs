using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace Soobak.Algo.Sorting.Tests
{
    public class InsertionSortAlgorithmTests
    {
        [Test]
        public async Task ExecuteAsync_SortsAscendingAndPreservesStability()
        {
            var algorithm = new InsertionSortAlgorithm();
            var items = new List<SortItem>
            {
                new SortItem(3, "A"),
                new SortItem(1, "B"),
                new SortItem(2, "C"),
                new SortItem(2, "D"),
                new SortItem(1, "E")
            };

            var recordedTypes = new List<SortOpType>();

            await algorithm.ExecuteAsync(
                items,
                op =>
                {
                    recordedTypes.Add(op.Type);
                    return UniTask.CompletedTask;
                },
                CancellationToken.None);

            Assert.That(items.Select(i => i.Value), Is.EqualTo(new[] { 1, 1, 2, 2, 3 }));
            Assert.That(items.Where(i => i.Value == 2).Select(i => i.Label), Is.EqualTo(new[] { "C", "D" }));
            Assert.That(recordedTypes, Does.Contain(SortOpType.Insert));
        }
    }
}
