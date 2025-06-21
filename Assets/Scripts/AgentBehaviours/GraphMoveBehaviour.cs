using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class GraphMoveBehaviour : SteeringBehaviour
{
    [SerializeField] private float maxAcceleration = 100;
    [SerializeField] private GraphVolume worldGraph;
    [SerializeField] private float reachRadius = 1;
    [SerializeField] private float timeToTarget = 0.1f;

    private int _currentIndex = -1;
    [SerializeField] private bool recalculatePath;
    public bool RecalculatePathVar
    {
        get => recalculatePath;
        set => recalculatePath = value;
    }

    [SerializeField] private Transform target;
    private List<float3> _path;

    public override SteeringOutput GetSteering(Agent agent)
    {
        if (recalculatePath)
        {
            RecalculatePath(agent);
            recalculatePath = false;
        }

        if (_path == null || _path.Count == 0 || _currentIndex < 0 || _currentIndex >= _path.Count)
        {
            return new SteeringOutput();
        }

        if (math.distance(_path[_currentIndex], agent.Position) <= reachRadius)
        {
            _currentIndex = math.clamp(_currentIndex + 1, 0, _path.Count - 1);
        }

        var targetPosition = (float3)_path[_currentIndex];

        var distance = math.distance(agent.Position, targetPosition);
        var targetSpeed = agent.MaxLinearSpeed;

        if (distance <= reachRadius)
        {
            targetSpeed = 0;
        }

        var direction = math.normalizesafe(targetPosition - agent.Position);
        var targetVelocity = direction * targetSpeed;

        var result = new SteeringOutput { Linear = targetVelocity - agent.LinearVelocity };
        result.Linear /= timeToTarget;
        result.Linear = math.normalizesafe(result.Linear) * math.min(math.length(result.Linear), maxAcceleration);
        result.Angular = 0;

        return result;
    }

    private void RecalculatePath(Agent agent)
    {
        var startNode = worldGraph.Graph.GetNearestGraphPoint(agent.Position);
        var endNode = worldGraph.Graph.GetNearestGraphPoint(target.position);

        var graphPath = worldGraph.Graph.GetPathTo(startNode, endNode);

        graphPath.Add(target.position);
        _path = graphPath;
        _currentIndex = 0;
    }
}