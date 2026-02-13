using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ExampleDataAsset")]
public class ExampleDataAsset : ScriptableObject, IValidatable
{
    public string id;
    public int damage;
    public string damageMessage;
        
    // This could be called at runtime, e.g. on loading to check that the object is valid
    // It can also be called in the Editor to show custom GUI
    // Extend it as needed whenever you add a field to this type
    public void Validate(out ValidationResult result, out List<string> errorMessages)
    {
        result = ValidationResult.Valid;
        errorMessages = new List<string>();

        if (string.IsNullOrEmpty(id))
        {
            result = ValidationResult.Invalid;
            errorMessages.Add("Missing ID");
        }

        if (damage > 0 && string.IsNullOrEmpty(damageMessage))
        {
            result = ValidationResult.Invalid;
            errorMessages.Add($"Does {damage} damage, but has no damage message");
        }
    }
}
    
#if UNITY_EDITOR
[CustomEditor(typeof(ExampleDataAsset))]
public class BuildConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Before drawing the regular inspector contents, draw a box that contains the validation results
        var targetAsset = (ExampleDataAsset)target;
        targetAsset.Validate(out var result, out var messages);

        var formattedMessages = string.Join("\n", messages);
            
        if (result != ValidationResult.Valid)
        {
            EditorGUILayout.HelpBox($"Data is invalid!\n{formattedMessages}", MessageType.Error);
        }
        else
        {
            EditorGUILayout.HelpBox($"Data is valid! Well done!", MessageType.Info);
        }
        
        // Now draw the regular inspector as usual
        DrawDefaultInspector();
    }
}
#endif