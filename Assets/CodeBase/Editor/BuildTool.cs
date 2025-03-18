using System.Collections.Generic;
using UnityEditor;

public class BuildTool : Editor
{
    private const string BuildLocation = "/home/adept/Unity/Projects/DOTS_MobaTanks/Builds/linux";

    [MenuItem("Tools/Build/Linux")]
    public static void OnClick() {
        List<string> sceneList = new();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            if (scene.enabled)
                sceneList.Add(scene.path);

        if (sceneList.Count == 0) {
            UnityEngine.Debug.Log("Scene list is empty. Set scene list in build settings");
            return;
        }
        
        BuildPlayerOptions buildPlayerOptions = new()
        {
            scenes = sceneList.ToArray(),
            target = BuildTarget.StandaloneLinux64,
            locationPathName = BuildLocation
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}