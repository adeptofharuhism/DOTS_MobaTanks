using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.CodeBase.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        private void Awake() {
            DontDestroyOnLoad(gameObject);

            SceneManager.LoadScene(0);
        }
    }
}
