using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class AvoidWallSteeringBehaviour : SteeringBehaviour
{
    [SerializeField] private float maxAcceleration = 100;

    [SerializeField] private float rayLength = 1;

    [SerializeField, Range(0, 180)] private float rayAngle = 45;

    [SerializeField] private LayerMask collisionMask;

    public override SteeringOutput GetSteering(Agent agent)
    {
        if (agent.LinearVelocity.Equals(float3.zero))
        {
            return new SteeringOutput();
        }

        var movementOrientation = math.atan2(agent.LinearVelocity.x, agent.LinearVelocity.z);

        Debug.DrawRay(agent.Position, math.normalizesafe(agent.LinearVelocity), Color.white);

        var leftDirection = math.mul(quaternion.Euler(0, movementOrientation - math.radians(rayAngle), 0),
            math.forward());
        var rightDirection = math.mul(quaternion.Euler(0, movementOrientation + math.radians(rayAngle), 0),
            math.forward());

        var leftCollisionDistance = float.MaxValue;
        float3? leftCollisionNormal = null;

        var rightCollisionDistance = float.MaxValue;
        float3? rightCollisionNormal = null;

        Debug.DrawRay(agent.Position, leftDirection * rayLength, Color.red);
        if (Physics.Raycast(agent.Position, leftDirection, out var leftHit, rayLength, collisionMask))
        {
            leftCollisionDistance = leftHit.distance;
            leftCollisionNormal = leftHit.normal;
        }

        Debug.DrawRay(agent.Position, rightDirection * rayLength, Color.yellow);
        if (Physics.Raycast(agent.Position, rightDirection, out var rightHit, rayLength, collisionMask))
        {
            rightCollisionDistance = rightHit.distance;
            rightCollisionNormal = rightHit.normal;
        }

        var avoidNormal = float3.zero;

        if (leftCollisionNormal != null && rightCollisionNormal != null)
        {
            avoidNormal = leftCollisionDistance > rightCollisionDistance
                ? leftCollisionNormal.Value
                : rightCollisionNormal.Value;
        }
        else if (leftCollisionNormal != null)
        {
            avoidNormal = leftCollisionNormal.Value;
        }
        else if (rightCollisionNormal != null)
        {
            avoidNormal = rightCollisionNormal.Value;
        }
        else
        {
            return new SteeringOutput();
        }

        return new SteeringOutput { Linear = avoidNormal * maxAcceleration };
    }
}