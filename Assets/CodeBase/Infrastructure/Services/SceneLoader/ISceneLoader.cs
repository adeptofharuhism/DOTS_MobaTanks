using System;

namespace Assets.CodeBase.Infrastructure.Services.SceneLoader
{
    public interface ISceneLoader
    {
        void Load(string sceneName, Action onLoaded);
    }
}