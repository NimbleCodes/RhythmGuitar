using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class ImportExport : myUI.Component{
    public List<List<Note>> lanes;
    string path;
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
        int _bpm;
        public int bpm{
            set{
                _bpm = value;
                dirty = true;
            }
            get{ return _bpm; }
        }
        public ImportExportStates(ImportExport _component) : base(_component){
            component = _component;
            _bpm = 120;
        }
    }
    public ImportExport(){
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Import-Export/ImportExport.uxml");
        rootVisualElement = visualTreeAsset.CloneTree();
        rootVisualElement.style.width = Length.Percent(100);
        rootVisualElement.style.height = Length.Percent(100);
        rootVisualElement.name = "import-export";

        TextField titleField = rootVisualElement.Q<VisualElement>("textfield-title").Q<TextField>("textfield-value");
        TextField artistField = rootVisualElement.Q<VisualElement>("textfield-artist").Q<TextField>("textfield-value");
        TextField bpmField = rootVisualElement.Q<VisualElement>("textfield-bpm").Q<TextField>("textfield-value");
        bpmField.RegisterValueChangedCallback((e)=>{
            float newVal = -1;
            if(!float.TryParse(e.newValue, out newVal)){
                bpmField.value = e.previousValue;
            }
        });
        bpmField.RegisterCallback<FocusOutEvent>((e)=>{
            float newVal = -1;
            if(float.TryParse(bpmField.value, out newVal)){
                ((ImportExportStates)states).bpm = (int)newVal;
                bpmField.value = ((int)newVal).ToString();
            }
        });

        rootVisualElement.Q<Button>("import-btn").clicked += ()=>{
            lanes.Clear();

            string originalPath = EditorUtility.OpenFilePanel("Importer", "Assets/Resources/Audio", "mp3,txt");
            string extention = Path.GetExtension(originalPath);

            switch(extention){
                case ".mp3":
                    path = "Assets/Resources/Audio/" + Path.GetFileNameWithoutExtension(originalPath) + "/" + Path.GetFileName(originalPath);
                    ((ImportExportStates)states).audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    rootVisualElement.Q<TextField>("path-display").value = path;
                    bpmField.value = "120";
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
                    for(int i = 0; i < noteData.notes.Count; i++){
                        lanes.Add(new List<Note>());
                        for(int j = 0; j < noteData.notes[i].Count; j+=3){
                            float timing = noteData.notes[i][j];
                            float timing2 = noteData.notes[i][j+1];
                            float noteType = noteData.notes[i][j+2];
                            lanes[i].Add(new Note(timing, timing2, (int)noteType));
                        }
                    }
                    ((ImportExportStates)states).audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources/Audio/" + musicName + "/" + musicName + ".mp3");
                    rootVisualElement.Q<TextField>("path-display").value = path;
                    titleField.value = noteData.title;
                    artistField.value = noteData.artist;
                    bpmField.value = noteData.bpm.ToString();
                break;
            }
        };
        rootVisualElement.Q<Button>("export-btn").clicked += ()=>{
            NoteData noteData = new NoteData();
            //INPUT NOTES HERE
            for(int i = 0; i < lanes.Count; i++){
                noteData.notes.Add(new List<float>());
                for(int j = 0; j < lanes[i].Count; j++){
                    noteData.notes[i].Add(lanes[i][j].timing);
                    noteData.notes[i].Add(lanes[i][j].timing2);
                    noteData.notes[i].Add(lanes[i][j].noteType);
                }
            }
            // Debug.Log(path);
            if(path.Substring(path.Length - 5, 5) == "_data"){
                path = path.Substring(0, path.Length - 5);
            }
            noteData.fileName = Path.GetFileNameWithoutExtension(path);
            noteData.title = titleField.value;
            noteData.artist = artistField.value;
            noteData.bpm = ((ImportExportStates)states).bpm;

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