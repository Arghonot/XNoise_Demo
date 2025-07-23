using System;
using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    // Here we assume that there will be only one value named "Seed".
    // This is for demo purposes and is not representative of your/other usages.
    public class SeedRowHandler : MonoBehaviour
    {
        public static string Seed = string.Empty;

        [SerializeField] private FloatVariableFieldUI _seedHandler;
        private UIManager _uiManager;
        [SerializeField] private GameObject _blocker;

        public static event Action OnCopiedSeedToClipboard;

        private void Start()
        {
            _uiManager = GetComponent<UIManager>();

            ShortcutHandler.OnPressedG += GenerateNewSeed;
            GraphLibrary.OnSelectedGraphChanged += CallSeedSetup;
            UIManager.CopyButtonClicked += CopySeedToClipBoard;
            UIManager.GenerateButtonClicked += GenerateNewSeed;
            UIManager.RandomSeedStateChanged += RefreshSeed;

            SetupSeedField();
        }

        private void OnDestroy()
        {
            ShortcutHandler.OnPressedG -= GenerateNewSeed;
            GraphLibrary.OnSelectedGraphChanged -= CallSeedSetup;
            UIManager.CopyButtonClicked -= CopySeedToClipBoard;
            UIManager.GenerateButtonClicked -= GenerateNewSeed;
            UIManager.RandomSeedStateChanged -= RefreshSeed;
        }

        private void CallSeedSetup(CustomGraph.GraphVariables obj) => SetupSeedField();

        private void CopySeedToClipBoard()
        {
            GUIUtility.systemCopyBuffer = _seedHandler.GetValue;
            OnCopiedSeedToClipboard?.Invoke();
        }

        private void GenerateNewSeed()
        {
            if (_uiManager.randomSeed.isOn) _seedHandler.SetValue(((int)UnityEngine.Random.Range(0, 9999)).ToString());
            Seed = _seedHandler.GetValue;
        }

        private void RefreshSeed(bool obj) => GenerateNewSeed();

        private void SetupSeedField()
        {
            _blocker.SetActive(!GraphArgumentsHandler.currentStorage.ContainName("Seed"));

            if (_blocker.activeInHierarchy) return;
            var guid = GraphArgumentsHandler.currentStorage.GetGUIDFromName("Seed");
            var seed = GraphArgumentsHandler.currentStorage.GetContainerInstance(guid);
            _seedHandler.Setup(seed.Name, seed.GUID, seed.GetValue());
        }
    }
}