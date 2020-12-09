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
        [MenuItem("UnityGRPC/dotnet restore")]
        public static async void DotNetRestore()
        {

            await runDotnetProcess("restore");
            await runDotnetProcess("build");
            Debug.Log("got here");

        }

        private static async Task runDotnetProcess(string argument)
        {
            var process = new Process();
            process.StartInfo.WorkingDirectory = Utility.GetPluginPath()+"/Tools~";
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = argument;
            process.OutputDataReceived += ProcessOnOutputDataReceived;
            process.ErrorDataReceived += ProcessOnErrorDataReceived;
            process.Exited += ProcessOnExited;
            process.Start();
            while (!process.HasExited)
            {
                await Task.Delay(1);
            }

            if (process.ExitCode != 0)
            {
                throw new Exception("dotnet " + argument + " has failed");
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

        
    }
}

