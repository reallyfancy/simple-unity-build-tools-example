#if UNITY_EDITOR

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Editor script that performs build operations
[InitializeOnLoad]
public static class Builder
{
    // InitializeOnLoad means that a fresh instance is created on domain reload
    static Builder()
    {
        if (!Application.isBatchMode)
        {
            // Register to perform builds in the Unity Editor
            BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
        }
    }

    // This function runs instead of Unity's regular build command
    // In this example, we intercept the call and show a dialogue
    private static void BuildPlayerHandler(BuildPlayerOptions buildOptions)
    {
        EditorUtility.DisplayDialog(
            "Don't use this option!",
            $"Use the custom menu scripts to build instead.",
            "OK"
        );
        
        // If you wanted to just build as normal, pass the object on like this
        // BuildPipeline.BuildPlayer(buildOptions);
    }

    [MenuItem("ExampleProjectName/Build")]
    public static void CustomBuildMenuItem()
    {
        // Load the BuildSettings
        var buildSettings = AssetDatabase.LoadAssetAtPath<BuildSettings>(BuildSettings.AssetPath);
        if (buildSettings == null)
        {
            EditorUtility.DisplayDialog(
                "Missing build settings!",
                $"Can't find build settings at {BuildSettings.AssetPath}.",
                "OK");
            return;
        }

        DoBuild(buildSettings);
    }
    
    private static void DoBuild(BuildSettings buildSettings)
    {
        // Check that it has a BuildConfig, and that the BuildConfig validates
        var currentBuildConfig = buildSettings._currentBuildConfig;
        if (currentBuildConfig == null)
        {
            EditorUtility.DisplayDialog(
                "Missing build config!",
                $"BuildSettings has no BuildConfig.",
                "OK");
            return;
        }
        
        currentBuildConfig.Validate(out var result, out var messages);
        if (result != ValidationResult.Valid)
        {
            Debug.LogError($"BuildConfig is invalid!\n{string.Join("\n", messages)}");
            
            EditorUtility.DisplayDialog(
                "Invalid build configuration!",
                "See log for details.",
                "OK");
            
            return;
        }

        var isDevelopmentBuild = buildSettings._currentBuildConfig.IsDevelopmentBuild;
        var buildTarget = buildSettings._currentBuildConfig.BuildTarget;
        var scenesEnabledInBuild = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(s => s.path)
            .ToArray();
        var buildPathAbsolute = Path.GetFullPath(Path.Combine(Application.dataPath, "../", buildSettings._currentBuildConfig.BuildPath));
        var buildOptions = isDevelopmentBuild ? BuildOptions.Development : BuildOptions.None;

        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenesEnabledInBuild,
            locationPathName = buildPathAbsolute,
            target = buildTarget,
            options = buildOptions
        };
        
        // Add any other prebuild steps here - e.g. here we check if some example data is valid
        var isDataValid = TryValidateData();
        
        if (!isDataValid)
        {
            // If data is invalid, give the user the choice to quit or continue
            // (Always provide the choice, so that false positives don't block a build!)
            var shouldContinueWithBuild = EditorUtility.DisplayDialog(
                "Found invalid assets!",
                $"See log for details.",
                "Continue with build anyway",
                "Cancel build"
            );
            if (!shouldContinueWithBuild)
            {
                return;
            }
        }

        var didUserConfirm = EditorUtility.DisplayDialog(
            "Build with current settings?",
            $"Current config: {buildSettings._currentBuildConfig.Id}\nCurrent build path: {buildPathAbsolute}",
            "OK",
            "Cancel"
        );
        if (!didUserConfirm)
        {
            return;
        }
        
        var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        var success = buildReport.summary.result == BuildResult.Succeeded;
        if (success)
        {
            Debug.Log($"Build succeeded!");
        }
        else
        {
            Debug.LogError("Build failed!");
        }
    }
    
    [MenuItem("ExampleProjectName/Validate data")]
    public static void ValidateDataMenuItem()
    {
        TryValidateData();
    }

    private static bool TryValidateData()
    {
        var areAllAssetsValid = true;
        
        // Find all assets of a given type
        var allExampleDataAssetGuids = AssetDatabase.FindAssets("t:ExampleDataAsset");
        foreach (var guid in allExampleDataAssetGuids)
        {
            // Get the asset path, and load the asset
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var currentAsset = AssetDatabase.LoadAssetAtPath<ExampleDataAsset>(assetPath);
            
            // If it fails to load, log this
            if (currentAsset == null)
            {
                areAllAssetsValid = false;
                Debug.LogError($"Couldn't load {assetPath}!");
                continue;
            }
            
            // If it fails to validate, log this
            currentAsset.Validate(out var result, out var messages);
            if (result != ValidationResult.Valid)
            {
                areAllAssetsValid = false;
                Debug.LogError($"{assetPath} has validation errors:\n{string.Join("\n", messages)}");    
            }
        }

        return areAllAssetsValid;
    }
}
#endif