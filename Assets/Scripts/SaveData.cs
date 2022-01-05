using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public class S_Enemy
    {
        public int index;
        public float[] pos;

        public S_Enemy(Vector3 pos, int index)
        {
            this.pos = new float[]{pos.x, pos.y, pos.z};
            this.index = index;
        }
    }

    public string saveName;

    public float seed;

    public float[] player_pos;
    public float[] player_rot_rb;
    public float[] player_rot_cam;

    public (float x, float y, int index)[] bloodUI_splatters;

    public S_Enemy[] enemies;
    public float enemy_spawnTimer;

    public SaveData()
    {
        saveName = currentSaveName;

        seed = ProceduralGeneration.seed;

        player_pos = new float[3];
        for(int j=0; j<3; j++) player_pos[j] = PlayerMovement.rb.position[j];
        player_rot_rb = new float[3];
        for(int j=0; j<3; j++) player_rot_rb[j] = PlayerMovement.rb.transform.localEulerAngles[j];
        player_rot_cam = new float[3];
        for(int j=0; j<3; j++) player_rot_cam[j] = PlayerMovement.instance.t_camera.localEulerAngles[j];

        // bloodUI_splatters = new (float, float, int)[PlayerBloodUI.splatters.Count];
        // int i=0;
        // foreach(var splatter in PlayerBloodUI.splatters)
        // {
        //     bloodUI_splatters[i] = (splatter.pos.x, splatter.pos.y, splatter.index);
        //     i ++;
        // }
    }

    public void Load()
    {
        currentSaveName = saveName;

        ProceduralGeneration.seed = seed;

        Vector3 pos = PlayerMovement.rb.position;
        for(int i=0; i<3; i++) pos[i] = player_pos[i];
        PlayerMovement.rb.position = pos;

        Vector3 rot = PlayerMovement.rb.transform.localEulerAngles;
        for(int i=0; i<3; i++) rot[i] = player_rot_rb[i];
        PlayerMovement.rb.transform.localEulerAngles = rot;

        rot = PlayerMovement.instance.t_camera.localEulerAngles;
        for(int i=0; i<3; i++) rot[i] = player_rot_cam[i];
        PlayerMovement.instance.t_camera.localEulerAngles = rot;

        // foreach(var splatter in bloodUI_splatters)
        // {
        //     PlayerBloodUI.AddSplatter(new Vector2(splatter.x, splatter.y), splatter.index, false);
        // }
    }

    public static string currentSaveName;
    public static string currentSaveFileName;

    public static string DIRECTORY_PATH { get { return Application.persistentDataPath + "/savedata/"; } }

    public static (string fileName, string saveName)[] GetSaves()
    {
        var fileInfos = new DirectoryInfo(DIRECTORY_PATH).GetFiles("*.save");
        Array.Sort(fileInfos, (y, x) => StringComparer.OrdinalIgnoreCase.Compare(x.CreationTime, y.CreationTime));
        var saves = new (string, string)[fileInfos.Length];

        int i=0;
        foreach(var fileInfo in fileInfos)
        {
            var data = GetSaveData(fileInfo.Name);
            saves[i] = (fileInfo.Name, data.saveName);
            i++;
        }

        return saves; // file name, save name
    }

    public static void Save(string fileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        Directory.CreateDirectory(DIRECTORY_PATH);
        FileStream stream = new FileStream(DIRECTORY_PATH + fileName, FileMode.Create);

        SaveData data = new SaveData();
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData GetSaveData(string fileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        var stream = new FileStream(DIRECTORY_PATH + fileName, FileMode.Open);

        SaveData data = formatter.Deserialize(stream) as SaveData;
        stream.Close();
        return data;
    }

    public static bool Load(string fileName)
    {
        if(File.Exists(DIRECTORY_PATH + fileName))
        {
            GetSaveData(fileName).Load();
            return true;
        }
        return false;
    }
}