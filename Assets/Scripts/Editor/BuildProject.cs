using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildProject
{
    [MenuItem("測試功能/包版")]
    public static void BuildFunc()
    {
        string[] scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        string outputPath = "D:/BuildTest2/AAABBB.exe";

        BuildOptions buildOptions = BuildOptions.None;
        BuildReport report = BuildPipeline.BuildPlayer(scenes, outputPath, BuildTarget.StandaloneWindows64, buildOptions);

        // 5. 驗證打包結果
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + report.summary.outputPath);
        }
        else
        {
            Debug.LogError("Build failed: " + report.summary.result);
        }
    }
}
