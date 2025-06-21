using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class AvoidAgentSteeringBehaviour : SteeringBehaviour
{
    private const float PARALLEL_RATIO = 0.8f;

    [SerializeField] private float _neighbourRange = 3;

    [SerializeField] private float _maxAcceleration = 100;

    private Agent[] FindNeighbours(Agent agent)
    {
        return Object.FindObjectsOfType<Agent>().Where(x =>
                x != agent && math.distancesq(agent.Position, x.Position) <=
                _neighbourRange * _neighbourRange)
            .ToArray();
    }

    public override SteeringOutput GetSteering(Agent agent)
    {
        float? imminentCollisionTime = null;
        SteeringOutput? imminentAvoidance = null;
        foreach (var neighbour in FindNeighbours(agent))
        {
            if (!EvaluateAvoidanceFor(agent, neighbour, out var timeToCollision, out var avoidance))
            {
                continue;
            }

            if (imminentCollisionTime != null && !(imminentCollisionTime > timeToCollision))
            {
                continue;
            }

            imminentCollisionTime = timeToCollision;
            imminentAvoidance = avoidance;
        }

        return imminentAvoidance.HasValue
            ? imminentAvoidance.Value
            : new SteeringOutput { Angular = 0, Linear = float3.zero };
    }

    private bool EvaluateAvoidanceFor(Agent agent, Agent target, out float timeToCollision,
        out SteeringOutput avoidance)
    {
        avoidance = new SteeringOutput();
        var relativePosition = target.Position - agent.Position;
        var relativeVelocity = target.LinearVelocity - agent.LinearVelocity;
        var relativeSpeedSquared = math.lengthsq(relativeVelocity);

        timeToCollision = math.dot(relativePosition, relativePosition) / relativeSpeedSquared;
        var distance = math.length(relativePosition);
        if (distance - math.sqrt(relativeSpeedSquared) > agent.Radius + target.Radius)
        {
            return false;
        }

        if (timeToCollision <= 0)
        {
            return false;
        }

        var avoidanceDirection = -math.normalizesafe(relativePosition);

        if (math.abs(math.dot(relativeVelocity, agent.LinearVelocity)) > PARALLEL_RATIO)
        {
            avoidanceDirection = math.cross(math.normalizesafe(agent.LinearVelocity), math.up());
        }

        avoidance.Linear = avoidanceDirection * _maxAcceleration;
        return true;
    }
}