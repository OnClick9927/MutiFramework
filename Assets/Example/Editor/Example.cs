﻿using MutiFramework;
using UnityEngine;

public class ExampleFrame1Line : FrameworkLineDrawer
{
    public override string name { get { return "ExampleFrame1"; } }
    public override void OnGUI()
    {
        base.OnGUI();
        if (GUILayout.Button("Say Type"))
        {
            Debug.Log(GetType().FullName);
        }  
    }
}
public class ExampleFrame2Line : FrameworkLineDrawer
{
    public override string name { get { return "ExampleFrame2"; } }
    public override void OnGUI()
    {
        base.OnGUI();
        if (GUILayout.Button("Say Type"))
        {
            Debug.Log(GetType().FullName);
        }
    }
}



public class ExampleToolLine : ToolLineDrawer
{
    private Color _color;
    public override string name => "ChooseColor";
    public override void OnGUI()
    {
        base.OnGUI();
        GUI.color = _color;
        _color= UnityEditor.EditorGUILayout.ColorField(_color);
        GUI.color = Color.white;
    }
}