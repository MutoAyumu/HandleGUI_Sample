using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Sample2))]
public class HandleEditor2 : Editor
{
    Sample2 _sample;

    private void OnEnable()
    {
        _sample = target as Sample2;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Add"))
        {
            //値の保存を行う
            Undo.RecordObject(target, "Add Node");

            StartEndPos pos;

            if (_sample.LocalNodes.Length > 1)
            {
                pos.Start = _sample.LocalNodes[_sample.LocalNodes.Length - 1].Start + Vector3.right;
                pos.End = _sample.LocalNodes[_sample.LocalNodes.Length - 1].End + Vector3.right;
            }
            else
            {
                if (_sample.LocalNodes.Length <= 0)
                {
                    pos.Start = _sample.transform.position;
                    pos.End = _sample.transform.position;
                }
                else
                {
                    pos = _sample.LocalNodes[0];
                }
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

            Vector3 start;
            Vector3 end;

            start = EditorGUILayout.Vector3Field("Start", _sample.LocalNodes[i].Start);
            end = EditorGUILayout.Vector3Field("End", _sample.LocalNodes[i].End);

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Position");

                _sample.LocalNodes[i] = new StartEndPos(start, end);
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
            Vector3 start;
            Vector3 end;

            if (Application.isPlaying)
            {
                //エディタ実行時
                start = _sample.WorldNodes[i].Start;
                end = _sample.WorldNodes[i].End;
            }
            else
            {
                start = _sample.transform.TransformPoint(_sample.LocalNodes[i].Start);
                end = _sample.transform.TransformPoint(_sample.LocalNodes[i].End);
            }

            Vector3 newStart = start;
            Vector3 newEnd = end;

            newStart = Handles.PositionHandle(start, Quaternion.identity);
            newEnd = Handles.PositionHandle(end, Quaternion.identity);

            Handles.color = Color.red;

            if (i == 0)
            {
                if (!Application.isPlaying)
                {
                    Handles.DrawDottedLine(start, end, 10);
                }
                else
                {
                    Handles.DrawDottedLine(_sample.transform.TransformPoint(_sample.LocalNodes[i - 1].Start), _sample.transform.TransformPoint(_sample.LocalNodes[i - 1].End), 10);
                }
            }
            else
            {
                if (!Application.isPlaying)
                {
                    Handles.DrawDottedLine(start, end, 10);
                }
                else
                {
                    Handles.DrawDottedLine(_sample.transform.TransformPoint(_sample.LocalNodes[i - 1].Start), _sample.transform.TransformPoint(_sample.LocalNodes[i - 1].End), 10);
                }
            }

            if (newStart != start)
            {
                Undo.RecordObject(target, "Moved Point");
                _sample.LocalNodes[i].Start = _sample.transform.InverseTransformPoint(newStart);
            }
            if (newEnd != end)
            {
                Undo.RecordObject(target, "Moved Point");
                _sample.LocalNodes[i].End = _sample.transform.InverseTransformPoint(newEnd);
            }

            Handles.Label(start, $"Node{i}[Start]:{start}");
            Handles.Label(end, $"Node{i}[End]:{end}");
        }
    }
}