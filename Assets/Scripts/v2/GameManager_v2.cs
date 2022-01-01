using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class GameManager_v2 : MonoBehaviour
{
    public static GameManager_v2 instance;
    UserData userData;
    public string selectedSong;

    void Awake(){
        instance = this;
        DontDestroyOnLoad(gameObject);
        if(File.Exists(Application.persistentDataPath + "/user_data.json"))
            userData = JsonConvert.DeserializeObject<UserData>(File.ReadAllText(Application.persistentDataPath + "/user_data.json"));
        else
            userData = new UserData();
    }
    void OnApplicationQuit(){
        string userDataJson = JsonConvert.SerializeObject(userData);
        File.WriteAllText(Application.persistentDataPath + "/user_data.json", userDataJson);
    }
}
public class UserData
{
    public Dictionary<string, float> playData;
    public UserData(){
        playData = new Dictionary<string, float>();
    }
}