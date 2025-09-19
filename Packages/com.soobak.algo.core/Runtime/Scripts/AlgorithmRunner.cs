using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Soobak.Algo.Core {
  public sealed class AlgorithmRunner<TState, TEvent> {
    readonly IAlgorithm<TState, TEvent> _algorithm;
    readonly IAlgorithmVisualizer<TState, TEvent> _visualizer;
    readonly Func<TState, TState> _stateCloner;

    public AlgorithmRunner(IAlgorithm<TState, TEvent> algorithm, IAlgorithmVisualizer<TState, TEvent> visualizer, Func<TState, TState> stateCloner) {
      _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
      _visualizer = visualizer ?? throw new ArgumentNullException(nameof(visualizer));
      _stateCloner = stateCloner ?? throw new ArgumentNullException(nameof(stateCloner));
    }

    public async UniTask<TState> RunAsync(TState input, CancellationToken cancellationToken = default) {
      if (input == null)
        throw new ArgumentNullException(nameof(input));

      var working = _stateCloner(input);
      if (working == null)
        throw new InvalidOperationException("AlgorithmRunner: state clone returned null.");

      try {
        cancellationToken.ThrowIfCancellationRequested();
        await _visualizer.InitializeAsync(working, cancellationToken);

        await _algorithm.ExecuteAsync(
          working,
          async evt => {
            cancellationToken.ThrowIfCancellationRequested();
            await _visualizer.ApplyAsync(evt, working, cancellationToken);
          },
          cancellationToken);

        await _visualizer.CompleteAsync(working, cancellationToken);

        var snapshot = _stateCloner(working);
        if (snapshot == null)
          throw new InvalidOperationException("AlgorithmRunner: state clone returned null.");

        return snapshot;
      }
      catch (OperationCanceledException) {
        Debug.LogWarning($"AlgorithmRunner: Execution canceled for algorithm {_algorithm.Id}.");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"AlgorithmRunner: Execution failed for algorithm {_algorithm.Id}. {ex.Message}");
        throw;
      }
    }
  }
}
