using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditorInternal;
using UnityEngine;
using Object = System.Object;

namespace UnityGRPC.Editor
{
    public class ProtoTemplateMenuItem
    {
        [MenuItem("Assets/Create/" + "ProtoFile", false, 70)]
        private static void CreateProtoFile()
        {
            CreateFromTemplate(
                "NewProto.proto", 
                Utility.GetPluginPath()+"\\Editor\\Templates\\proto.txt");
        }
        public static void CreateFromTemplate(string initialName, string templatePath)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<DoCreateCodeFile>(),
                initialName,
                (EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D),
                resourceFile:templatePath);
        }
        internal static UnityEngine.Object CreateScript(string pathName, string templatePath)
        {
            string newFilePath = Path.GetFullPath (pathName);

            string templateText = string.Empty;

            if (File.Exists(templatePath))
            {
                using (var sr = new StreamReader (templatePath))
                {
                    templateText = sr.ReadToEnd();
                }

                var packageName = Utility.GetPackageName(pathName);
                templateText = templateText.Replace ("##PACKAGE##", packageName);
                var nameSpace = Utility.GetNameSpace(pathName);
                templateText = templateText.Replace ("##NAMESPACE##", nameSpace);
                UTF8Encoding encoding = new UTF8Encoding (true, false);
				
                using (var sw = new StreamWriter (newFilePath, false, encoding))
                {
                    sw.Write (templateText);
                }
				
                AssetDatabase.ImportAsset (pathName);
				
                return AssetDatabase.LoadAssetAtPath (pathName, typeof(Object));
            }
            else
            {
                Debug.LogError(string.Format("The template file was not found: {0}", templatePath));

                return null;
            }
        }
    }
    public class DoCreateCodeFile : EndNameEditAction
    {
        public override void Action (int instanceId, string pathName, string resourceFile)
        {
            pathName = Utility.GetCleanPath(pathName);
            UnityEngine.Object o = ProtoTemplateMenuItem.CreateScript(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(o);
        }

        
    }

    public static class Utility
    {
        public static string GetPackageName(string pathName)
        {
            var folders = Path.GetDirectoryName(pathName).Split('\\');
            return folders[folders.Length - 1];
        }

        public static string GetNameSpace(string pathName)
        {
            var folders = Path.GetDirectoryName(pathName).Split('\\');
            var package =  folders[folders.Length - 1];
            if (folders.Length == 1)
                return package;
            var root = folders[1];
            return root + "." + package;
        }
        public static string GetCleanPath(string pathName)
        {
            if (pathName.Contains(".proto"))
            {
                var tmpPath = pathName.Remove(pathName.Length - 5, 5);
                if (tmpPath.Contains(".proto"))
                {
                    return tmpPath;
                }
            }
            return pathName;
        }
        public static string GetPluginPath()
        {
            if (System.IO.File.Exists(Path.GetFullPath("Packages/com.donnerlab.unitygrpc/README.md")))
                return Path.GetFullPath("Packages/com.donnerlab.unitygrpc");
            if(File.Exists(Application.dataPath+"/UnityGRPC/README.md"))
                return Application.dataPath +"/UnityGRPC";
            if (System.IO.File.Exists(Path.GetFullPath("Packages/UnityGRPC/README.md")))
                return Path.GetFullPath("Packages/UnityGRPC/Tools~/UnityGRPC.csproj");
            return Path.GetFullPath("Packages/com.donnerlab.unitygrpc");
        }
    }
}