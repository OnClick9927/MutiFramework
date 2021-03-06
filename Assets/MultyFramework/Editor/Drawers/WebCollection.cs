﻿using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace MultyFramework
{
    public class WebCollection : MultyFrameworkDrawer
    {
        private string _name;
        private string _author;


        private WebCollectionInfo.Version _current { get { return versions[_versionSelect]; } }
        public WebCollectionInfo.Version[] versions;

        public string unityVersion { get { return _current.unityVersion; } }

        public bool exist { get { return Directory.Exists(assetPath); } }

        public override string name { get { return _name; } }
        public override string version { get { return _current.version; } }
        public override string author { get { return _author; } }
        public override string describtion { get { return _current.describtion; } }
        public string assetPath { get { return _current.assetPath; } }

        public override string helpurl
        {
            get
            {
                if (string.IsNullOrEmpty(_current.helpurl))
                    return base.helpurl;
                return _current.helpurl;
            }
        }
        public override string[] dependences { get { return _current.dependences; } }

        public WebCollection(string name, string author, WebCollectionInfo.Version[] versions)
        {
            _name = name;
            this.versions = versions;
            _author = author;
            _versionSelect = 0;
        }
        private int _versionSelect;

        private bool _describtionFold = true;
        private bool _dependencesFold = true;
        private Vector2 _scroll;
        private string[] _versionNames;
        public override void Awake()
        {
            MultyFrameworkEditorTool.onPackagesChange += OnEnable;
        }
        public override void OnEnable()
        {
            _versionNames = new string[versions.Length];
            for (int i = 0; i < versions.Length; i++)
            {
                _versionNames[i] = "v " + versions[i].version;
            }
           
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect);
            {
                _scroll = GUILayout.BeginScrollView(_scroll);
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(name, Styles.header);
                    if (GUILayout.Button(Contents.help, Styles.controlLabel))
                    {
                        Help.BrowseURL(helpurl);
                    }
                    GUILayout.Space(10);
                    GUILayout.Label(unityVersion);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(new GUIContent("Newest", "Update to Newest")))
                    {
                        UpdatePakage();
                    }
                    using (new EditorGUI.DisabledScope(exist))
                    {
                        if (GUILayout.Button("Install", Styles.buttonLeft))
                        {
                            InstallPakage();
                        }
                    }
                    _versionSelect = EditorGUILayout.Popup(_versionSelect, _versionNames, new GUIStyle("Popup")
                    {
                        margin = new RectOffset(2, 0, 3, 2)
                    }, GUILayout.Width(Contents.gap * 10));
                    using (new EditorGUI.DisabledScope(!exist))
                    {
                        if (GUILayout.Button("Remove"))
                        {
                            RemovePakage(assetPath);
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.Label("Version  " + version, Styles.boldLabel);
                GUILayout.Label("Author " + author, Styles.boldLabel);

                {
                    GUILayout.Label("Dependences", Styles.in_title);
                    Rect last = GUILayoutUtility.GetLastRect();
                    last.width -= 10;
                    last.xMin += 10;
                    if (Event.current.type == EventType.MouseUp && last.Contains(Event.current.mousePosition))
                    {
                        _dependencesFold = !_dependencesFold;
                        Event.current.Use();
                    }
                    last.xMin -= 10;
                    last.width = 10;

                    _dependencesFold = EditorGUI.Foldout(last, _dependencesFold, "");
                    if (_dependencesFold)
                    {
                        if (dependences != null)
                        {
                            for (int i = 0; i < dependences.Length; i++)
                            {
                                GUILayout.Label(dependences[i]);
                            }
                        }
                    }
                }
                {
                    GUILayout.Label("Describtion ", Styles.in_title);
                    Rect last = GUILayoutUtility.GetLastRect();
                    last.width -= 10;
                    last.xMin += 10;
                    if (Event.current.type == EventType.MouseUp && last.Contains(Event.current.mousePosition))
                    {
                        _describtionFold = !_describtionFold;
                        Event.current.Use();
                    }
                    last.xMin -= 10;
                    last.width = 10;
                    _describtionFold = EditorGUI.Foldout(last, _describtionFold, "");
                    if (_describtionFold)
                    {
                        GUILayout.Label(describtion);
                    }
                }
                GUILayout.Label("", Styles.in_title, GUILayout.Height(0));
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }

        private void UpdatePakage()
        {
            MultyFrameworkEditorTool.UpdatePakage(name);
        }

        private void InstallPakage()
        {
            MultyFrameworkEditorTool.RemovePakageFromAssets(assetPath);
            MultyFrameworkEditorTool.InstallPackage(name, version);
        }
        protected virtual void RemovePakage(string path)
        {
            MultyFrameworkEditorTool.RemovePakageFromAssets(path);
        }



        public static implicit operator WebCollection(WebCollectionInfo info)
        {
            return new WebCollection(info.name, info.author, info.versions);
        }
        public static implicit operator WebCollectionInfo(WebCollection drawer)
        {
            return new WebCollectionInfo(drawer.name, drawer.author, drawer.versions);
        }
    }
}
