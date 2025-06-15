using System;
using UnityEngine;

public class WorldGraph : MonoBehaviour
{
    [Serializable]
    public class Edge
    {
        public Transform Start;
        public Transform End;
        public int Weight;
    }

    public Transform[] Nodes;
    public Edge[] Edges;

    private Graph _graph;

    public Graph Graph => _graph;

    private void Start()
    {
        _graph = new Graph();

        foreach (var node in Nodes)
        {
            _graph.AddNode(node.position);
        }

        foreach (var edge in Edges)
        {
            _graph.AddEdge(edge.Start.position, edge.End.position, edge.Weight);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var node in Nodes)
        {
            if (node)
            {
                Gizmos.DrawSphere(node.position, .25f);
            }
        }
        
        Gizmos.color = Color.yellow;
        foreach (var edge in Edges)
        {
            if (edge != null)
            {
                Gizmos.DrawLine(edge.Start.position, edge.End.position);

            }
        }
    }
}