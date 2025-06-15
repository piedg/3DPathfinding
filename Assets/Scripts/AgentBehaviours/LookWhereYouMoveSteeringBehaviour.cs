using System;
using Unity.Mathematics;
using UnityEngine;


[Serializable]
public class LookWhereYouMoveSteeringBehaviour : SteeringBehaviour
{
    [SerializeField] private float acceptanceRange = 5;

    public override SteeringOutput GetSteering(Agent agent)
    {
        var targetOrientation = math.atan2(agent.LinearVelocity.x, agent.LinearVelocity.z);
        var delta = MapToRange(targetOrientation) - MapToRange(agent.Orientation);

        var targetAngularSpeed = delta;

        var direction = math.sign(delta);
        var targetAngularVelocity = direction * targetAngularSpeed;

        var result = new SteeringOutput
        {
            Linear = 0, Angular = (targetAngularVelocity - agent.AngularVelocity) / Time.fixedDeltaTime
        };

        Debug.DrawRay(agent.Position, math.mul(quaternion.Euler(0, targetOrientation, 0), math.forward()), Color.cyan);

        result.Angular = math.sign(result.Angular) * math.clamp(math.abs(result.Angular), 0, agent.MaxAngularSpeed);
        return result;
    }

    private float MapToRange(float angle)
    {
        const float twoPi = 2 * math.PI;
        while (angle < 0)
        {
            angle += twoPi;
        }

        while (angle > twoPi)
        {
            angle -= twoPi;
        }

        return angle;
    }
}