using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Handle))]
public class SampleEditor : Editor
{
    /* MEMO
     using UnityEditorを追加したクラスはEditorフォルダーに属していないとエラーがでてしまう
     */

    Handle _sample;

    private void OnEnable()
    {
        _sample = target as Handle;
    }

    public override void OnInspectorGUI()
    {
        //インスペクターにボタンを追表示
        if (GUILayout.Button("Add"))
        {
            //値の保存を行う
            Undo.RecordObject(target, "Add Node");

            Vector3 pos;

            //一個前のノードから少しずらした位置に設定
            if (_sample.LocalNodes.Length > 0)
            {
                pos = _sample.LocalNodes[_sample.LocalNodes.Length - 1] + Vector3.right;
            }
            else//ノードが無かった時は自身を基準に設定
            {
                pos = _sample.transform.position + Vector3.right;
            }

            //ローカルノード配列に要素を追加
            ArrayUtility.Add(ref _sample.LocalNodes, pos);
        }

        EditorGUIUtility.labelWidth = 50;
        int delete = -1;

        for (int i = 0; i < _sample.LocalNodes.Length; i++)
        {
            //インスペクターが変更されたかを確認
            EditorGUI.BeginChangeCheck();
            //ここからEndHorizontalまでを横並び表示させる
            EditorGUILayout.BeginHorizontal();

            int size = 50;
            //ここからEndVerticalまでを縦並びに表示させる
            EditorGUILayout.BeginVertical(GUILayout.Width(size));
            EditorGUILayout.LabelField("Node " + i, GUILayout.Width(size));

            //ボタンを追加
            if (GUILayout.Button("Delete", GUILayout.Width(size)))
            {
                //何番目のボタンを消すかを設定
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
                //インスペクターが変更されていたらシーンの変更をさせる
                Undo.RecordObject(target, "Changed Position");

                //配列のi番目の要素を変更
                _sample.LocalNodes[i] = pos;
            }
        }

        EditorGUIUtility.labelWidth = 0;

        //削除する番号が変更されていたら
        if (delete != -1)
        {
            Undo.RecordObject(target, "Delete Node");

            //配列からi番目の要素を削除
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