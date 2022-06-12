using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using kgh.Signals;
using UnityEngine.Networking;

public class Scene_1_Manager : MonoBehaviour
{
    DataIO dataIO;
    public NoteData noteData;
    public AudioSource audioSource;
    Switch audioLoaded, playAudio, gameOver;
    public float delay = 2.0f;
    bool once = true;
    bool played = false;

    void Awake(){
        noteData = new NoteData();
        dataIO = new DataIO(noteData);

        string path;
        path = GameManager.instance.selectedSong + "/" + GameManager.instance.selectedSong;
        dataIO.Load(path + "_data");
        audioSource.clip = Resources.Load<AudioClip>(path);
        
        audioLoaded = GameManager.instance.sigs.Register("audio_loaded", typeof(Action<NoteData>));
        playAudio = GameManager.instance.sigs.Register("play_audio", typeof(Action));
        gameOver = GameManager.instance.sigs.Register("game_over", typeof(Action));
    }
    void Start(){
        // audioSource.clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(path + ".mp3"));
        audioSource.PlayDelayed(delay);
    }
    void Update(){
        if(once){
            if(audioLoaded != null)
                audioLoaded.Invoke(noteData);
            once = false;
        }
        if(audioSource.isPlaying){
            played = true;
        }
        if(played && !audioSource.isPlaying){
            //game over
            if(gameOver != null)
                gameOver.Invoke();
        }
    }
    IEnumerator StartDelay(){
        yield return new WaitForSeconds(delay);
        audioSource.Play();
    }
}
