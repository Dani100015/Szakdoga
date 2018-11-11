using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

class GameStartOptions : MonoBehaviour {

    public string PlayerName;
    public string EnemyName;
    public int StarCount;

    public Text playnerNameText;
    public Text enemyNameText;
    public Text starCountNumberText;

    void Awake()
    {
        DontDestroyOnLoad(transform);
    }
    public void SaveSetting()
    {
        PlayerName = playnerNameText.text;
        EnemyName = enemyNameText.text;
        StarCount = int.Parse(starCountNumberText.text);
    }

}
