using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class LookWhereYouMoveSteeringBehaviour : SteeringBehaviour
{
    [SerializeField] private float _acceptanceRange = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _velocityThreshold = 0.1f;

    public override SteeringOutput GetSteering(Agent agent)
    {
        var speed = math.length(agent.LinearVelocity);

        if (speed < _velocityThreshold)
        {
            return new SteeringOutput { Angular = -agent.AngularVelocity * 0.9f };
        }

        var targetOrientation = math.atan2(agent.LinearVelocity.x, agent.LinearVelocity.z);
        var delta = MapToRange(targetOrientation - agent.Orientation);

        var dynamicAcceptanceRange = math.radians(_acceptanceRange);

        if (math.abs(delta) < dynamicAcceptanceRange)
        {
            return new SteeringOutput { Angular = -agent.AngularVelocity * 0.8f };
        }

        var targetRotation = math.clamp(delta * _rotationSpeed, -agent.MaxAngularSpeed, agent.MaxAngularSpeed);
        var smoothedRotation = math.lerp(agent.AngularVelocity, targetRotation, 0.15f);

        return new SteeringOutput
        {
            Linear = 0,
            Angular = smoothedRotation
        };
    }

    private float MapToRange(float angle)
    {
        while (angle > math.PI) angle -= 2 * math.PI;
        while (angle < -math.PI) angle += 2 * math.PI;
        return angle;
    }
}