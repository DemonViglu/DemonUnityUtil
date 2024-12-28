using UnityEditor;
using UnityEngine;
using demonviglu.config;
using System;

public class SaveEditor : EditorWindow
{
    private SerializedObject serObj;

    private static SaveEditor instance;

    public DemonConfig dm = new();
    public SerializedProperty dmp;

    [MenuItem("Tools/DemonTool/SaveEditor")]
    private static void ShowWindow()
    {
        instance = EditorWindow.GetWindow<SaveEditor>();
        instance.Show();
    }
    public void OnEnable()
    {
        serObj = new SerializedObject(this);
        dmp = serObj.FindProperty("dm");
    }
    public void OnGUI()
    {
        serObj.Update();
        EditorGUILayout.LabelField("Welcome using DemonViglu's JsonTool", EditorStyles.boldLabel);

        if (GUILayout.Button("Save"))
        {
            Save();
        }
        if (GUILayout.Button("Load"))
        {
            Load();
        }
        EditorGUILayout.LabelField("Here is the data", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(dmp);


        serObj.ApplyModifiedProperties();
    }

    private void Save()
    {
        dm.Save();
    }

    private void Load()
    {
        dm = DemonConfig.Load<DemonConfig>();
    }
}

[Serializable]
public class DemonConfig : BaseConfig
{
    public string name = "ahh";
    public int old = 1;
}
