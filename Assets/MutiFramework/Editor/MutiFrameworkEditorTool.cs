﻿#if MutiFramework
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MutiFramework
{
    class MutiFrameworkEditorTool
    {
        public class UploadInfo
        {
            public string unityVersion { get { return Application.unityVersion; } }
            public bool isPublic = true;
            public string name = "pkg name";
            public string version = "0.0.0.1";
            public string author = "author";
            public string describtion = "No Describtion ";
            public string assetPath = "Assets";
            public string helpurl = MutiFrameworkEditorTool.baidu;
            public List<string> dependences = new List<string>();
        }

        internal const string baidu = "https://www.baidu.com/";
        internal const string frameworkUrl = "https://tdouguo.github.io/x/";
        internal const string version="0.0.0.1";
        internal const string framewokName = "MutiFramework";
        private const string userJsonName = "version.json";
        private static Encoding encod = Encoding.UTF8;

        internal static string rootPath
        {
            get
            {
                string path = Path.Combine(EditorApplication.applicationContentsPath, framewokName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }



        internal static void CreateClass()
        {

            string flag = "//ToDo";
            string editorPath = "Assets/MutiFramework/Editor/EditorFrameworks.cs";
            string rtPath = "Assets/MutiFramework/Frameworks.cs";
            Encoding utf8 = Encoding.UTF8;
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany((a) => { return a.GetTypes(); });
            EditorUtility.DisplayProgressBar("Fresh Scripts", "", 0);

            string txt = File.ReadAllText(editorPath, utf8);
            string add = "";
            types
                 .Where((type) => {
                     return !type.IsAbstract && type.IsSubclassOf(typeof(Framework)) &&
                         type.IsDefined(typeof(FrameworkAttribute), false) &&
                         (type.GetCustomAttributes(typeof(FrameworkAttribute), false).First() as FrameworkAttribute).env.HasFlag(EnvironmentType.Editor);
                 })
                 .Select((type) => {
                     Framework f = Activator.CreateInstance(type) as Framework;
                     return f;
                 }).ToList()
                 .ForEach((f) => {
                     add = add + "\t\tpublic static " + f.GetType() + " " + f.name + "{ get { return GetFramework(\"" + f.name + "\") as " + f.GetType() + ";}} \n";
                     //_container.Subscribe(f);
                     f.Dispose();
                 });
            int first = txt.IndexOf(flag);
            int last = txt.LastIndexOf(flag);
            string _1 = txt.Substring(0, first);
            string _2 = flag + "\n" + add;
            string _3 = txt.Substring(last, txt.Length - last);

            txt = _1 + _2 + _3;
            File.WriteAllText(editorPath, txt.Replace("\r\n", "\n"), utf8);
            EditorUtility.DisplayProgressBar("Fresh Scripts", "", 0.5f);



            txt = File.ReadAllText(rtPath, utf8);
            add = "";
            types
                 .Where((type) => {
                     return !type.IsAbstract && type.IsSubclassOf(typeof(Framework)) &&
                         type.IsDefined(typeof(FrameworkAttribute), false) &&
                         (type.GetCustomAttributes(typeof(FrameworkAttribute), false).First() as FrameworkAttribute).env.HasFlag(EnvironmentType.Runtime);
                 })
                 .Select((type) => {
                     Framework f = Activator.CreateInstance(type) as Framework;
                     return f;
                 }).ToList()
                 .ForEach((f) => {
                     add = add + "\t\tpublic static " + f.GetType() + " " + f.name + "{ get { return GetFramework(\"" + f.name + "\") as " + f.GetType() + ";}} \n";
                     //_container.Subscribe(f);
                     f.Dispose();
                 });

            first = txt.IndexOf(flag);
            last = txt.LastIndexOf(flag);
            _1 = txt.Substring(0, first);
            _2 = flag + "\n" + add;
            _3 = txt.Substring(last, txt.Length - last);

            txt = _1 + _2 + _3;
            File.WriteAllText(rtPath, txt.Replace("\r\n", "\n"), utf8);
            EditorUtility.DisplayProgressBar("Fresh Scripts", "", 0.8f);

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();

        }
        internal static string ReadDiskVersion(string assetPath)
        {
            string version = "";
            string path = Path.Combine(assetPath, userJsonName);
            if (File.Exists(path))
            {
                version=JsonUtility.FromJson<UploadInfo>(File.ReadAllText(path,encod)).version;
            }
            return version;
        }
        internal static void CreateVersionJson(string assetPath,UploadInfo info)
        {
            string path = Path.Combine(assetPath, userJsonName);
            File.WriteAllText(path, JsonUtility.ToJson(info), encod);
        }
        internal static void RemovePakage(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif