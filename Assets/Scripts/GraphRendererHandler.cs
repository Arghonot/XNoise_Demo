using UnityEngine;
using XNoise;
using static XNoise.Renderer;

namespace XNoise_DemoWebglPlayer
{
    public class GraphRendererHandler : MonoBehaviour
    {
        [SerializeField] private Material[] _materials;

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
            UIManager.TextureHeightOrWidthChanged += SimpleEventReceivedHandler;

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
            UIManager.TextureHeightOrWidthChanged -= SimpleEventReceivedHandler;
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

            graph.runtimeStorage = GraphArgumentsHandler.currentStorage;
            graph.renderer.input = graph.GetGenerator() as INoiseStrategy;
            graph.renderer.width = (int)TextureSizeHandler.CurrentResolution.x;
            graph.renderer.Height = (int)TextureSizeHandler.CurrentResolution.y;
            graph.renderer.projectionMode = (ProjectionMode)ProjectionHandler.ProjectionType;
            graph.renderer.renderMode = XNoise.Renderer.RenderMode.GPU;

            Debug.Log($"<color=red>+------------------------------------ EDITED -----------------------------------+</color>");
            GraphArgumentsHandler.currentStorage.DebugDictionnaryInDepth();
            Debug.Log($"<color=red>+------------------------------------ RUNTIME -----------------------------------+</color>");
            graph.runtimeStorage.DebugDictionnaryInDepth();
            Debug.Log($"<color=red>+------------------------------------ ORIGINAL -----------------------------------+</color>");
            graph.originalStorage.DebugDictionnaryInDepth();
            Debug.Log($"<color=red>+------------------------------------ OFF -----------------------------------+</color>");


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