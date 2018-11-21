using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

class GameSaveManager : MonoBehaviour {

   
    //public GameLoad gameLoad;
    public Game game;
    //public Game savedGame = new Game();

    public List<GameObject> objects = new List<GameObject>();

    public string SAVE_FILE = "/SAVEGAME";
    public string FILE_EXTENSION = ".TXT";

    void Awake()
    {
        //gameLoad = new GameLoad();
        game = GameObject.Find("Game").GetComponent<Game>();
    }
    void Start()
    {
        //gameLoad.game = Game.game;
        objects = game.solarSystemPrefabs;
    }

    public bool isSaveFile()
    {
        return Directory.Exists(Application.persistentDataPath + "/game_save");
    }

    public void SaveGame()
    {

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + SAVE_FILE + FILE_EXTENSION);
        var json = JsonUtility.ToJson(objects);
        bf.Serialize(file, json);
        file.Close();
    }

    public void LoadGame()
    {
        if (File.Exists(Application.dataPath + SAVE_FILE + FILE_EXTENSION))
        {
            FileStream file = File.Open(Application.dataPath + SAVE_FILE + FILE_EXTENSION,FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), objects);

            for (int i = 0; i < objects.Count; i++)
            {
                Debug.Log(objects[i].name);
            }

            file.Close();
        }
    }

    //public void SaveGame()
    //{
    //    Stream stream = File.Open(Application.dataPath + SAVE_FILE + FILE_EXTENSION, FileMode.OpenOrCreate);
    //    BinaryFormatter bf = new BinaryFormatter();

    //    savedGame = gameLoad.game;

    //    var json = JsonUtility.ToJson(savedGame);
    //    bf.Serialize(stream, json);


    //    bf.Serialize(stream, savedGame);
    //    stream.Close();

    //}

    //public void LoadGame()
    //{
    //    Stream stream = File.Open(Application.dataPath + SAVE_FILE + FILE_EXTENSION, FileMode.Open);
    //    BinaryFormatter bf = new BinaryFormatter();

    //    savedGame = (Game)bf.Deserialize(stream);
    //    stream.Close();



    //}
}
