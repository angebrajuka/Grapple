using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

    public float[] player_pos;
    public float[] player_rot_rb;
    public float[] player_rot_cam;

    public (float x, float y, int index)[] bloodUI_splatters;

    public S_Enemy[] enemies;
    public float enemy_spawnTimer;

    public SaveData()
    {
        player_pos = new float[3];
        for(int j=0; j<3; j++) player_pos[j] = PlayerMovement.m_rigidbody.position[j];
        player_rot_rb = new float[3];
        for(int j=0; j<3; j++) player_rot_rb[j] = PlayerMovement.m_rigidbody.transform.localEulerAngles[j];
        player_rot_cam = new float[3];
        for(int j=0; j<3; j++) player_rot_cam[j] = PlayerMovement.instance.t_camera.localEulerAngles[j];

        bloodUI_splatters = new (float, float, int)[PlayerBloodUI.splatters.Count];
        int i=0;
        foreach(var splatter in PlayerBloodUI.splatters)
        {
            bloodUI_splatters[i] = (splatter.pos.x, splatter.pos.y, splatter.index);
            i ++;
        }
    }

    public void Load()
    {
        Vector3 pos = PlayerMovement.m_rigidbody.position;
        for(int i=0; i<3; i++) pos[i] = player_pos[i];
        PlayerMovement.m_rigidbody.position = pos;

        Vector3 rot = PlayerMovement.m_rigidbody.transform.localEulerAngles;
        for(int i=0; i<3; i++) rot[i] = player_rot_rb[i];
        PlayerMovement.m_rigidbody.transform.localEulerAngles = rot;

        rot = PlayerMovement.instance.t_camera.localEulerAngles;
        for(int i=0; i<3; i++) rot[i] = player_rot_cam[i];
        PlayerMovement.instance.t_camera.localEulerAngles = rot;

        foreach(var splatter in bloodUI_splatters)
        {
            PlayerBloodUI.AddSplatter(new Vector2(splatter.x, splatter.y), splatter.index, false);
        }
    }

    public static string DIRECTORY_PATH
    {
        get { return Application.persistentDataPath + "/savedata/"; }
    }

    public static string FILE_PATH
    {
        get { return DIRECTORY_PATH+"game.sav"; }
    }

    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        Directory.CreateDirectory(DIRECTORY_PATH);
        FileStream stream = new FileStream(FILE_PATH, FileMode.Create);

        SaveData data = new SaveData();
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static bool TryLoad()
    {
        if(File.Exists(FILE_PATH))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            var stream = new FileStream(FILE_PATH, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            data.Load();
            stream.Close();

            return true;
        }
        return false;
    }
}