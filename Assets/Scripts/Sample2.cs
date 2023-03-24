using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample2 : MonoBehaviour
{
    [HideInInspector] public StartEndPos[] LocalNodes = new StartEndPos[1];
    protected StartEndPos[] _worldNodes;
    public StartEndPos[] WorldNodes => _worldNodes;

    private void Awake()
    {
        _worldNodes = new StartEndPos[LocalNodes.Length];

        for (int i = 0; i < _worldNodes.Length; i++)
        {
            _worldNodes[i].Start = transform.TransformPoint(LocalNodes[i].Start);
            _worldNodes[i].End = transform.TransformPoint(LocalNodes[i].End);
        }
    }
}
public struct StartEndPos
{
    public StartEndPos(Vector3 start, Vector3 end)
    {
        Start = start;
        End = end;
    }

    public Vector3 Start;
    public Vector3 End;
}
