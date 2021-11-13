using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DataIO
{
    DataWriter dataWriter;
    
    #if UNITY_ENGINE
    NoteEditor NoteEditor;
    AudioPlayer audioPlayer;
    #endif
    
    NoteData noteData;
    DataParse parse;
    string basePath;

    public DataIO(NoteData _noteData)
    {
        noteData = _noteData;
        parse = new DataParse(noteData);
        basePath = Application.dataPath + "/Resources/Audio/" + noteData.fileName;
        dataWriter = new DataWriter();
        dataWriter.N_data = noteData;
        //Debug.Log(basePath);
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
        TextAsset txtAsset = Resources.Load<TextAsset>(path);
        // Debug.LogError(txtAsset == null);
        // string data = "";
        // using(StringReader strReader = new StringReader(txt.text)){
        //     while((data = strReader.ReadLine()) != null){
        //         parse.Parse(data);
        //         Debug.Log(data + "\n");
        //     }
        // }

        string data = "";
        // // noteData.notes = new List<List<float>>();

        using (StringReader strReader = new StringReader(txtAsset.text))
        {
            while ((data = strReader.ReadLine()) != null)
            {
                parse.Parse(data);
            }
            // audioPlayer.MusicInit();
        }

        // if(Application.platform == RuntimePlatform.Android){
        
        // }
        // else{
        //     string data = File.ReadAllText(path);
        //     string[] tokens = data.Split('\n');
        //     for(int i = 0; i < tokens.Length - 1; i++){
        //         parse.Parse(tokens[i]);
        //     }
        // }

        parse.isfirstRead = false;
    }

}
