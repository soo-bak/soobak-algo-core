using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Soobak.Algo.Sorting {
  public interface IBarVisualizer {
    UniTask InitializeAsync(IReadOnlyList<SortItem> items, CancellationToken cancellationToken);

    UniTask ApplyAsync(SortOp op, IReadOnlyList<SortItem> items, CancellationToken cancellationToken);

    UniTask CompleteAsync(IReadOnlyList<SortItem> items, CancellationToken cancellationToken);
  }
}
