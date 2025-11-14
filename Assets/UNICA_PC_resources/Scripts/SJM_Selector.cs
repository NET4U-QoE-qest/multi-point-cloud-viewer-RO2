using TMPro;
using UnityEngine;


public class SJM_Selector : MonoBehaviour
{
    // current rateing value
    public int currentValue;

    // ui reference
    public TMP_Text valueContainer;

    // selector manager reference
    SJM_SelectorManager selectorManager;

    void Start()
    {
        currentValue = int.Parse(valueContainer.text);
        selectorManager = GetComponentInParent<SJM_SelectorManager>();
    }

    public void ValueUP()
    {
        if (currentValue > 8)
        {
            currentValue = 0;
            UpdateUI();
        }
        else
        {
            currentValue++;
            UpdateUI();
        }
    }

    public void ValueDown()
    {
        if (currentValue < 1)
        {
            currentValue = 9;
            UpdateUI();
        }
        else
        {
            currentValue--;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        string playerIDValue = currentValue + "";
        valueContainer.text = playerIDValue;
        selectorManager.UpdateSelectorsValues();
    }
}
