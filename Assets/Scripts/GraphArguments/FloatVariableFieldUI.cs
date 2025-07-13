using System;
using TMPro;
using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class FloatVariableFieldUI : GraphVariableFieldUI
    {
        [SerializeField] private TMP_InputField inputField;
        private Type _originalType; // we store floats, ints, doubles etc
        public string GetValue => inputField.text;
        private string _lastValue;

        public override void Setup(string name, string guid, object defaultValue)
        {
            _originalType = defaultValue.GetType();
            base.Setup(name, guid, defaultValue);
            inputField.SetTextWithoutNotify(defaultValue.ToString());
            inputField.onEndEdit.AddListener(OnInputEnd);
            SetupInputFieldContentType();
            _lastValue = inputField.text;
        }

        void OnInputEnd(string val)
        {
            if (val == _lastValue) return;
            if (_originalType == typeof(float))
            {
                if (float.TryParse(val, out float f)) RaiseValueChanged(f);
            }
            else if (_originalType == typeof(double))
            {
                if (double.TryParse(val, out double f)) RaiseValueChanged(f);
            }
            else if (_originalType == typeof(int))
            {
                if (int.TryParse(val, out int f)) RaiseValueChanged(f);
            }
            _lastValue = inputField.text;
        }

        public override void SetValue(string value)
        {
            inputField.text = value;
            OnInputEnd(value);
        }

        private void SetupInputFieldContentType()
        {
            if (_originalType == typeof(float) || _originalType == typeof(double) || _originalType == typeof(long))
            {
                inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
            }
            else if (_originalType == typeof(int))
            {
                inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            }
        }
    }
}