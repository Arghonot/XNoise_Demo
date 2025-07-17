using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CenteringDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private Scrollbar _scrollbar;

    private void Awake()
    {
        StartCoroutine(ScrollToSelectedItem());
    }

    IEnumerator ScrollToSelectedItem()
    {
        yield return null;

        float currentValue = _dropdown.value;
        float optionAmount = _dropdown.options.Count;

        _scrollbar.value = 1 - (currentValue / optionAmount);
    }
}