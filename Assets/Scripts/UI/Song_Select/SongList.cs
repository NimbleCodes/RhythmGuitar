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
    public GameObject template;

    int dirCnt;

    void Start()
    {
        AddItem();
        template.AddComponent<SongItem>();

        for(int i=0; i < dirCnt; i++)
        {
            GameObject obj = Instantiate(template);
            SongItem sItem = obj.GetComponent<SongItem>();

            sItem.songName = newItems[i].songName;
            sItem.songArtist = newItems[i].songArtist;
            sItem.songLevel = newItems[i].songLevel;
            sItem.sprite = newItems[i].sprite;

            items.Add(obj.GetComponent<SongItem>());
        }


        SongDisplay song = (SongDisplay)Instantiate(songDisplayPrefab);
        song.Prime(items);
    }

    void AddItem()
    {
        string fileName = "";
        // string basePath = Application.dataPath + "/Resources/Audio/";
        // DirectoryInfo directoryInfo = new DirectoryInfo(basePath);

        TextAsset DirList = Resources.Load<TextAsset>("TestFiles/DirList"); //디렉토리 리스트파일 작성, 리스트 파일을 읽어 디렉토리내 리소스를 정상로딩
        using(StringReader strReader = new StringReader(DirList.text)){
            while((fileName = strReader.ReadLine()) != null){
                dirCnt++;
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
                            songItem.sprite = Resources.Load<Sprite>(fileName + "/" + fileName + "_Img");
                    }
                }
                newItems.Add(new NewSongItem(songItem.songName, songItem.songLevel, songItem.songArtist, songItem.sprite));
            }
        }

        // foreach()
        // {
        //     Debug.Log(basePath + di.Name + "/" + di.Name);
        //     dirCnt++;
        //     using (StreamReader streamReader = new StreamReader(basePath + di.Name + "/" + di.Name + "_data.txt"))
        //     {
        //         while((data = streamReader.ReadLine()) != null)
        //         {
        //             // parse
        //             string[] splitedData = new string[2];
        //             splitedData = data.Split('=');

        //             if (splitedData[0] == "Title")
        //                 songItem.songName = splitedData[1];
        //             else if (splitedData[0] == "Artist")
        //                 songItem.songArtist = splitedData[1];
        //             else if (splitedData[0] == "Difficult")
        //                 songItem.songLevel = splitedData[1];
        //             else if (splitedData[0] == "ImageFileName")
        //                 songItem.sprite = Resources.Load<Sprite>(di.Name + "/" + di.Name + "_Img");
        //         }
        //     }

        //     newItems.Add(new NewSongItem(songItem.songName, songItem.songLevel, songItem.songArtist, songItem.sprite));
        // }
    }
}
