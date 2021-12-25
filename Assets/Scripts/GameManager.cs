using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kgh.Signals;

public class GameManager : MonoBehaviour
{
    public Exchange sigs;
    public static GameManager instance;
    public string selectedSong{set; get;}

    void Awake(){
        instance = this;
        sigs = new Exchange();
        //TEST
        GameObject.DontDestroyOnLoad(gameObject);
    }
    void Update(){
        //TEST
        if(Input.GetKeyDown(KeyCode.Space)){
            Time.timeScale = 0;
            //오디오 멈춤도 필요
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            Time.timeScale = 1;
        }
    }
    void OnDestroy(){
        //Debug.Log(sigs.ToString());
    }
}
