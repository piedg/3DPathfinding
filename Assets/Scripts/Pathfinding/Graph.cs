using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

public class GraphNode : IEquatable<GraphNode>
{
    public readonly float3 Value;

    public GraphNode(float3 value)
    {
        Value = value;
    }

    public bool Equals(GraphNode other)
    {
        return Value.Equals(other.Value);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((GraphNode)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static implicit operator float3(GraphNode node) => node.Value;
    public static implicit operator GraphNode(float3 node) => new(node);
}

public class GraphEdge
{
    public int StartNode;
    public int EndNode;
    public float Weight;
}

public class Graph
{
    private List<GraphNode> _nodes = new();
    private List<GraphEdge> _edges = new();

    public List<GraphNode> Nodes => _nodes;
    public List<GraphEdge> Edges => _edges;

    public void AddNode(float3 node)
    {
        if (_nodes.Contains(node))
        {
            return;
        }

        _nodes.Add(node);
    }

    public void RemoveNode(float3 node)
    {
        if (!_nodes.Contains(node))
        {
            return;
        }

        var nodeIndex = _nodes.IndexOf(node);
        
        for (var i = _edges.Count - 1; i >= 0; i--)
        {
            if (_edges[i].StartNode == nodeIndex || _edges[i].EndNode == nodeIndex)
            {
                _edges.RemoveAt(i);
                continue;
            }

            if (_edges[i].StartNode > nodeIndex)
            {
                _edges[i].StartNode--;
            }
            
            if (_edges[i].EndNode > nodeIndex)
            {
                _edges[i].EndNode--;
            }
        }

        _nodes.Remove(node);
    }

    public void AddEdge(float3 startNode, float3 endNode, float weight)
    {
        if (startNode.Equals(endNode))
        {
            return;
        }

        if (!_nodes.Contains(startNode))
        {
            return;
        }

        if (!_nodes.Contains(endNode))
        {
            return;
        }

        var startNodeIndex = _nodes.IndexOf(startNode);
        var endNodeIndex = _nodes.IndexOf(endNode);

        if (_edges.Any(x => x.StartNode == startNodeIndex && x.EndNode == endNodeIndex))
        {
            return;
        }
        
        _edges.Add(new GraphEdge{StartNode = startNodeIndex, EndNode = endNodeIndex, Weight = weight});
    }

    public void RemoveEdge(float3 startNode, float3 endNode)
    {
        if (startNode.Equals(endNode))
        {
            return;
        }

        if (!_nodes.Contains(startNode))
        {
            return;
        }

        if (!_nodes.Contains(endNode))
        {
            return;
        }

        var startNodeIndex = _nodes.IndexOf(startNode);
        var endNodeIndex = _nodes.IndexOf(endNode);
        
        _edges.RemoveAll(x => x.StartNode == startNodeIndex && x.EndNode == endNodeIndex);
    }

    public List<(GraphNode node, float weight)> GetNeighbours(GraphNode node)
    {
        if (!_nodes.Contains(node))
        {
            return new List<(GraphNode, float)>();
        }
        
        var result = new List<(GraphNode, float)>();
        var nodeIndex = _nodes.IndexOf(node);
        
        foreach (var edge in _edges)
        {
            if (edge.StartNode == nodeIndex)
            {
                result.Add((_nodes[edge.EndNode], edge.Weight));
            }
        }

        return result;
    }

    public List<float3> GetPathTo(float3 start, float3 end)
    {
        if(!_nodes.Contains(start))
        {
            return new List<float3>();
        }
        
        if(!_nodes.Contains(end))
        {
            return new List<float3>();
        }

        var nodesToEvaluate = _nodes.ToList();

        var previous = _nodes.ToDictionary<GraphNode, GraphNode, GraphNode>(node => node, _ => null);
        var distance = _nodes.ToDictionary(node => node, _ => float.MaxValue);

        distance[start] = 0;

        while (nodesToEvaluate.Count > 0)
        {
            nodesToEvaluate.Sort((x, y) =>
            {
                var xValue = distance[x] + math.distancesq(x, end);
                var yValue = distance[y] + math.distancesq(y, end);

                return xValue.CompareTo(yValue);
            });
            var evaluatingNode = nodesToEvaluate[0];
            nodesToEvaluate.RemoveAt(0);

            if (evaluatingNode.Equals(end))
            {
                break;
            }

            foreach (var neighbour in GetNeighbours(evaluatingNode))
            {
                var newDistance = distance[evaluatingNode] + neighbour.weight;
                if (newDistance >= distance[neighbour.node])
                {
                    continue;
                }

                distance[neighbour.node] = newDistance;
                previous[neighbour.node] = evaluatingNode;
            }
        }

        if (distance[end] == int.MaxValue)
        {
            return new List<float3>();
        }

        var result = new List<float3>();

        GraphNode reversingNode = end;
        while (reversingNode != null)
        {
            result.Add(reversingNode);
            reversingNode = previous[reversingNode];
        }

        result.Reverse();
        return result;
    }

    public float3 GetNearestGraphPoint(float3 position)
    {
        var nearest = _nodes[0];
        var nearestDistance = math.distancesq(_nodes[0], position);
        for (var i = 1; i < _nodes.Count; i++)
        {
            var possibleDistance = math.distancesq(_nodes[i], position);
            if (possibleDistance < nearestDistance)
            {
                nearestDistance = possibleDistance;
                nearest = _nodes[i];
            }
        }

        return nearest;
    }
}