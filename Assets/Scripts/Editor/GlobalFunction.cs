using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Builder
{
    public static class GlobalFunction
    {
        public static void CommandLineTest()
        {
            Debug.Log("Hello CommandLineTest");

            string[] args = Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
            {
                Debug.Log($"[ {i} ] = {args[i]}");
                if (args[i] == "--myParam" && i + 1 < args.Length)
                {
                    string myParamValue = args[i + 1];
                    Debug.Log("myParam value: " + myParamValue);
                }
            }
        }

        public static void CopyFile()
        {
            EnvironmentArgs environmentArgs = new EnvironmentArgs();
            environmentArgs.TryGetCommandArgs();
            Debug.Log($"sourceFilePath = {environmentArgs.sourceFilePath}");
            Debug.Log($"desFilePath = {environmentArgs.desFilePath}");
            if(string.IsNullOrEmpty(environmentArgs.sourceFilePath) || string.IsNullOrEmpty(environmentArgs.desFilePath))
            {
                Debug.Log("Path is Empty");
                return;
            }
            System.IO.File.Copy(environmentArgs.sourceFilePath, environmentArgs.desFilePath);
        }
        public static void Build()
        {
            string locationPathName = "D:/UnityProject/ListTest/Build/AAA.exe";
            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, locationPathName, buildTarget, BuildOptions.None);
            Test();
        }

        public static void Test()
        {
            string path = "D:/test";

            if (!System.IO.File.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
    }

    public class EnvironmentArgs
    {
        public string sourceFilePath = string.Empty;
        public string desFilePath = string.Empty;

        public void TryGetCommandArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            foreach (var arg in args)
            {
                if (arg.StartsWith("-SourceFilePath"))
                {
                    GetValue(arg, ref sourceFilePath);
                }

                if (arg.StartsWith("-DesFilePath"))
                {
                    GetValue(arg, ref desFilePath);
                }
            }
        }

        private void GetValue(string cmd, ref string parameter)
        {
            int startIndex = cmd.IndexOf('=') + 1;
            if (!(startIndex < 0 || startIndex > cmd.Length))
            {
                parameter = cmd.Substring(startIndex);
            }
        }
    }
}

