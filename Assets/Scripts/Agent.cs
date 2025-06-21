using System;
using Unity.Mathematics;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] private float _maxLinearSpeed = 10;

    [SerializeField] private float _maxAngularSpeed = 360f;

    [SerializeField] private float _radius = .5f;

    [SerializeReference, SubclassSelector] private SteeringBehaviour[] _steering = Array.Empty<SteeringBehaviour>();

    public float MaxLinearSpeed => _maxLinearSpeed;
    public float MaxAngularSpeed => _maxAngularSpeed;
    public float Radius => _radius;

    public float3 LinearVelocity { get; private set; }
    public float AngularVelocity { get; private set; }

    public float3 Position
    {
        get => transform.position;
        private set => transform.position = value;
    }

    public float Orientation
    {
        get => math.radians(transform.eulerAngles.y);
        private set => transform.rotation = quaternion.Euler(0, value, 0);
    }

    private GraphMoveBehaviour _graphMoveBehaviour;

    private void Start()
    {
        // DEBUG PORPUSE
        foreach (var sb in _steering)
        {
            if (sb is GraphMoveBehaviour)
            {
                _graphMoveBehaviour = sb as GraphMoveBehaviour;
            }
        }
    }

    private void Update()
    {
        // DEBUG PORPUSE
        if (Input.GetKeyDown(KeyCode.F))
        {
            _graphMoveBehaviour.RecalculatePathVar = true;
        }
    }

    private void FixedUpdate()
    {
        float3 totalLinear = float3.zero;
        float totalAngular = 0f;
        int maxPriority = int.MinValue;

        foreach (var currentSteering in _steering)
        {
            var steering = currentSteering.GetSteering(this);
            totalLinear += steering.Linear;

            // Applica solo l'Angular del behaviour con priorità più alta
            if (currentSteering.Weight > maxPriority && math.abs(steering.Angular) > 0.0001f)
            {
                totalAngular = steering.Angular;
                maxPriority = currentSteering.Weight;
            }
        }

        LinearVelocity += totalLinear * Time.fixedDeltaTime;
        AngularVelocity += totalAngular * Time.fixedDeltaTime;

        LinearVelocity = math.normalizesafe(LinearVelocity) * math.min(math.length(LinearVelocity), MaxLinearSpeed);
        AngularVelocity = math.clamp(math.abs(AngularVelocity), 0, MaxAngularSpeed) * math.sign(AngularVelocity);

        Position += LinearVelocity * Time.fixedDeltaTime;
        Orientation += AngularVelocity * Time.fixedDeltaTime;
    }
}