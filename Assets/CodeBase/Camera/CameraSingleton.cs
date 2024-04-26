using Unity.Mathematics;
using UnityEngine;

namespace Assets.CodeBase.Camera
{
    public class CameraSingleton : MonoBehaviour
    {
        private static CameraSingleton _instance;

        public static CameraSingleton Instance => _instance;

        [SerializeField] private float3 _lookOffset = float3.zero;
        [SerializeField] private float _smoothTime = 0.2f;

        private Vector3 _smoothVelocity = Vector3.zero;

        private float3 _targetPosition;
        public float3 TargetPosition { set { _targetPosition = value; } }

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void LateUpdate() {
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition + _lookOffset, ref _smoothVelocity, _smoothTime);
        }
    }
}