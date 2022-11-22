using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Handle))]
public class SampleEditor : Editor
{
    /* MEMO
     using UnityEditor��ǉ������N���X��Editor�t�H���_�[�ɑ����Ă��Ȃ��ƃG���[���łĂ��܂�
     */

    Handle _sample;

    private void OnEnable()
    {
        _sample = target as Handle;
    }

    public override void OnInspectorGUI()
    {
        //�C���X�y�N�^�[�Ƀ{�^����Ǖ\��
        if (GUILayout.Button("Add"))
        {
            //�l�̕ۑ����s��
            Undo.RecordObject(target, "Add Node");

            Vector3 pos;

            //��O�̃m�[�h���班�����炵���ʒu�ɐݒ�
            if (_sample.LocalNodes.Length > 0)
            {
                pos = _sample.LocalNodes[_sample.LocalNodes.Length - 1] + Vector3.right;
            }
            else//�m�[�h�������������͎��g����ɐݒ�
            {
                pos = _sample.transform.position + Vector3.right;
            }

            //���[�J���m�[�h�z��ɗv�f��ǉ�
            ArrayUtility.Add(ref _sample.LocalNodes, pos);
        }

        EditorGUIUtility.labelWidth = 50;
        int delete = -1;

        for (int i = 0; i < _sample.LocalNodes.Length; i++)
        {
            //�C���X�y�N�^�[���ύX���ꂽ�����m�F
            EditorGUI.BeginChangeCheck();
            //��������EndHorizontal�܂ł������ѕ\��������
            EditorGUILayout.BeginHorizontal();

            int size = 50;
            //��������EndVertical�܂ł��c���тɕ\��������
            EditorGUILayout.BeginVertical(GUILayout.Width(size));
            EditorGUILayout.LabelField("Node " + i, GUILayout.Width(size));

            //�{�^����ǉ�
            if (GUILayout.Button("Delete", GUILayout.Width(size)))
            {
                //���Ԗڂ̃{�^������������ݒ�
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
                //�C���X�y�N�^�[���ύX����Ă�����V�[���̕ύX��������
                Undo.RecordObject(target, "Changed Position");

                //�z���i�Ԗڂ̗v�f��ύX
                _sample.LocalNodes[i] = pos;
            }
        }

        EditorGUIUtility.labelWidth = 0;

        //�폜����ԍ����ύX����Ă�����
        if (delete != -1)
        {
            Undo.RecordObject(target, "Delete Node");

            //�z�񂩂�i�Ԗڂ̗v�f���폜
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
                //�G�f�B�^���s��
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