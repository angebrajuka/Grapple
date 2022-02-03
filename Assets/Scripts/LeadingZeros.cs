using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeadingZeros : MonoBehaviour
{
    // hierarchy
    public int numDigits;

    TextMeshProUGUI text;

    public void Check()
    {
        if(text == null) text = GetComponent<TextMeshProUGUI>();

        if(text.text.Length < numDigits)
        {
            text.text = new string('0', numDigits-text.text.Length)+text.text;
        }
    }
}