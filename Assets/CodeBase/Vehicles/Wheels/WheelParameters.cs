using UnityEngine;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [CreateAssetMenu(menuName = Constants.ScriptableObjectsNames.WheelParameters)]
    public class WheelParameters : ScriptableObject
    {
        [Header("Spring")]
        public float SpringRestDistance;
        public float WheelDiameter;
        public float SpringStrength;
        public float SpringDamper;
        [Header("Rotation")]
        public float MaxRotationAngle;
    }
}