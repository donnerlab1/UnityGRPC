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
        [MenuItem("UnityGRPC/Compile Protofiles")]
        public static async void CompileProtofiles()
        {
            SetupTools();
            RunDotnetProcess("restore");
            RunDotnetProcess("build");
            Debug.Log("Finished Compiling");

        }
        
        
        private static void RunDotnetProcess(string argument)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.WorkingDirectory = toolsPath;
            psi.FileName = "dotnet";
            psi.Arguments = argument;
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;

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
        
        

        

        private static void SetupTools()
        {
            if (!Directory.Exists(toolsPath))
                Directory.CreateDirectory(toolsPath);
            WriteFile(csprojFilePath, csprojFileContent);
            WriteFile(gitignoreFilePath, gitignoreFileContent);
            
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
        
        private static void ProcessOnExited(object sender, EventArgs e)
        {
            ((IDisposable)sender).Dispose();
        }

        private static void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.LogError(e.Data);
        }

        private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Log(e.Data);
        }
        private static string toolsPath = Application.dataPath + "/GrpcTools~";
        private static string csprojFilePath = toolsPath + "/.protos.csproj";
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
        private static string gitignoreFilePath = toolsPath + "/.gitignore";
        private const string gitignoreFileContent = @"
obj/**
bin/**
";
    }
}

