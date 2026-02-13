using UnityEngine;

// A runtime class that exposes build settings so that you can query its state at runtime to e.g. enable cheats
public class BuildSettingsManager : MonoBehaviour
{
    [SerializeField] private BuildSettings _buildSettings;
    public BuildSettings BuildSettings => _buildSettings;
}