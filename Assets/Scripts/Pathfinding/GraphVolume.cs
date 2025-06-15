using System;
using Unity.Mathematics;
using UnityEngine;

public class GraphVolume : MonoBehaviour
{
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private LayerMask collisionMask;

    [SerializeField] private float3 volumeSize;

    [SerializeField] private float nodeDistance;

    private Graph _graph = new();
    public Graph Graph => _graph;

    private void Start()
    {
        Bake();
    }

    private void Bake()
    {
        for (var z = transform.position.z - (volumeSize.z / 2);
             z < transform.position.z + (volumeSize.z / 2);
             z += nodeDistance)
        {
            for (var x = transform.position.x - (volumeSize.x / 2);
                 x < transform.position.x + (volumeSize.x / 2);
                 x += nodeDistance)
            {
                if (Physics.Raycast(new float3(x, transform.position.y + (volumeSize.y / 2), z), math.down(),
                        out var hit, volumeSize.z + math.EPSILON, floorMask | collisionMask))
                {
                    if (floorMask == (floorMask | (1 << hit.transform.gameObject.layer)))
                    {
                        _graph.AddNode(hit.point);
                    }
                }
            }
        }

        foreach (var firstNode in _graph.Nodes)
        {
            foreach (var secondNode in _graph.Nodes)
            {
                if (firstNode.Equals(secondNode))
                {
                    continue;
                }

                if (math.abs(secondNode.Value.x - firstNode.Value.x) <= nodeDistance &&
                    math.abs(secondNode.Value.z - firstNode.Value.z) <= nodeDistance)
                {
                    if (!Physics.Raycast(firstNode.Value, math.normalizesafe(secondNode.Value - firstNode.Value),
                            math.distance(firstNode, secondNode) * 2, collisionMask))
                    {
                        _graph.AddEdge(firstNode, secondNode, math.distancesq(firstNode, secondNode));
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, volumeSize);
        foreach (var node in _graph.Nodes)
        {
            if (node != null)
            {
                Gizmos.DrawSphere((float3)node, .5f);
            }
        }

        Gizmos.color = Color.yellow;
        foreach (var edge in _graph.Edges)
        {
            if (edge != null)
            {
                Gizmos.DrawLine((float3)_graph.Nodes[edge.StartNode], (float3)_graph.Nodes[edge.EndNode]);
            }
        }
    }
}