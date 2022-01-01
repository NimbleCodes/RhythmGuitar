using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Scene_1_v2 : MonoBehaviour
{
    public static Scene_1_v2 instance;
    NoteData noteData;
    DataParse dataParse;

    void Awake(){
        instance = this;
        noteData = new NoteData();
        dataParse = new DataParse(noteData);
    }
    void Start(){
        GameManager_v2.instance.selectedSong = "Haru Modoki (Asterisk DnB Remix Cut)";
        TextAsset mScore = Resources.Load<TextAsset>(GameManager_v2.instance.selectedSong + "/" + GameManager_v2.instance.selectedSong + "_data");
        dataParse.Parse(mScore.text);
        
    }
}
