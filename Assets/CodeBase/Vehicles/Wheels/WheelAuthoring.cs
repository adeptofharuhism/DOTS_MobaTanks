using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.CodeBase.Vehicles.Wheels
{
    public class WheelAuthoring : MonoBehaviour
    {
        [SerializeField] private int _wheelIndex;

        [SerializeField] private GameObject _wheelModel;
        [SerializeField] private GameObject _forceApplicationPoint;
        [SerializeField] private bool _hasAcceleration;
        [SerializeField] private WheelRotationType _rotationType;
        [SerializeField] private WheelParameters _wheelParameters;

        public int WheelIndex { get => _wheelIndex; set => _wheelIndex = value; }

        public GameObject WheelModel => _wheelModel;
        public GameObject ForceCastPoint => _forceApplicationPoint;
        public bool HasAcceleration => _hasAcceleration;
        public WheelRotationType RotationType => _rotationType;
        public WheelParameters WheelParameters => _wheelParameters;

        public class WheelBaker : Baker<WheelAuthoring>
        {
            public override void Bake(WheelAuthoring authoring) {
                Entity wheel = GetEntity(TransformUsageFlags.Dynamic);

                AddCommonWheelComponents(wheel, authoring);

                if (authoring.RotationType != WheelRotationType.None)
                    AddRotatingWheelComponents(wheel, authoring);

                if (authoring.HasAcceleration)
                    AddAcceleratedWheelComponents(wheel, authoring);
            }

            private void AddCommonWheelComponents(Entity wheel, WheelAuthoring authoring) {
                AddComponent<NewWheelTag>(wheel);
                AddComponent(wheel, new WheelIndex { Value = authoring.WheelIndex });
                AddComponent(wheel, new WheelForceCastPoint { Value = GetEntity(authoring.ForceCastPoint, TransformUsageFlags.Dynamic) });
                AddComponent(wheel, new WheelModelParameters {
                    ModelContainer = GetEntity(authoring.WheelModel, TransformUsageFlags.Dynamic),
                    Diameter = authoring.WheelParameters.WheelDiameter
                });

                AddComponent(wheel, new WheelSpringRestDistance { Value = authoring.WheelParameters.SpringRestDistance });
                AddComponent(wheel, new WheelSpringCompression { Value = 0 });
                AddComponent(wheel, new WheelSpringStrength {
                    Damper = authoring.WheelParameters.SpringDamper,
                    Strength = authoring.WheelParameters.SpringStrength
                });

                AddComponent(wheel, new WheelSteeringParameters {
                    MaximalSteering = authoring.WheelParameters.MaxSteeringTraction,
                    MinimalSteering = authoring.WheelParameters.MinSteeringTraction,
                });

                AddComponent(wheel, new WheelLinearVelocity { Value = float3.zero });
                AddComponent(wheel, new WheelAxisProjectedVelocity { Value = float3.zero });

                AddComponent(wheel, new WheelAxisForceSpring { Value = float3.zero });
                AddComponent(wheel, new WheelAxisForceSteering { Value = float3.zero });
                AddComponent(wheel, new WheelAxisForceAcceleration { Value = float3.zero });

                AddComponent<WheelBrakingTag>(wheel);
            }

            private void AddRotatingWheelComponents(Entity wheel, WheelAuthoring authoring) {
                AddComponent<WheelHasRotationTag>(wheel);
                AddComponent(wheel, new WheelRotationInput { Value = 0 });
                AddComponent(wheel, new WheelRotationParameters {
                    MaxRotationAngle = authoring.WheelParameters.MaxRotationAngle,
                    RotatesClockwise = authoring.RotationType == WheelRotationType.Clockwise ? true : false
                });
            }

            private void AddAcceleratedWheelComponents(Entity wheel, WheelAuthoring authoring) {
                AddComponent<WheelHasAccelerationTag>(wheel);
                AddComponent(wheel, new WheelAccelerationInput { Value = 0 });
                AddComponent(wheel, new WheelAccelerationParameters {
                    MaxVelocity = authoring.WheelParameters.MaxVelocity,
                    MaxVelocityBackwards = authoring.WheelParameters.MaxVelocityBackwards,
                    EngineForceMultiplier = authoring.WheelParameters.EngineForceMultiplier,
                    HardBrakingForceMultiplier = authoring.WheelParameters.HardBrakingForceMultiplier
                });
            }
        }
    }
}