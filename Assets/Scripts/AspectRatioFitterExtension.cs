using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AspectRatioFitterExtension : MonoBehaviour
{
    AspectRatioFitter arf;
    RawImage ri;

    void Start()
    {
        arf = GetComponent<AspectRatioFitter>();
        ri = GetComponent<RawImage>();
    }

    void Update()
    {
        arf.aspectRatio = (float)ri.texture.width/ri.texture.height;
    }
}