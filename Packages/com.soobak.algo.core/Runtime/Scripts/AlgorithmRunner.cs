using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Soobak.Algo.Core {
  public sealed class AlgorithmRunner<TState, TEvent> : IAlgorithmStepSink<TState, TEvent> {
    readonly Func<TState, TState> _stateCloner;
    readonly IReadOnlyList<IAlgorithmStepSink<TState, TEvent>> _sinks;

    public AlgorithmRunner(Func<TState, TState> stateCloner, IReadOnlyList<IAlgorithmStepSink<TState, TEvent>> sinks) {
      _stateCloner = stateCloner ?? throw new ArgumentNullException(nameof(stateCloner));
      if (sinks == null)
        throw new ArgumentNullException(nameof(sinks));

      if (sinks.Count == 0) {
        _sinks = Array.Empty<IAlgorithmStepSink<TState, TEvent>>();
      }
      else {
        var copy = new IAlgorithmStepSink<TState, TEvent>[sinks.Count];
        for (var i = 0; i < sinks.Count; i++) {
          var sink = sinks[i];
          if (sink == null)
            throw new ArgumentNullException($"sinks[{i}]");
          copy[i] = sink;
        }

        _sinks = Array.AsReadOnly(copy);
      }
    }

    public AlgorithmRunner(Func<TState, TState> stateCloner, params IAlgorithmStepSink<TState, TEvent>[] sinks)
      : this(stateCloner, (IReadOnlyList<IAlgorithmStepSink<TState, TEvent>>)(sinks ?? Array.Empty<IAlgorithmStepSink<TState, TEvent>>())) {
    }

    public async UniTask<TState> ExecuteAsync(IAlgorithm<TState, TEvent> algorithm, TState input, CancellationToken cancellationToken) {
      if (algorithm == null)
        throw new ArgumentNullException(nameof(algorithm));

      if (input == null)
        throw new ArgumentNullException(nameof(input));

      var working = _stateCloner(input);
      if (working == null)
        throw new InvalidOperationException("AlgorithmRunner: state clone returned null.");

      await BroadcastInitializeAsync(working, cancellationToken);

      try {
        await algorithm.ExecuteAsync(working, this, cancellationToken);
      }
      catch (OperationCanceledException ex) {
        Debug.LogWarning($"AlgorithmRunner: Execution cancelled for algorithm {algorithm.Id}. {ex.Message}");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"AlgorithmRunner: Execution failed for algorithm {algorithm.Id}. {ex}");
        throw;
      }

      await BroadcastCompleteAsync(working, cancellationToken);

      var snapshot = _stateCloner(working);
      if (snapshot == null)
        throw new InvalidOperationException("AlgorithmRunner: state clone returned null.");

      return snapshot;
    }

    public async UniTask InitializeAsync(TState initialState, CancellationToken cancellationToken) {
      await BroadcastInitializeAsync(initialState, cancellationToken);
    }

    public async UniTask PublishAsync(AlgorithmStep<TState, TEvent> step, CancellationToken cancellationToken) {
      foreach (var sink in _sinks) {
        cancellationToken.ThrowIfCancellationRequested();
        var snapshot = _stateCloner(step.Snapshot);
        if (snapshot == null)
          throw new InvalidOperationException("AlgorithmRunner: state clone returned null.");
        var forwarded = new AlgorithmStep<TState, TEvent>(snapshot, step.Event, step.Message, step.Timestamp);
        try {
          await sink.PublishAsync(forwarded, cancellationToken);
        }
        catch (OperationCanceledException) {
          throw;
        }
        catch (Exception ex) {
          Debug.LogError($"AlgorithmRunner: Publish failed. {ex}");
          throw;
        }
      }
    }

    public async UniTask CompleteAsync(TState finalState, CancellationToken cancellationToken) {
      await BroadcastCompleteAsync(finalState, cancellationToken);
    }

    async UniTask BroadcastInitializeAsync(TState state, CancellationToken cancellationToken) {
      foreach (var sink in _sinks) {
        cancellationToken.ThrowIfCancellationRequested();
        var snapshot = _stateCloner(state);
        if (snapshot == null)
          throw new InvalidOperationException("AlgorithmRunner: state clone returned null.");
        try {
          await sink.InitializeAsync(snapshot, cancellationToken);
        }
        catch (OperationCanceledException) {
          throw;
        }
        catch (Exception ex) {
          Debug.LogError($"AlgorithmRunner: Initialize failed. {ex}");
          throw;
        }
      }
    }

    async UniTask BroadcastCompleteAsync(TState state, CancellationToken cancellationToken) {
      foreach (var sink in _sinks) {
        cancellationToken.ThrowIfCancellationRequested();
        var snapshot = _stateCloner(state);
        if (snapshot == null)
          throw new InvalidOperationException("AlgorithmRunner: state clone returned null.");
        try {
          await sink.CompleteAsync(snapshot, cancellationToken);
        }
        catch (OperationCanceledException) {
          throw;
        }
        catch (Exception ex) {
          Debug.LogError($"AlgorithmRunner: Complete failed. {ex}");
          throw;
        }
      }
    }
  }
}
