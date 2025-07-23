using UnityEngine;
using XNoise;

namespace XNoise_DemoWebglPlayer
{
    public class GraphRendererHandler : MonoBehaviour
    {
        public static Material OriginalRenderGradient2d;
        [SerializeField] private Material[] _materials;

        private void Awake()
        {
            // the first is for 2d and can be used to render the final image
            OriginalRenderGradient2d = _materials[0];
            GraphVariableFieldUI.OnValueChanged += VariableFieldUIEventReceivedHandler;
            UIManager.SelectedTextureSizeIndexChanged += IntEventReceivedHandler;
            UIManager.SelectedGraphIndexChanged += IntEventReceivedHandler;
            UIManager.SelectedProjectionTypeIndexChanged += IntEventReceivedHandler;
            UIManager.SelectedGradiantIndexChanged += IntEventReceivedHandler;
            UIManager.PlusOneStateChanged += BoolEventReceivedHandler;
            UIManager.RandomSeedStateChanged += BoolEventReceivedHandler;
            TextureSizeHandler.OnTextureSizeChanged += SimpleEventReceivedHandler;
        }

        private void OnDestroy()
        {
            GraphVariableFieldUI.OnValueChanged -= VariableFieldUIEventReceivedHandler;
            UIManager.SelectedTextureSizeIndexChanged -= IntEventReceivedHandler;
            UIManager.SelectedGraphIndexChanged -= IntEventReceivedHandler;
            UIManager.SelectedProjectionTypeIndexChanged -= IntEventReceivedHandler;
            UIManager.SelectedGradiantIndexChanged -= IntEventReceivedHandler;
            UIManager.PlusOneStateChanged -= BoolEventReceivedHandler;
            UIManager.RandomSeedStateChanged -= BoolEventReceivedHandler;
            TextureSizeHandler.OnTextureSizeChanged -= SimpleEventReceivedHandler;
        }

        private void SimpleEventReceivedHandler() => this.enabled = true;
        private void BoolEventReceivedHandler(bool obj) => this.enabled = true;
        private void IntEventReceivedHandler(int obj) => this.enabled = true;
        private void VariableFieldUIEventReceivedHandler(string arg1, object arg2)
        {
            var graph = GraphLibrary.CurrentGraph;
            this.enabled = true;
        }

        private void LateUpdate()
        {
            this.enabled = false;
            UpdateRender();
        }

        private void UpdateRender()
        {
            var graph = GraphLibrary.CurrentGraph;

            graph.renderer.input = graph.Run(GraphArgumentsHandler.currentStorage) as INoiseStrategy;
            graph.renderer.width = (int)TextureSizeHandler.CurrentResolution.x;
            graph.renderer.Height = (int)TextureSizeHandler.CurrentResolution.y;
            graph.renderer.projectionMode = (XNoise.Renderer.ProjectionMode)ProjectionHandler.ProjectionType;
            graph.renderer.renderMode = XNoise.Renderer.RenderMode.GPU;
            graph.renderer.Render();

            UpdateAllMaterials(graph.renderer.rtex);
        }

        private void UpdateAllMaterials(RenderTexture rtex)
        {
            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].SetTexture("_MainTex", rtex);
                _materials[i].SetTexture("_Gradient", GradiantHandler.CurrentGradient.texture);
            }
        }
    }
}