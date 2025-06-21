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
    [SerializeField]
    protected int _weight = 0;

    public virtual int Weight => _weight;
    public abstract SteeringOutput GetSteering(Agent agent);
    protected LayerMask AgentLayer;
}