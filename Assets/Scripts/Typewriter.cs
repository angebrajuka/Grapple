using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Typewriter : MonoBehaviour
{
    private int index;
    private string actualText = "";
    
    public TextMeshProUGUI logTextBox;
    public TextAsset finalText;
    public float startDelay;
    public PauseInfo pauseInfo;
    public Action endFunc;

    private void OnEnable()
    {
        Invoke(nameof(ReproduceText), startDelay);
    }

    private void ReproduceText()
    {
        //if not readied all letters
        if (index < finalText.text.Length) {
            //get one letter
            char letter = finalText.text[index];

            //Actualize on screen
            logTextBox.text = Write(letter);

            //set to go to the next
            index += 1;
            StartCoroutine(PauseBetweenChars(letter));
        }
        else {
            endFunc();
        }
    }

    private string Write(char letter)
    {
        actualText += letter;
        return actualText;
    }

    private IEnumerator PauseBetweenChars(char letter)
    {
        switch (letter)
        {
        case '.':
        case '\n':
            yield return new WaitForSeconds(pauseInfo.dotPause);
            ReproduceText();
            yield break;
        case ',':
        case ':':
            yield return new WaitForSeconds(pauseInfo.commaPause);
            ReproduceText();
            yield break;
        case ' ':
            yield return new WaitForSeconds(pauseInfo.spacePause);
            ReproduceText();
            yield break;
        default:
            yield return new WaitForSeconds(pauseInfo.normalPause);
            ReproduceText();
            yield break;
        }
    }
}

[Serializable]
public class PauseInfo
{
    public float dotPause;
    public float commaPause;
    public float spacePause;
    public float normalPause;
}