using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;

public class MenuNewGame : MonoBehaviour
{
    public InputField seed;
    public Dropdown difficulty;
    public InputField save;

    const string glyphs= "abcdefghijklmnopqrstuvwxyz0123456789";
    static string RandString(int length)
    {
        StringBuilder sb = new StringBuilder();
        for(int i=0; i<length; i++)
        {
            sb.Append(glyphs[UnityEngine.Random.Range(0, glyphs.Length)]);
        }
        return sb.ToString();
    }

    public void StartGame()
    {
        if(!float.TryParse(seed.text, out ProceduralGeneration.seed))
        {
            if(seed.text == "")
            {
                ProceduralGeneration.RandomSeed();
            }
            else
            {
                int hash = seed.text.GetHashCode();
                ProceduralGeneration.seed = Mathf.Abs(4000*Mathf.Sin(90285421.3940567f*((float)hash / (float)(Mathf.Pow(10, hash.ToString().Length/2)))));
            }
        }

        EnemySpawning.difficulty = difficulty.value;

        SaveData.currentSaveName = save.text;
        if(SaveData.currentSaveName == "") SaveData.currentSaveName = save.placeholder.GetComponent<Text>().text;

        int i=0;
        do
        {
            SaveData.currentSaveFileName = RandString(10)+".save";
            i++;
        }
        while(File.Exists(SaveData.DIRECTORY_PATH + SaveData.currentSaveFileName) && i < 1000);

        EnemySpawning.instance.Reset();
        PlayerMovement.instance.Reset();
        PlayerInventory.Reset();
        // other reset shit

        SaveData.Save(SaveData.currentSaveFileName);

        MenuHandler.StartGame();
    }
}