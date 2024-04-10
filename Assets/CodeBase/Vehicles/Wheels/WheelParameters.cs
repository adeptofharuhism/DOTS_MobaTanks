using UnityEngine;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [CreateAssetMenu(menuName = Constants.ScriptableObjectsNames.WheelParameters)]
    public class WheelParameters : ScriptableObject
    {
        public float SpringRestDistance;
        public float WheelDiameter;
    }
}