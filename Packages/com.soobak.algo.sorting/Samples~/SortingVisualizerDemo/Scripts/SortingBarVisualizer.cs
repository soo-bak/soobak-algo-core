using System;
using System.Collections.Generic;
using Soobak.Algo.Core;
using Soobak.Algo.Sorting;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Soobak.Algo.Sorting.Samples {
  [DisallowMultipleComponent]
  public sealed class SortingBarVisualizer : MonoBehaviour, IBarVisualizer {
    [SerializeField] float _barSpacing = 1f;
    [SerializeField] float _barWidth = 0.8f;
    [SerializeField] float _heightScale = 0.1f;
    [SerializeField] Color _baseColor = new(0.2f, 0.2f, 0.2f);
    [SerializeField] Color _compareColor = new(0.95f, 0.7f, 0.2f);
    [SerializeField] Color _swapColor = new(0.95f, 0.3f, 0.3f);
    [SerializeField] Color _insertColor = new(0.3f, 0.6f, 0.95f);
    [SerializeField] Color _completedColor = new(0.3f, 0.8f, 0.3f);
    [SerializeField, Range(0f, 0.5f)] float _stepDelaySeconds = 0.05f;

    readonly List<Bar> _bars = new();
    readonly List<Material> _materials = new();

    struct Bar {
      public Transform Transform;
      public Renderer Renderer;
    }

    public async UniTask InitializeAsync(SortingState initialState, CancellationToken cancellationToken) {
      await UniTask.SwitchToMainThread(cancellationToken);
      ClearBars();
      AllocateBars(initialState.Items.Count);
      ApplySnapshot(initialState);
      ResetColors();
    }

    public async UniTask PublishAsync(AlgorithmStep<SortingState, SortOp> step, CancellationToken cancellationToken) {
      await UniTask.SwitchToMainThread(cancellationToken);
      EnsureBarCount(step.Snapshot.Items.Count);
      ApplySnapshot(step.Snapshot);
      ApplyHighlight(step.Event);
      if (_stepDelaySeconds > 0f)
        await UniTask.Delay(TimeSpan.FromSeconds(_stepDelaySeconds), cancellationToken: cancellationToken);
    }

    public async UniTask CompleteAsync(SortingState finalState, CancellationToken cancellationToken) {
      await UniTask.SwitchToMainThread(cancellationToken);
      ApplySnapshot(finalState);
      SetAllColors(_completedColor);
    }

    void EnsureBarCount(int requiredCount) {
      if (_bars.Count == requiredCount)
        return;
      ClearBars();
      AllocateBars(requiredCount);
    }

    void AllocateBars(int count) {
      for (var i = 0; i < count; i++) {
        var barObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        barObject.name = $"Bar_{i:D2}";
        barObject.transform.SetParent(transform, false);
        Destroy(barObject.GetComponent<Collider>());
        var renderer = barObject.GetComponent<Renderer>();
        _bars.Add(new Bar { Transform = barObject.transform, Renderer = renderer });
        _materials.Add(renderer.material);
      }
      PositionBars();
    }

    void PositionBars() {
      for (var i = 0; i < _bars.Count; i++) {
        var bar = _bars[i];
        var position = new Vector3(i * _barSpacing, 0f, 0f);
        bar.Transform.localPosition = position;
        bar.Transform.localScale = new Vector3(_barWidth, 1f, _barWidth);
      }
    }

    void ApplySnapshot(SortingState state) {
      for (var i = 0; i < state.Items.Count && i < _bars.Count; i++) {
        var bar = _bars[i];
        var height = Mathf.Max(0.1f, state.Items[i].Value * _heightScale);
        var scale = bar.Transform.localScale;
        scale.y = height;
        bar.Transform.localScale = scale;
        var position = bar.Transform.localPosition;
        position.y = height * 0.5f;
        bar.Transform.localPosition = position;
      }
    }

    void ApplyHighlight(SortOp op) {
      ResetColors();
      switch (op.Type) {
        case SortOpType.Highlight:
          SetColor(op.PrimaryIndex, _compareColor);
          break;
        case SortOpType.Compare:
          SetColor(op.PrimaryIndex, _compareColor);
          SetColor(op.SecondaryIndex, _compareColor);
          break;
        case SortOpType.Swap:
          SetColor(op.PrimaryIndex, _swapColor);
          SetColor(op.SecondaryIndex, _swapColor);
          break;
        case SortOpType.Insert:
          SetColor(op.PrimaryIndex, _insertColor);
          SetColor(op.SecondaryIndex, _insertColor);
          break;
        case SortOpType.Pivot:
          SetColor(op.PrimaryIndex, _compareColor);
          break;
        case SortOpType.Partition:
          SetColor(op.PrimaryIndex, _compareColor);
          SetColor(op.SecondaryIndex, _compareColor);
          break;
        case SortOpType.Finalize:
          SetAllColors(_completedColor);
          break;
      }
    }

    void ResetColors() {
      SetAllColors(_baseColor);
    }

    void SetAllColors(Color color) {
      for (var i = 0; i < _materials.Count; i++) {
        if (_materials[i] != null)
          _materials[i].color = color;
      }
    }

    void SetColor(int index, Color color) {
      if (index < 0 || index >= _materials.Count)
        return;
      if (_materials[index] != null)
        _materials[index].color = color;
    }

    void ClearBars() {
      for (var i = 0; i < _bars.Count; i++) {
        if (_bars[i].Transform != null)
          Destroy(_bars[i].Transform.gameObject);
        if (_materials.Count > i && _materials[i] != null)
          Destroy(_materials[i]);
      }
      _bars.Clear();
      _materials.Clear();
    }

    void OnDestroy() {
      ClearBars();
    }
  }
}
