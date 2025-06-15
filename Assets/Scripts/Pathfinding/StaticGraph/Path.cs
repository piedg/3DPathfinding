using System;
using Unity.Mathematics;
using UnityEngine;

public class Path : MonoBehaviour
{
    
    public bool Loop;
    public Transform[] Nodes = Array.Empty<Transform>();
    
    public float3 GetPointPosition(int index)
    {
        return Nodes[index].position;
    }

    
    public int GetNearestPoint(float3 position)
    {
        var nearestIndex = -1;
        var nearestDistance = float.MaxValue;

        for (var i = 0; i < Nodes.Length; i++)
        {
            var newDistance = math.distancesq(Nodes[i].position, position);
            if (newDistance < nearestDistance)
            {
                nearestDistance = newDistance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }
    
    private float SegmentPointDistance(float3 segmentStart, float3 segmentEnd, float3 point)
    {
        var lengthSquared = math.distancesq(segmentStart, segmentEnd);
        if (lengthSquared == 0f)
        {
            return math.distance(segmentStart, point);
        }

        var t = math.max(0, math.min(1, math.dot(point - segmentStart, segmentEnd - segmentStart) / lengthSquared));
        var projection = segmentStart + t * (segmentEnd - segmentStart);
        return math.distance(point, projection);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (var i = 0; i < Nodes.Length - 1; i++)
        {
            Gizmos.DrawLine(Nodes[i].position, Nodes[i + 1].position);
        }

        if (Loop)
        {
            Gizmos.DrawLine(Nodes[^1].position, Nodes[0].position);
        }
        
        Gizmos.color = Color.red;
        foreach (var node in Nodes)
        {
            Gizmos.DrawWireSphere(node.position, .25f);
        }
    }
}