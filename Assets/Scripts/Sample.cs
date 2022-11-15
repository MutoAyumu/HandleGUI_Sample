using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    [HideInInspector] public Vector3[] LocalNodes = new Vector3[1];
    protected Vector3[] _worldNodes;
    public Vector3[] WorldNodes => _worldNodes;

    private void Awake()
    {
        _worldNodes = new Vector3[LocalNodes.Length];

        for(int i = 0; i < _worldNodes.Length; i++)
        {
            _worldNodes[i] = transform.TransformPoint(LocalNodes[i]);
        }
    }
}