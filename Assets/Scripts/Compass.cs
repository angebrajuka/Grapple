using UnityEngine;

public class Compass : MonoBehaviour
{
    public RectTransform border;
    public RectTransform scrollPoint;

    void Update()
    {
        var pos = scrollPoint.anchoredPosition;

        pos.x = (-PlayerMovement.rb.transform.eulerAngles.y / 180f) * border.sizeDelta.x;


        scrollPoint.anchoredPosition = pos;
    }
}