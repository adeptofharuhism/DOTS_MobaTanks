using Assets.CodeBase.UI.MainScene;
using UnityEngine;
using Zenject;

namespace Assets.CodeBase.Infrastructure.Installers
{
    public class MainSceneUiInstaller : MonoInstaller
    {
        [SerializeField] private MainSceneView _mainSceneView;

        public override void InstallBindings() {

        }
    }
}
