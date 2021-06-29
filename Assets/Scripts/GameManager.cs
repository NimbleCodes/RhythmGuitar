using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    string path = "Assets/Resources/Audio/Robin Hustin x TobiMorrow - Light It Up (feat. Jex) [NCS Release]_data.txt";
    public NoteData noteData;
    DataIO dataIO;
    public AudioSource audioSource;
    void Awake(){
        instance = this;

        noteData = new NoteData();
        dataIO = new DataIO(noteData);
        dataIO.Load(path);
        // string tot = "";
        // noteData.notes[0].ForEach((n)=>{
        //     tot += n.ToString() + ", ";
        // });
        // Debug.Log(tot);

        audioSource.clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes("Assets/Resources/Audio/" + noteData.fileName + ".mp3"));
    }
    void Start(){
        audioSource.Play();
    }
}
