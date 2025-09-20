using System;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;
using UnityEngine;

namespace Soobak.Algo.Sorting.Samples {
  [DisallowMultipleComponent]
  [RequireComponent(typeof(SortingBarVisualizer))]
  public sealed class SortingDemoController : MonoBehaviour {
    [SerializeField] string _algorithmId = "radix-sort";
    [SerializeField] bool _generateRandomValues = true;
    [SerializeField, Min(1)] int _arrayLength = 12;
    [SerializeField] Vector2Int _valueRange = new(1, 100);
    [SerializeField] int[] _values = Array.Empty<int>();

    SortingBarVisualizer _visualizer = null!;
    SortingRunner _runner = null!;
    SortingAlgorithmCatalog _catalog = null!;

    void Awake() {
      _visualizer = GetComponent<SortingBarVisualizer>();
      _catalog = new SortingAlgorithmCatalog();
      _runner = new SortingRunner(_visualizer, _catalog);
    }

    async void Start() {
      await RunAsync();
    }

    [ContextMenu("Run Demo")]
    public void RunFromInspector() {
      RunAsync().Forget();
    }

    async UniTask RunAsync() {
      var token = this.GetCancellationTokenOnDestroy();
      var data = BuildValues();
      if (!_catalog.TryGetDescriptor(_algorithmId, out _)) {
        Debug.LogWarning($"SortingDemoController: Algorithm '{_algorithmId}' not found. Falling back to counting-sort.");
        _algorithmId = "counting-sort";
      }

      try {
        var initial = SortingState.FromValues(data);
        await _runner.ExecuteAsync(_algorithmId, initial, token);
      }
      catch (OperationCanceledException) {
      }
      catch (Exception ex) {
        Debug.LogError($"SortingDemoController: Execution failed. {ex}");
      }
    }

    int[] BuildValues() {
      if (_generateRandomValues) {
        var random = new System.Random();
        var length = Mathf.Max(1, _arrayLength);
        var min = Mathf.Min(_valueRange.x, _valueRange.y);
        var max = Mathf.Max(_valueRange.x, _valueRange.y) + 1;
        var result = new int[length];
        for (var i = 0; i < length; i++)
          result[i] = random.Next(min, max);
        return result;
      }

      if (_values == null || _values.Length == 0)
        return new[] { 5, 3, 8, 1, 4, 7, 2, 6 };

      var copy = new int[_values.Length];
      Array.Copy(_values, copy, _values.Length);
      return copy;
    }
  }
}
