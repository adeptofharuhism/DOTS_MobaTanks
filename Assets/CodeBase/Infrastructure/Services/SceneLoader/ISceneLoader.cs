using System;
using UnityEngine.SceneManagement;

namespace Assets.CodeBase.Infrastructure.Services.SceneLoader
{
    public interface ISceneLoader
    {
        void Load(string sceneName, LoadSceneMode loadSceneMode, Action onLoaded);
    }
}