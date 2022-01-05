using UnityEngine;
using UnityEngine.UI;

public class MenuLoad : MonoBehaviour
{
    public RectTransform content;
    public GameObject prefab_loadButton;

    public void OnEnable()
    {
        int i;
        for(i=0; i<content.transform.childCount; ++i)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }

        var saves = SaveData.GetSaves();

        i=0;
        foreach(var save in saves)
        {
            var go = Instantiate(prefab_loadButton, content);
            var rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition -= new Vector2(0, i*60 + 15);
            var buttonText = go.transform.GetChild(0).GetComponent<Text>();
            buttonText.text = save.saveName;
            go.GetComponent<MenuButton>().load = save.fileName;
            go.GetComponent<MenuButton>().menuLoad = this;
            i++;
        }

        var size = content.offsetMin;
        size.y = -(saves.Length * 60 + 30);
        content.offsetMin = size;
    }
}