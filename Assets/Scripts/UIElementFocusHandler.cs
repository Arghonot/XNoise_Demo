using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XNoise_DemoWebglPlayer
{
    public class UIElementFocusHandler : MonoBehaviour
    {
        private void Awake()
        {
            ShortcutHandler.OnPressedTab += HandleTab;
            GraphArgumentsHandler.OnFinishedLoadingArguments += SelectSelectable;
        }

        private void OnDestroy()
        {
            ShortcutHandler.OnPressedTab -= HandleTab;
            GraphArgumentsHandler.OnFinishedLoadingArguments -= SelectSelectable;
        }

        private void SelectSelectable(Selectable obj) => obj.Select();

        private void HandleTab()
        {
            var current = EventSystem.current.currentSelectedGameObject;
            if (current == null) return;

            var currentSelectable = current.GetComponent<Selectable>();
            if (currentSelectable == null) return;

            var next = currentSelectable.FindSelectableOnDown();
            if (next != null) next.Select();
        }
    }
}