using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[ExecuteInEditMode]
public class UIColorLink : MonoBehaviour
{
#if(UNITY_EDITOR)
    public Color color;
    public bool ignoreAlpha;

    UIColorLink parentColor;
    Image image;
    RawImage rawImage;
    Text text;
    UIAmmoGraphic ammoGraphic;

    void CheckParent(Transform t)
    {
        if(t == null) return;

        parentColor = t.GetComponent<UIColorLink>();
        if(parentColor == null) CheckParent(t.parent);
    }

    void Start()
    {
        CheckParent(transform.parent);
        image = GetComponent<Image>();
        rawImage = GetComponent<RawImage>();
        text = GetComponent<Text>();
        ammoGraphic = GetComponent<UIAmmoGraphic>();
    }

    void Update()
    {
        if(parentColor == null) return;

        float alpha = color.a;
        color = parentColor.color;
        if(ignoreAlpha) color.a = alpha;
        if(image != null)       image.color = color;
        if(rawImage != null)    rawImage.color = color;
        if(text != null)        text.color = color;
        if(ammoGraphic != null) ammoGraphic.color = color;
    }
#endif
}