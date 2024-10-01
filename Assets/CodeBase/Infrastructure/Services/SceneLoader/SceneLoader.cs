using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.CodeBase.Infrastructure.Services.SceneLoader
{
    public class SceneLoader : ISceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public SceneLoader(ICoroutineRunner coroutineRunner) {
            _coroutineRunner = coroutineRunner;
        }

        public void Load(string sceneName, LoadSceneMode loadSceneMode, Action onLoaded) {
            _coroutineRunner.StartCoroutine(LoadScene(sceneName, loadSceneMode, onLoaded));
        }

        private IEnumerator LoadScene(string sceneName, LoadSceneMode loadSceneMode, Action onLoaded) {
            AsyncOperation asyncOperation =
                SceneManager.LoadSceneAsync(sceneName);

            while (!asyncOperation.isDone)
                yield return null;

            onLoaded?.Invoke();
        }
    }
}
