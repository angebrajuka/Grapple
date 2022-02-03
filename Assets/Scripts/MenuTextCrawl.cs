using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuTextCrawl : MonoBehaviour {
    // hierarchy
    public Typewriter console;
    public GameObject continueButton;
    public float delay_popup, delay_end;
    public MenuNewGame menuNewGame;

    void OnEnable() {
        console.endFunc = End;
        console.gameObject.SetActive(false);
        Invoke(nameof(Popup), delay_popup);
    }

    public void Popup() {
        console.gameObject.SetActive(true);
    }

    public void End() {
        menuNewGame.Invoke(nameof(menuNewGame.StartGame), delay_end);
    }
}