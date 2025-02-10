using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] bool showDebugConsoleOnBuild;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        VolumeController.VolumeToggled += VolumeToggledHandler;

        Debug.developerConsoleVisible = true;
    }

    private void OnDestroy()
    {
        VolumeController.VolumeToggled -= VolumeToggledHandler;
    }

    private void VolumeToggledHandler(bool value)
    {
        AudioListener.volume = value ? 1 : 0;
    }
}
