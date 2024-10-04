using System;
using Zenject;

namespace Assets.CodeBase.UI.MainScene
{
    public enum MainSceneMode
    {
        Loading,
        Preparing,
        InGame,
        GameOver
    }

    public interface IMainSceneViewModel
    {

    }

    public class MainSceneViewModel : IMainSceneViewModel, IInitializable, IDisposable
    {
        public MainSceneViewModel() {

        }

        public void Initialize() {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
