using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SongItemDisplay : MonoBehaviour
{
    public Text Name,Level,Artist;
    public Image sprite;

    public delegate void SongItemDisplayDelegate(SongItemDisplay item);
    public event SongItemDisplayDelegate onClick;

    public SongItem item;
    public SongManager songManager;


    public void GetInfo(SongItem item)
    {
        this.item = item;
        if (Name != null)
            Name.text = item.songName;
        if (Level != null)
            Level.text = item.songLevel;
        if (Artist != null)
            Artist.text = item.songArtist;
        if (sprite != null)
            sprite.sprite = item.sprite;
    }   
}
