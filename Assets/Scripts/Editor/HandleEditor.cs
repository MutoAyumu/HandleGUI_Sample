using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Handle))]
public class SampleEditor : Editor
{
    Handle _sample;

    private void OnEnable()
    {
        _sample = target as Handle;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Add"))
        {
            //値の保存を行う
            Undo.RecordObject(target, "Add Node");

            Vector3 pos;

            if (_sample.LocalNodes.Length > 0)
            {
                pos = _sample.LocalNodes[_sample.LocalNodes.Length - 1] + Vector3.right;
            }
            else
            {
                pos = _sample.transform.position + Vector3.right;
            }

            ArrayUtility.Add(ref _sample.LocalNodes, pos);
        }

        EditorGUIUtility.labelWidth = 50;
        int delete = -1;

        for (int i = 0; i < _sample.LocalNodes.Length; i++)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();

            int size = 50;
            EditorGUILayout.BeginVertical(GUILayout.Width(size));
            EditorGUILayout.LabelField("Node " + i, GUILayout.Width(size));

            if (GUILayout.Button("Delete", GUILayout.Width(size)))
            {
                delete = i;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            Vector3 pos;

            pos = EditorGUILayout.Vector3Field("Position", _sample.LocalNodes[i]);

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Position");

                _sample.LocalNodes[i] = pos;
            }
        }

        EditorGUIUtility.labelWidth = 0;

        if (delete != -1)
        {
            Undo.RecordObject(target, "Delete Node");

            ArrayUtility.RemoveAt(ref _sample.LocalNodes, delete);
        }
    }
    private void OnSceneGUI()
    {
        for (int i = 0; i < _sample.LocalNodes.Length; i++)
        {
            Vector3 pos;

            if (Application.isPlaying)
            {
                //エディタ実行時
                pos = _sample.LocalNodes[i];
            }
            else
            {
                pos = _sample.transform.TransformPoint(_sample.LocalNodes[i]);
            }

            Vector3 newPos = pos;

            newPos = Handles.PositionHandle(pos, Quaternion.identity);

            Handles.color = Color.yellow;

            if (newPos != pos)
            {
                Undo.RecordObject(target, "Moved Point");
                _sample.LocalNodes[i] = _sample.transform.InverseTransformPoint(newPos);
            }

            Handles.SphereHandleCap(0, pos, Quaternion.identity, 0.2f, EventType.Repaint);
            Handles.Label(pos, $"Node{i}:{pos - _sample.transform.position}");
        }
    }
}