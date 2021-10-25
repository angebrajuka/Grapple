using UnityEngine;
using UnityEngine.UI;

public class LeadingZeros : MonoBehaviour
{
    // hierarchy
    public int numDigits;

    Text text;

    public void Check()
    {
        if(text == null) text = GetComponent<Text>();

        if(text.text.Length < numDigits)
        {
            text.text = new string('0', numDigits-text.text.Length)+text.text;
        }
    }
}