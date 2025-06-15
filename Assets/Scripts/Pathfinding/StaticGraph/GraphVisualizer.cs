using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GraphVisualizer : MonoBehaviour
{
    [SerializeField] WorldGraph worldGraph;

    [SerializeField] private Transform startNode;
    [SerializeField] private Transform endNode;
    [SerializeField] private bool execute;
    private float3[] _path = Array.Empty<float3>();

    private void Update()
    {
        if (!execute)
        {
            return;
        }

        FindPath();
        execute = false;
    }

    private void FindPath()
    {
        if (worldGraph == null || worldGraph.Graph == null)
        {
            return;
        }

        if (startNode == null)
        {
            return;
        }

        if (endNode == null)
        {
            return;
        }

        _path = worldGraph.Graph.GetPathTo(startNode.position, endNode.position).ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        for (var i = 0; i < _path.Length - 1; i++)
        {
            Gizmos.DrawLine(_path[i], _path[i + 1]);
        }

        Gizmos.color = Color.magenta;
        foreach (var pos in _path)
        {
            Gizmos.DrawWireSphere(pos, 0.6f);
        }
    }
}