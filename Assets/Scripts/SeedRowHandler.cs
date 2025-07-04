using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    // Here we assume that there will be only one value named "Seed".
    // This is for demo purposes and is not representative of your/other usages.
    public class SeedRowHandler : MonoBehaviour
    {
        [SerializeField] private FloatVariableFieldUI _seedHandler;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private GameObject _blocker;

        private void Start()
        {
            GraphLibrary.OnSelectedGraphChanged += CallSeedSetup;
            UIManager.CopyButtonClicked += CopySeedToClipBoard;
            UIManager.GenerateButtonClicked += GenerateNewSeed;
            UIManager.RandomSeedStateChanged += RefreshSeed;

            SetupSeedField();
        }

        private void OnDestroy()
        {
            GraphLibrary.OnSelectedGraphChanged -= CallSeedSetup;
            UIManager.CopyButtonClicked -= CopySeedToClipBoard;
            UIManager.GenerateButtonClicked -= GenerateNewSeed;
            UIManager.RandomSeedStateChanged -= RefreshSeed;
        }

        private void CallSeedSetup(CustomGraph.GraphVariables obj) => SetupSeedField();
        private void CopySeedToClipBoard() => GUIUtility.systemCopyBuffer = _seedHandler.GetValue();

        private void GenerateNewSeed()
        {
            if (_uiManager.randomSeed.isOn) _seedHandler.SetValue(((int)Random.Range(0, 9999)).ToString());
        }

        private void RefreshSeed(bool obj) => GenerateNewSeed();

        private void SetupSeedField()
        {
            _blocker.SetActive(!GraphLibrary.CurrentEditedGraphStorage.ContainName("Seed"));

            if (_blocker.active) return;
            var seed = GraphLibrary.CurrentEditedGraphStorage.GetContainerInstance(GraphLibrary.CurrentEditedGraphStorage.GetGUIDFromName("Seed"));
            _seedHandler.Setup(seed.Name, seed.GUID, seed.GetValue());
        }

    }
}