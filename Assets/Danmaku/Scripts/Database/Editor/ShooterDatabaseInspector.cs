using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;


[CustomEditor(typeof(ShooterDatabase))]
public class ShooterDatabaseEditor : Editor
{
    private readonly Dictionary<string, bool> foldouts = new();

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var listProp = serializedObject.FindProperty("shooterDatas");

        EditorGUILayout.LabelField("Shooter Database", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("＋ Add Radial", GUILayout.Height(24)))
                AddEntry(listProp, new RadialShooterData { ShooterType = ShooterEnum.Radial });

            if (GUILayout.Button("＋ Add Orbit", GUILayout.Height(24)))
                AddEntry(listProp, new OrbitShooterData { ShooterType = ShooterEnum.Orbit });

            if (GUILayout.Button("＋ Add Block", GUILayout.Height(24)))
                AddEntry(listProp, new ShooterBaseData { ShooterType = ShooterEnum.Block });
        }

        EditorGUILayout.Space(6);

        for (int i = 0; i < listProp.arraySize; i++)
        {
            var element = listProp.GetArrayElementAtIndex(i);
            var managedRef = element.managedReferenceValue;
            string typeName = managedRef?.GetType().Name ?? "null";
            string key = $"{i}_{typeName}";

            if (!foldouts.ContainsKey(key)) 
                foldouts[key] = true;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    foldouts[key] = EditorGUILayout.Foldout(foldouts[key], $"[{i}]  {typeName}", true);

                    GUILayout.FlexibleSpace();

                    GUI.color = new Color(1f, 0.5f, 0.5f);
                    if (GUILayout.Button("✕", GUILayout.Width(24), GUILayout.Height(18)))
                    {
                        Undo.RecordObject(target, "Remove Shooter");
                        listProp.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        break;
                    }
                    GUI.color = Color.white;
                }

                if (foldouts[key])
                {
                    using (new IndentLevelScope(8))
                    {
                        DrawAllVisibleChildren(element);
                    }
                }
            }

            EditorGUILayout.Space(2);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private static void DrawAllVisibleChildren(SerializedProperty parent)
    {
        var iter = parent.Copy();
        var end = parent.GetEndProperty();
        bool enterChildren = true;

        while (iter.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iter, end))
        {
            enterChildren = false;
            EditorGUILayout.PropertyField(iter, true);
        }
    }

    private void AddEntry(SerializedProperty listProp, ShooterBaseData newData)
    {
        Undo.RecordObject(target, "Add Shooter");
        int idx = listProp.arraySize;
        listProp.arraySize++;
        listProp.GetArrayElementAtIndex(idx).managedReferenceValue = newData;
        serializedObject.ApplyModifiedProperties();
    }
}

public class IndentLevelScope : GUI.Scope
{
    public Rect rect { get; protected set; }

    public IndentLevelScope(float pixel, params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(pixel);
        rect = EditorGUILayout.BeginVertical(options);
    }

    public IndentLevelScope(float pixel, GUIStyle style, params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(pixel);
        rect = EditorGUILayout.BeginVertical(style, options);
    }

    protected override void CloseScope()
    {
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}