using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreviewPanel : MonoBehaviour
{
    public static PreviewPanel instance;
    bool active = false;

    RectTransform rt;
    GameObject AlbumArtContainer, AlbumArt;
    RectTransform AlbumArtContainerRt, AlbumArtRt;
    Image AlbumArtImg;

    GameObject PlayBtn;
    RectTransform PlayBtnRt;

    Text SongNameTxt, SongArtistTxt;

    void Awake(){
        rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height * .75f);
        // rt.sizeDelta = new Vector2(Screen.currentResolution.height, Screen.currentResolution.width * .75f);
        AlbumArtContainer = rt.Find("AlbumArtContainer").gameObject;
        AlbumArt = rt.Find("AlbumArt").gameObject;
        AlbumArtContainerRt = AlbumArtContainer.GetComponent<RectTransform>();
        AlbumArtRt = AlbumArt.GetComponent<RectTransform>();
        AlbumArtContainerRt.sizeDelta = new Vector2(Screen.currentResolution.width * .6f, Screen.currentResolution.width * .6f);
        // AlbumArtContainerRt.sizeDelta = new Vector2(Screen.currentResolution.height * .6f, Screen.currentResolution.height * .6f);
        AlbumArtRt.sizeDelta = new Vector2(AlbumArtContainerRt.rect.width - 20, AlbumArtContainerRt.rect.width - 20);
        float yPos = rt.rect.height * .5f - AlbumArtContainerRt.rect.height *.5f - 50;
        AlbumArtContainerRt.localPosition = new Vector3(0, yPos, 0);
        AlbumArtRt.localPosition = new Vector3(0, yPos, 0);
        AlbumArtImg = AlbumArt.GetComponent<Image>();

        SongNameTxt = rt.Find("SongName").gameObject.GetComponent<Text>();
        SongArtistTxt = rt.Find("SongArtist").gameObject.GetComponent<Text>();
        
        yPos = yPos - AlbumArtContainerRt.rect.height / 2 - 100;
        rt.Find("SongName").gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, yPos, 0);
        rt.Find("SongArtist").gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, yPos - 100, 0);

        PlayBtn = rt.Find("PlayBtn").gameObject;
        PlayBtnRt = PlayBtn.GetComponent<RectTransform>();
        PlayBtnRt.localPosition = new Vector3(-(rt.rect.width - PlayBtnRt.rect.width), -rt.rect.height * 0.25f, 0);
        PlayBtn.GetComponent<Button>().onClick.AddListener(()=>{
            GameManager.instance.selectedSong = SongNameTxt.text;
            FindObjectOfType<SongManager>().SelectSong(SongNameTxt.text);
            SceneManager.LoadSceneAsync("VerticalTest");
        });

        instance = this;
    }
    void Update(){      
        if(active){
            foreach(var touch in Input.touches){
                if(OutOfBounds(touch.position)){
                    rt.position = new Vector3(-2000, 0, 0);
                    active = false;
                }
            }
        }
    }
    public void OnPreview(SongItem si){
        rt.position = new Vector3(0, 0, 0);
        AlbumArtImg.sprite = si.sprite;
        SongNameTxt.text = si.songName;
        SongArtistTxt.text = si.songArtist;
        active = true;
    }
    bool OutOfBounds(Vector3 touchPos){        
        if(touchPos.y > rt.position.y + rt.rect.height / 2 + Screen.currentResolution.height / 2 ||
         touchPos.y < rt.position.y - rt.rect.height / 2 + Screen.currentResolution.height / 2)
            return true;
        return false;
    }
}
