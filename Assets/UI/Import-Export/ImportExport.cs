using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ImportExport : myUI.Component{
    public List<List<Note>> lanes;
    public class ImportExportStates : States{
        ImportExport component;
        AudioClip _audioClip;
        public AudioClip audioClip{
            set{
                _audioClip = value;
                dirty = true;
            }
            get{ return _audioClip; }
        }
        public ImportExportStates(ImportExport _component) : base(_component){
            component = _component;
        }
    }
    public ImportExport(){
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Import-Export/ImportExport.uxml");
        rootVisualElement = visualTreeAsset.CloneTree();
        rootVisualElement.style.width = Length.Percent(100);
        rootVisualElement.style.height = Length.Percent(100);
        rootVisualElement.name = "import-export";
        rootVisualElement.Q<Button>("import-btn").clicked += ()=>{
            string originalPath = EditorUtility.OpenFilePanel("Importer", "Assets/Resources/Audio", "mp3,txt");
            string extention = Path.GetExtension(originalPath);

            switch(extention){
                case ".mp3":
                    string path = "Assets/Resources/Audio/" + Path.GetFileNameWithoutExtension(originalPath) + "/" + Path.GetFileName(originalPath);
                    ((ImportExportStates)states).audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    rootVisualElement.Q<TextField>("path-display").value = path;
                break;
                case ".txt":
                    NoteData noteData = new NoteData();
                    DataIO dataIO = new DataIO(noteData);
                    string musicName = Path.GetFileNameWithoutExtension(originalPath);
                    //REMOVE _DATA FROM FILE NAME
                    musicName = musicName.Substring(0, musicName.Length - 5);
                    path = "Audio/" + musicName + "/" + Path.GetFileNameWithoutExtension(originalPath);
                    dataIO.Load(path);
                    //USE LOADED DATA TO INITIALIZE INTERNAL DATA STRUCTURE

                    ((ImportExportStates)states).audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources/Audio/" + musicName + "/" + musicName + ".mp3");
                    rootVisualElement.Q<TextField>("path-display").value = path;
                break;
            }
        };
        rootVisualElement.Q<Button>("export-btn").clicked += ()=>{
            NoteData noteData = new NoteData();
            //INPUT NOTES HERE

            DataIO dataIO = new DataIO(noteData);
            dataIO.Save();
        };
        states = new ImportExportStates(this);
    }
    protected override void _Update()
    {
        // Debug.Log("ImportExport::Update");
    }
    protected override void Synchronize()
    {
        //do nothing
    }
    protected override void _Dispose()
    {
        //do nothing
    }
}