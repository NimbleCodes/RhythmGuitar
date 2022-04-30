using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBack : MonoBehaviour
{
    public SongManager songManager;
    NoteData data;
    
    public void goBack(){
        songManager.StopSong(true);
        //Destroy(data.notes); //로드해둔 노트정보를 Destroy 시켜야하지 않을까


        SceneManager.LoadScene("SongSelect");
    }
}
