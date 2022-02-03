using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuTextCrawl : MonoBehaviour {
    // hierarchy
    public RectTransform console;
    public TextAsset script;
    public TextMeshProUGUI textbox;
    public float delay_popup, delay_startTyping, delay_betweenLetters;

    int index;
    float enableTime;
    float lastAdvanceTime;
    bool poppedUp;

    void OnEnable() {
        console.gameObject.SetActive(false);
        enableTime = Time.time;
        poppedUp = false;
    }

    public void Popup() {
        poppedUp = true;
        console.gameObject.SetActive(true);
        lastAdvanceTime = Time.time + delay_startTyping;
    }

    public void Advance() {
        if(index >= script.text.Length) return;
        switch(script.text[index]) {
        case '[':
            int close = script.text.IndexOf(']', index);
            float delay = float.Parse(script.text.Substring(index+1, close-index-1));
            index = close+1;
            Invoke(nameof(Advance), delay);
            return;
        case ' ':
            textbox.text += script.text[index];
            index ++;
            Advance();
            return;
        }

        textbox.text += script.text[index];
        index ++;
        Invoke(nameof(Advance), delay_betweenLetters);
    }

    void Update() {
        if(!poppedUp && Time.time - enableTime > delay_popup) {
            Popup();
        }
        if(Time.time - lastAdvanceTime > delay_betweenLetters) {
            lastAdvanceTime = Time.time;
            Advance();
        }
    }
}