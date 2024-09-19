using Assets.CodeBase.Infrastructure.Services.SceneLoader;
using Assets.CodeBase.Infrastructure.StateMachine;
using Zenject;

namespace Assets.CodeBase.Infrastructure
{
    public class BootstrapInstaller : MonoInstaller, ICoroutineRunner
    {
        public override void InstallBindings() {
#if UNITY_EDITOR
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            UnityEngine.Debug.Log("Installing dependencies");
#endif
            RegisterCoroutineRunner();
            RegisterSceneLoader();
            RegisterStateMachine();
        }

        private void RegisterCoroutineRunner() =>
            Container
                .Bind<ICoroutineRunner>()
                .FromInstance(this)
                .AsSingle()
                .NonLazy();

        private void RegisterSceneLoader() {
            Container
                .Bind<ISceneLoader>()
                .To<SceneLoader>()
                .FromNew()
                .AsSingle()
                .NonLazy();
        }

        private void RegisterStateMachine() {
            Container
                .BindInterfacesTo<GameStateMachine>()
                .FromNew()
                .AsSingle()
                .NonLazy();
        }
    }
}
