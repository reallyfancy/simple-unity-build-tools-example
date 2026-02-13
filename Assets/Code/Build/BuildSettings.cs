using UnityEngine;

// An ScriptableObject that stores common build settings, and has a reference to the current BuildConfig preset.
public class BuildSettings : ScriptableObject
{
    [SerializeField] public BuildConfig _currentBuildConfig;
    public BuildConfig CurrentBuildConfig => _currentBuildConfig;
    
#if UNITY_EDITOR
    public static readonly string AssetPath = $"Assets/BuildSettings/{nameof(BuildSettings)}.asset";
#endif
}
