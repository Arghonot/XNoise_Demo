using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CenteringDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private Scrollbar _scrollbar;

    private float yPositionForSingleItem => 1 / _dropdown.options.Count;

    private void Awake()
    {
        StartCoroutine(ScrollToSelectedItem());
    }

    IEnumerator ScrollToSelectedItem()
    {
        yield return null;

        float currentValue = _dropdown.value;
        float optionAmount = _dropdown.options.Count;

        _scrollbar.value = Mathf.Clamp(1 - (currentValue / optionAmount), yPositionForSingleItem, 1 - yPositionForSingleItem);
    }
}