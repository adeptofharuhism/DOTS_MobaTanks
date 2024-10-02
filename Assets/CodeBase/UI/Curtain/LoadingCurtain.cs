using UnityEngine;

namespace Assets.CodeBase.UI.Curtain
{
    public class LoadingCurtain : MonoBehaviour, ILoadingCurtain
    {
        private void Awake() =>
            DontDestroyOnLoad(gameObject);

        public void Show() =>
            gameObject.SetActive(true);

        public void Hide() =>
            gameObject.SetActive(false);
    }
}
