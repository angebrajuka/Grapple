using UnityEngine;
using UnityEngine.UI;

public class MenuBetterScroll : MonoBehaviour
{
    // hierarchy
    public float sens;

    ScrollRect scrollRect;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    void Update()
    {
        if(Input.mouseScrollDelta.y == 0) return;

        var pos = scrollRect.content.anchoredPosition;
        pos.y -= Input.mouseScrollDelta.y * sens;
        scrollRect.content.anchoredPosition = pos;
    }
}