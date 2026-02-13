using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ScriptableObject that stores a preset of build-related settings
[CreateAssetMenu(menuName = "ScriptableObjects/BuildConfig")]
public class BuildConfig : ScriptableObject, IValidatable
{
    // These settings can be used at either runtime or build time
    [SerializeField] public string _id;
    public string Id => _id;
    
    [SerializeField] public BuildStorefront _storefront;
    public BuildStorefront Storefront => _storefront;
    
    [SerializeField] public bool _areCheatsEnabled;
    public bool AreCheatsEnabled => _areCheatsEnabled;
    
    // These settings are build time only
#if UNITY_EDITOR
    [SerializeField] public BuildTarget _buildTarget = BuildTarget.StandaloneWindows64;
    public BuildTarget BuildTarget => _buildTarget;
    
    [SerializeField] public string _buildPath = "build/MyDemo.exe";
    public string BuildPath => _buildPath;
    
    [SerializeField] public bool _isDevelopmentBuild;
    public bool IsDevelopmentBuild => _isDevelopmentBuild;
#endif
    
    public void Validate(out ValidationResult result, out List<string> errorMessages)
    {
        result = ValidationResult.Valid;
        errorMessages = new List<string>();

        if (string.IsNullOrEmpty(_id))
        {
            result = ValidationResult.Invalid;
            errorMessages.Add("Missing ID");
        }
        
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(_buildPath))
        {
            result = ValidationResult.Invalid;
            errorMessages.Add("Missing build path");
        }
        else
        {
            if (!_buildPath.StartsWith("build/"))
            {
                result = ValidationResult.Invalid;
                errorMessages.Add("Build path must start with 'build/'");
            }
            
            var buildFileName = Path.GetFileName(_buildPath);
            if (_buildTarget == BuildTarget.StandaloneWindows64 && !buildFileName.EndsWith(".exe"))
            {
                result = ValidationResult.Invalid;
                errorMessages.Add("Invalid file extension in build path");
            }
        }
        
        if (_buildTarget == BuildTarget.NoTarget)
        {
            result = ValidationResult.Invalid;
            errorMessages.Add("Invalid build target");
        }
#endif
    }
}
