using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KusoGame.Signals;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public SignalManager sigs;
    string path = "Assets/Resources/Audio/Haru Modoki (Asterisk DnB Remix Cut)_data.txt";
    public NoteData noteData;
    DataIO dataIO;
    public AudioSource audioSource;
    void Awake(){
        instance = this;
        sigs = new SignalManager();

        noteData = new NoteData();
        dataIO = new DataIO(noteData);
        dataIO.Load(path);

        audioSource.clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes("Assets/Resources/Audio/" + noteData.fileName + ".mp3"));
    }
    void Start(){
        StartCoroutine("PrepTime");
    }
    IEnumerator PrepTime(){
        yield return new WaitForSeconds(3);
        audioSource.Play();
    }
}
