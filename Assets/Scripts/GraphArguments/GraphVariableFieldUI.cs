using System;
using TMPro;
using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    // Base class for any UI row that exposes graph's arguments
    public abstract class GraphVariableFieldUI : MonoBehaviour
    {
        [HideInInspector] [SerializeField] protected string guid;
        [SerializeField] protected TMP_Text _nameLabel;

        public static event Action<string, object> OnValueChanged;

        protected void RaiseValueChanged(object value)
        {
            GraphArgumentsHandler.currentStorage.SetValue(guid, value);
            OnValueChanged?.Invoke(guid, value);
        }

        public virtual void SetValue(object value) { }
        public virtual void SetValue(string value) { }
        public virtual void Setup(string name, string guid, object defaultValue)
        {
            this.guid = guid;
            _nameLabel.text = name.Replace("_", "  ");
        }
    }
}