using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace UnityGRPC.Editor
{
    public class UnityGrpcTools
    {
        [MenuItem("UnityGRPC/Compile Protos in Asset Folder")]
        public static void CompileProtofiles()
        {
            SetupAndCompile(toolsPath);
        }

        [MenuItem("UnityGRPC/Compile Protos in Folder")]
        public static void CompileProtofilesInFolder()
        {
            string path = EditorUtility.OpenFolderPanel("Choose Folder to build protos in", "", "");
            path = path + "/GrpcTools~";
            SetupAndCompile(path);
        }

        [MenuItem("Assets/CompileProtos")]
        public static void CompileProtos()
        {
            var path = "";
            var obj = Selection.activeObject;
            if (obj == null) path = "Assets";
            else path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            SetupAndCompile(path);
        }

        [MenuItem("Assets/CompileProtos", true)]
        public static bool IsFoliderValidation()
        {
            var path = "";
            var obj = Selection.activeObject;
            if (obj == null) path = "Assets";
            else path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            if (path.Length > 0)
            {
                if (Directory.Exists(path))
                {
                    return true;
                }
            }
            return false;
        }

        public static void SetupAndCompile(string path)
        {
            Debug.Log("Started Compiling in " + path);
            SetupToolsInPath(path);
            try
            {
                RunDotnetProcess("restore", path);
                RunDotnetProcess("build", path);
                Debug.Log("Finished Compiling");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                CleanUp(path);
            }
        }

        private static void CleanUp(string path)
        {
            Directory.Delete(path,true);
        }
        private static void RunDotnetProcess(string argument, string workdingDir)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.WorkingDirectory = workdingDir;
            psi.FileName = "dotnet";
            psi.Arguments = argument;
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;

            using (var process = Process.Start(psi))
            {
                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                if (output.Length > 0)
                {
                    Debug.Log(output);
                }
                
                if(error.Length > 0)
                {
                    Debug.LogError(error);
                }
                if (process.ExitCode != 0)
                {
                    throw new Exception("dotnet " + argument + " has failed");
                }
            }
        }

        private static void SetupToolsInPath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            WriteFile(path+csprojFilePath, csprojFileContent);
        }

        private static void WriteFile(string path, string content)
        {
            if (File.Exists(path))
                File.Delete(path);
            using (var writer = File.CreateText(path))
            {
                writer.Write(content);
            }
        }
        
        private static string toolsPath = Application.dataPath + "/GrpcTools~";
        private static string csprojFilePath = "/.protos.csproj";
        private const string csprojFileContent = 
            @"
        <Project Sdk=""Microsoft.NET.Sdk\"">
        <PropertyGroup>
        <TargetFramework>netstandard2.0</TargetFramework>
        </PropertyGroup>

        <ItemGroup>
        <PackageReference Include=""Grpc.Tools"" Version=""2.34.0"">
        <PrivateAssets>all</PrivateAssets>
        </PackageReference>
        </ItemGroup>
        <ItemGroup>
    
        <Protobuf Include=""../**/*.proto"" OutputDir=""%(RelativeDir)"" CompileOutputs=""false"" />
        </ItemGroup>
        </Project>
        ";
    }
}

