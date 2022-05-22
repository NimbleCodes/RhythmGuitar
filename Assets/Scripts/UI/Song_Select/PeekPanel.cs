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
    RectTransform rt;
    public RenderTexture renderTex;
    public GameObject BlurBox;
    public Material BlurBoxMat;

    void Awake(){
        instance = this;
        songManager = FindObjectOfType<SongManager>();
        rt = GetComponent<RectTransform>();
    }
    void Start(){
        Camera.main.targetTexture = renderTex;
        Texture2D screenshot = new Texture2D(renderTex.width, renderTex.height);
        Camera.main.Render();
        RenderTexture.active = renderTex;
        screenshot.ReadPixels(new Rect(0,0,screenshot.width,screenshot.height),0,0);
        screenshot.Apply();
        BlurBox.GetComponent<Image>().sprite = Sprite.Create(screenshot, new Rect(0,0,screenshot.width, screenshot.height), new Vector2(0,0), 100.0f);
        BlurBox.GetComponent<Image>().material = BlurBoxMat;
        Camera.main.targetTexture = null;
    }
    public void OnPreview(SongItem item){
        GameManager.instance.selectedSong = item.songName;
        songName.text = item.songName;
        artistName.text = item.songArtist;
        albumImage.sprite = item.sprite;
        rt.position = new Vector3(0,0,0);
    }
}
