using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SongList : MonoBehaviour
{
    public List<SongItem> items = new List<SongItem>();
    public SongDisplay songDisplayPrefab;

    public SongItem songItem;
    public List<NewSongItem> newItems = new List<NewSongItem>();

    public int dirCnt;

    void Start()
    {
        AddItem();

        SongDisplay song = (SongDisplay)Instantiate(songDisplayPrefab);
        song.GetComponent<Canvas>().worldCamera = Camera.main;
        song.SetData(items);
    }

    public void AddItem()
    {
        string fileName = "";
        // string basePath = Application.dataPath + "/Resources/Audio/";
        // DirectoryInfo directoryInfo = new DirectoryInfo(basePath);
        
        //디렉토리 리스트파일 작성, 리스트 파일을 읽어 디렉토리내 리소스를 정상로딩
        TextAsset DirList = Resources.Load<TextAsset>("TestFiles/DirList"); 
        using(StringReader strReader = new StringReader(DirList.text)){
            while((fileName = strReader.ReadLine()) != null){
                dirCnt++;
                Debug.Log("Audio/" + fileName + "/" + fileName + "_data");
                TextAsset dataFile = Resources.Load<TextAsset>("Audio/" + fileName + "/" + fileName + "_data");
                string data = "";
                using(StringReader strReader2 = new StringReader(dataFile.text)){
                    while((data = strReader2.ReadLine()) != null){
                        string[] splitedData = new string[2];
                        splitedData = data.Split('=');
                        if (splitedData[0] == "Title")
                            songItem.songName = splitedData[1];
                        else if (splitedData[0] == "Artist")
                            songItem.songArtist = splitedData[1];
                        else if (splitedData[0] == "Difficult")
                            songItem.songLevel = splitedData[1];
                        else if (splitedData[0] == "ImageFileName")
                            songItem.sprite = Resources.Load<Sprite>("Audio/" + fileName + "/" + fileName + "_Img");
                    }
                }
                items.Add(new SongItem(songItem.songName, songItem.songLevel, songItem.songArtist, songItem.sprite));
            }
        }
    }
}
