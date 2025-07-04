using System;
using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class ShortcutHandler : MonoBehaviour
    {
        public static event Action OnPressedTab;
        public static event Action OnPressedShiftTab;
        public static event Action OnPressedS;
        public static event Action OnPressedG;
        public static event Action OnPressedR;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab)) OnPressedTab?.Invoke();
            if (Input.GetKeyDown(KeyCode.S)) OnPressedS?.Invoke();
            if (Input.GetKeyDown(KeyCode.G)) OnPressedG?.Invoke();
            if (Input.GetKeyDown(KeyCode.R)) OnPressedR?.Invoke();
        }
    }
}