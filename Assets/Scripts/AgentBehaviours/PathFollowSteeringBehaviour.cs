using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class PathFollowSteeringBehaviour : SteeringBehaviour
{
    [SerializeField]
    private float _maxAcceleration = 100;

    [SerializeField]
    private Path _pathToFollow;

    [SerializeField]
    private float _reachRadius = 1;

    [SerializeField]
    private float _timeToTarget = 0.1f;

    private int _currentIndex = -1;

    private void IncreaseIndex()
    {
        _currentIndex = (_currentIndex + 1) % _pathToFollow.Nodes.Length;
    }

    public override SteeringOutput GetSteering(Agent agent)
    {
        if (_currentIndex < 0)
        {
            _currentIndex = _pathToFollow.GetNearestPoint(agent.Position);
        }

        if (math.distance(_pathToFollow.GetPointPosition(_currentIndex), agent.Position) <= _reachRadius)
        {
            IncreaseIndex();
        }

        var targetPosition = (float3)_pathToFollow.Nodes[_currentIndex].position;

        //Arrive
        var distance = math.distance(agent.Position, targetPosition);

        var targetSpeed = agent.MaxLinearSpeed;

        if (distance <= _reachRadius)
        {
            targetSpeed = 0;
        }

        var direction = math.normalizesafe(targetPosition - agent.Position);
        var targetVelocity = direction * targetSpeed;

        var result = new SteeringOutput { Linear = targetVelocity - agent.LinearVelocity };
        result.Linear /= _timeToTarget;

        result.Linear = math.normalizesafe(result.Linear) * math.min(math.length(result.Linear), _maxAcceleration);
        result.Angular = 0;

        return result;
    }
}