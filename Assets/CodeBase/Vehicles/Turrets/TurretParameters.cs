using UnityEngine;

namespace Assets.CodeBase.Vehicles.Turrets
{
    [CreateAssetMenu(menuName = Constants.ScriptableObjectsNames.TurretParameters)]
    public class TurretParameters : ScriptableObject
    {
        [SerializeField] private GameObject _turretModelPrefab;
        [SerializeField] private GameObject _turretWeaponPrefab;

        public GameObject TurretModelPrefab => _turretModelPrefab;
        public GameObject TurretWeaponPrefab => _turretWeaponPrefab;
    }
}
