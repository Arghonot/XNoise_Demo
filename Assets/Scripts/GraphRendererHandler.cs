using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class GraphRendererHandler : MonoBehaviour
    {
        // use render tex directly to speed up process
        private Texture2D output;
        [SerializeField] private Material _3dObjectMaterial;

        private void Awake()
        {
            ShortcutHandler.OnPressedG += SimpleEventReceivedHandler;
            GraphVariableFieldUI.OnValueChanged += VariableFieldUIEventReceivedHandler;
            UIManager.SelectedTextureSizeIndexChanged += IntEventReceivedHandler;
            UIManager.SelectedGraphIndexChanged += IntEventReceivedHandler;
            UIManager.SelectedProjectionTypeIndexChanged += IntEventReceivedHandler;
            UIManager.SelectedGradiantIndexChanged += IntEventReceivedHandler;
            UIManager.PlusOneStateChanged += BoolEventReceivedHandler;
            UIManager.RandomSeedStateChanged += BoolEventReceivedHandler;
        }


        private void OnDestroy()
        {
            ShortcutHandler.OnPressedG -= SimpleEventReceivedHandler;
            GraphVariableFieldUI.OnValueChanged -= VariableFieldUIEventReceivedHandler;
            UIManager.SelectedTextureSizeIndexChanged -= IntEventReceivedHandler;
            UIManager.SelectedGraphIndexChanged -= IntEventReceivedHandler;
            UIManager.SelectedProjectionTypeIndexChanged -= IntEventReceivedHandler;
            UIManager.SelectedGradiantIndexChanged -= IntEventReceivedHandler;
            UIManager.PlusOneStateChanged -= BoolEventReceivedHandler;
            UIManager.RandomSeedStateChanged -= BoolEventReceivedHandler;
        }

        private void SimpleEventReceivedHandler() => this.enabled = true;
        private void BoolEventReceivedHandler(bool obj) => this.enabled = true;
        private void IntEventReceivedHandler(int obj) => this.enabled = true;
        private void VariableFieldUIEventReceivedHandler(string arg1, object arg2) => this.enabled = true;

        private void LateUpdate()
        {
            this.enabled = false;
            UpdateRender();
        }

        private void UpdateRender()
        {
            var graph = GraphLibrary.CurrentGraph;

            graph.renderer.input = graph.GetGenerator();
            graph.renderer.width = (int)TextureSizeHandler.CurrentResolution.x;
            graph.renderer.Height = (int)TextureSizeHandler.CurrentResolution.y;
            graph.renderer.projectionMode = ProjectionHandler.ProjectionType;

            graph.renderer.RenderGPU();
            _3dObjectMaterial.SetTexture("_Input", graph.renderer.rtex);
        }
    }
}