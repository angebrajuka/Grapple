using UnityEngine;
using UnityEngine.UI;

public class LeadingZeros : MonoBehaviour
{
    // hierarchy
    public int numDigits;

    Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    public void Check()
    {
        if(text.text.Length < numDigits)
        {
            text.text = new string('0', numDigits-text.text.Length)+text.text;
        }
    }
}