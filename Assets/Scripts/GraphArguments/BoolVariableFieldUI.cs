using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XNoise_DemoWebglPlayer
{
    public class BoolVariableFieldUI : GraphVariableFieldUI
    {
        [SerializeField] private Toggle toggle;

        public override void Setup(string name, string guid, object defaultValue)
        {
            base.Setup(name, guid, defaultValue);
            toggle.SetIsOnWithoutNotify((bool)defaultValue);
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        void OnToggleChanged(bool val) => RaiseValueChanged(val);

        public override void SetValue(object value) => toggle.isOn = (bool)value;
    }
}