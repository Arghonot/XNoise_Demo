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
        public static event Action OnPressedT;

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Tab)) OnPressedTab?.Invoke();
            if (Input.GetKeyUp(KeyCode.S)) OnPressedS?.Invoke();
            if (Input.GetKeyUp(KeyCode.G)) OnPressedG?.Invoke();
            if (Input.GetKeyUp(KeyCode.R)) OnPressedR?.Invoke();
            if (Input.GetKeyUp(KeyCode.T)) OnPressedT?.Invoke();
        }
    }
}