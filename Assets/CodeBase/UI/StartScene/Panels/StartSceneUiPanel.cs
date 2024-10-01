using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.StartScene.Panels
{
    public abstract class StartSceneUiPanel : UiPanel
    {
        protected readonly IStartSceneViewModel _startSceneViewModel;

        protected StartSceneUiPanel(VisualTreeAsset panelAsset, IStartSceneViewModel startSceneViewModel)
            : base(panelAsset) {

            _startSceneViewModel = startSceneViewModel;
        }
    }
}
