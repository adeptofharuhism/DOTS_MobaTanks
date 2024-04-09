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

        public int WheelIndex { get => _wheelIndex; set => _wheelIndex = value; }

        public GameObject WheelModel => _wheelModel;
        public GameObject ForceApplicationPoint => _forceApplicationPoint;
        public bool HasAcceleration => _hasAcceleration;
        public WheelRotationType RotationType => _rotationType;

        public class WheelBaker : Baker<WheelAuthoring>
        {
            public override void Bake(WheelAuthoring authoring) {

            }
        }
    }
}