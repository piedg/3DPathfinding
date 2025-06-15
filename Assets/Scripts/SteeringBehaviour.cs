using System;
using Unity.Mathematics;
using UnityEngine;

public struct SteeringOutput
{
    public float3 Linear;
    public float Angular;
}

[Serializable]
public abstract class SteeringBehaviour
{
    public abstract SteeringOutput GetSteering(Agent agent);
    protected LayerMask AgentLayer;
}