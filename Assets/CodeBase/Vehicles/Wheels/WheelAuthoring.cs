using Unity.Entities;
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
                AddRotatingWheelComponents(wheel, authoring);
                AddAcceleratedWheelComponents(wheel, authoring);
            }

            private void AddCommonWheelComponents(Entity wheel, WheelAuthoring authoring) {
                AddComponent(wheel, new WheelIndex { Value = authoring.WheelIndex });
                AddComponent(wheel, new WheelForceCastPoint { Value = GetEntity(authoring.ForceCastPoint, TransformUsageFlags.Dynamic) });
                AddComponent(wheel, new WheelModelParameters {
                    ModelContainer = GetEntity(authoring.WheelModel, TransformUsageFlags.Dynamic),
                    Diameter = authoring.WheelParameters.WheelDiameter
                });

                AddComponent(wheel, new WheelHasGroundContact { Value = false });
                AddComponent(wheel, new WheelSpringRestDistance { Value = authoring.WheelParameters.SpringRestDistance });
                AddComponent(wheel, new WheelCompressedSpringLength { Value = authoring.WheelParameters.SpringRestDistance });
            }

            private void AddRotatingWheelComponents(Entity wheel, WheelAuthoring authoring) {

            }

            private void AddAcceleratedWheelComponents(Entity wheel, WheelAuthoring authoring) {

            }
        }
    }
}