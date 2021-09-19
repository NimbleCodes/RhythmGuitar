using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kgh.Signals;

public class GameManager : MonoBehaviour
{
    public Exchange sigs;
    public static GameManager instance;
    public string selectedSong{private set; get;}

    void Awake(){
        instance = this;
        sigs = new Exchange();
        //TEST
        selectedSong = "Haru Modoki (Asterisk DnB Remix Cut)";
        // GameObject.DontDestroyOnLoad(gameObject);
    }
}
