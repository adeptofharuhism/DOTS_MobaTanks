﻿using Assets.CodeBase.Infrastructure.GameStateManagement;
using Assets.CodeBase.Infrastructure.Services;
using Assets.CodeBase.Infrastructure.Services.ConnectionInfo;
using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Infrastructure.Services.SceneLoader;
using Assets.CodeBase.Infrastructure.Services.WorldAccess;
using Assets.CodeBase.Infrastructure.Services.WorldCommandSender;
using Assets.CodeBase.Infrastructure.Services.WorldControl;
using Assets.CodeBase.Infrastructure.Services.WorldEvents;
using Assets.CodeBase.UI.Curtain;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Assets.CodeBase.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller, ICoroutineRunner
    {
        [SerializeField] private LoadingCurtain _loadingCurtain;

        public override void InstallBindings() {
#if UNITY_EDITOR
            SceneManager.LoadScene(0);
            Debug.Log("Installing dependencies");
#endif
            RegisterMainSceneModeNotifier();
            RegisterConnectionInfo();

            RegisterWorldAccessService();
            RegisterWorldRpcSenderService();
            RegisterWorldEventService();
            RegisterWorldControlService();

            RegisterCoroutineRunner();
            RegisterLoadingCurtain();
            RegisterSceneLoader();
            RegisterStateMachine();
        }

        private void RegisterWorldRpcSenderService() =>
            Container
                .Bind<IWorldRpcSenderService>()
                .To<WorldRpcSenderService>()
                .FromNew()
                .AsSingle()
                .NonLazy();

        private void RegisterMainSceneModeNotifier() =>
            Container
                .Bind<IMainSceneModeNotifier>()
                .To<MainSceneModeNotifier>()
                .FromNew()
                .AsSingle()
                .NonLazy();

        private void RegisterConnectionInfo() =>
            Container
                .Bind<IConnectionInfoService>()
                .To<ConnectionInfoService>()
                .FromNew()
                .AsSingle()
                .NonLazy();

        private void RegisterWorldAccessService() =>
            Container
                .Bind<IWorldAccessService>()
                .To<WorldAccessService>()
                .FromNew()
                .AsSingle()
                .NonLazy();

        private void RegisterWorldEventService() =>
            Container
                .BindInterfacesTo<WorldEventService>()
                .FromNew()
                .AsSingle()
                .NonLazy();

        private void RegisterWorldControlService() =>
            Container
                .Bind<IWorldControlService>()
                .To<WorldControlService>()
                .FromNew()
                .AsSingle()
                .NonLazy();

        private void RegisterCoroutineRunner() =>
            Container
                .Bind<ICoroutineRunner>()
                .FromInstance(this)
                .AsSingle()
                .NonLazy();

        private void RegisterLoadingCurtain() {
            LoadingCurtain curtain = Instantiate(_loadingCurtain);

            Container
                .Bind<ILoadingCurtain>()
                .FromInstance(curtain)
                .AsSingle()
                .NonLazy();
        }

        private void RegisterSceneLoader() =>
            Container
                .Bind<ISceneLoader>()
                .To<SceneLoader>()
                .FromNew()
                .AsSingle()
                .NonLazy();

        private void RegisterStateMachine() =>
            Container
                .BindInterfacesTo<GameStateMachine>()
                .FromNew()
                .AsSingle()
                .NonLazy();
    }
}
