using UnityEngine;

namespace Assets.CodeBase.Infrastructure
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private GameBootstrapper _bootstrapPrefab;

        private void Awake() {
            GameBootstrapper bootstrapper = FindFirstObjectByType<GameBootstrapper>();

            if (bootstrapper == null)
                Instantiate(_bootstrapPrefab);

            Destroy(gameObject);
        }
    }
}
