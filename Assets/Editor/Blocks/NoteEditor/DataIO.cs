using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class DataIO : MonoBehaviour
{
    DataWriter dataWriter;
    NoteEditor NoteEditor;
    AudioPlayer audioPlayer;
    NoteData noteData;
    DataParse parse;
    string basePath;

    private void Start()
    {
        basePath = Application.dataPath + "/Resources/Audio/" + noteData.fileName;
        Debug.Log(basePath);
    }

    public void SetBasePath()
    {
        basePath = Application.dataPath + "/Resources/Audio/" + noteData.fileName;
    }

    public void Save() //저장버튼을 눌렀을때 디렉토리 및 파일 생성.
    {

        DirectoryInfo directoryInfo = new DirectoryInfo(basePath); //해당 디렉토리가 없을때 디렉토리를 생성
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        using (StreamWriter streamWriter = new StreamWriter(new FileStream(basePath + "/" + noteData.fileName + "_data.txt", FileMode.Create, FileAccess.Write), System.Text.Encoding.Unicode))
        {
            streamWriter.Write(dataWriter.WriteSheetInfo());
            streamWriter.Write(dataWriter.WriteContentInfo());
            streamWriter.Write(dataWriter.WriteNoteInfo());
        }
    }

    public void Load(string path)
    {
        string data = "";
        noteData.notes = new List<List<float>>();

        using (StreamReader streamReader = new StreamReader(path))
        {
            while ((data = streamReader.ReadLine()) != null)
            {
                parse.Parse(data);
            }

            audioPlayer.MusicInit();
        }
        parse.isfirstRead = false;
    }

}
