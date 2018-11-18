using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

class GameSaveManager : MonoBehaviour {


    Game game;
    List<SolarSystem> Systems;

    GameSaveManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);

        game = GameObject.Find("Game").GetComponent<Game>();
        Systems = new List<SolarSystem>();
        Systems = game.Systems;
    }

    public bool isSaveFile()
    {
        return Directory.Exists(Application.persistentDataPath + "/game_save");
    }

    public void SaveGame()
    {
        if (!isSaveFile())
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/game_save");
        }
        if (!Directory.Exists(Application.persistentDataPath + "/game_save/game_data"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/game_save/game_data");
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/game_save/game_data/game_saved.txt");
        var json = JsonUtility.ToJson(Systems.Count);
        bf.Serialize(file, json);
        file.Close();
    }

    public void LoadGame()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/game_save/game_data"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/game_save/game_data");
        }
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(Application.persistentDataPath + "game_save/game_data/game_saved.txt"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "game_save/game_data/game_saved.", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), Systems.Count);
            file.Close();
        }

    
    }
}
