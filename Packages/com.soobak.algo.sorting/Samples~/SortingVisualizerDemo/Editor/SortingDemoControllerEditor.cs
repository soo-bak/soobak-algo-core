using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Soobak.Algo.Sorting.Samples.Editor {
  [CustomEditor(typeof(SortingDemoController))]
  public sealed class SortingDemoControllerEditor : UnityEditor.Editor {
    string[] _algorithmIds = Array.Empty<string>();

    void OnEnable() {
      try {
        var catalog = new SortingAlgorithmCatalog();
        _algorithmIds = catalog.Descriptors.Select(d => d.Id).ToArray();
        if (_algorithmIds.Length == 0)
          _algorithmIds = new[] { "counting-sort" };
      }
      catch (Exception ex) {
        Debug.LogWarning($"SortingDemoControllerEditor: Failed to read catalog. {ex.Message}");
        _algorithmIds = new[] { "counting-sort" };
      }
    }

    public override void OnInspectorGUI() {
      serializedObject.Update();

      DrawAlgorithmIdPopup();
      EditorGUILayout.PropertyField(serializedObject.FindProperty("_generateRandomValues"));
      using (new EditorGUI.DisabledScope(serializedObject.FindProperty("_generateRandomValues").boolValue == false)) {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_arrayLength"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_valueRange"));
      }
      if (!serializedObject.FindProperty("_generateRandomValues").boolValue)
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_values"), includeChildren: true);

      serializedObject.ApplyModifiedProperties();

      if (GUILayout.Button("Run Demo")) {
        var controller = (SortingDemoController)target;
        if (Application.isPlaying)
          controller.RunFromInspector();
        else
          Debug.LogWarning("SortingDemoController: Enter Play Mode to run the demo.");
      }
    }

    void DrawAlgorithmIdPopup() {
      var property = serializedObject.FindProperty("_algorithmId");
      var currentIndex = Array.IndexOf(_algorithmIds, property.stringValue);
      if (currentIndex < 0)
        currentIndex = 0;

      EditorGUI.BeginChangeCheck();
      var newIndex = EditorGUILayout.Popup("Algorithm Id", currentIndex, _algorithmIds);
      if (EditorGUI.EndChangeCheck())
        property.stringValue = _algorithmIds[Mathf.Clamp(newIndex, 0, _algorithmIds.Length - 1)];
    }
  }
}
