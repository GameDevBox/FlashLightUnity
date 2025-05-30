/*
 * BoxRaycastSampler.cs
 * Created by Arian - GameDevBox
 * YouTube Channel: https://www.youtube.com/@GameDevBox
 *
 * 🎮 Want more Unity tips, tools, and advanced systems?
 * 🧠 Learn from practical examples and well-explained logic.
 * 📦 Subscribe to GameDevBox for more game dev content!
 *
 * This script performs a BoxCast from the camera and visualizes the cast 
 * in the Scene and Game view using Debug.DrawLine and Gizmos.
 */

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdvancedFlashlight))]
public class AdvancedFlashlightEditor : Editor
{
    private float energyToAdd = 10f;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty handleInputInternallyProp = serializedObject.FindProperty("handleInputInternally");
        SerializedProperty keyPressProp = serializedObject.FindProperty("KeyPress");

        SerializedProperty prop = serializedObject.GetIterator();
        bool enterChildren = true;

        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;

            // Draw handleInputInternally normally
            if (prop.name == "handleInputInternally")
            {
                EditorGUILayout.PropertyField(prop);
                continue;
            }

            // Draw KeyPress only if handleInputInternally is true
            if (prop.name == "KeyPress")
            {
                if (handleInputInternallyProp.boolValue)
                {
                    EditorGUILayout.PropertyField(prop);
                }
                continue;
            }

            // Draw all other properties normally
            EditorGUILayout.PropertyField(prop, true);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Energy Status", EditorStyles.boldLabel);

        AdvancedFlashlight flashlight = (AdvancedFlashlight)target;
        float energyPercent = flashlight.GetEnergyPercentage();
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, energyPercent, $"Energy: {(energyPercent * 100):F1}%");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);

        energyToAdd = EditorGUILayout.Slider("Energy to Add", energyToAdd, 1f, flashlight.GetMaxEnergy());

        if (GUILayout.Button("Add Energy"))
        {
            flashlight.AddEnergy(energyToAdd);
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(flashlight);
        }
    }
}
