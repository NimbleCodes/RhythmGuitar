using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeekPanel : MonoBehaviour
{
    public static PeekPanel instance;
    public Image albumImage;
    public Text songName;
    public Text artistName;
    public SongManager songManager;

    void Awake(){
        instance = this;
        songManager = FindObjectOfType<SongManager>();
    }
    public void OnPreview(SongItem item){
        GameManager.instance.selectedSong = item.songName;
        songName.text = item.songName;
        artistName.text = item.songArtist;
        albumImage.sprite = item.sprite;
        songManager.PlayAudioPreview(item.songName);
    }
}
