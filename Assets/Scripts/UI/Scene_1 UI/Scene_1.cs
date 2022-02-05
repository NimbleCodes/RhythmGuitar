using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using kgh.Signals;

public class Scene_1 : MonoBehaviour
{
    UIDocument uiDocument;
    VisualElement rootVisualElement;
    VisualElement noteDisplay, strings, mainDisplay, other;
    // bool geometryChanged = false;
    public NoteData _noteData{
        private set;
        get;
    }
    public AudioSource audioSource;
    bool start = false;
    List<VisualElement> noteIndicators;
    float visibleAreaSize = 3f;
    public List<Sprite> noteIndicatorSprites;
    float time;
    public float noteDelay = 0f;

    List<(int, float)> notes;

    void Awake(){
        uiDocument = GetComponent<UIDocument>();
        rootVisualElement = uiDocument.rootVisualElement;
        noteDisplay = rootVisualElement.Query<VisualElement>("note_display");
        noteIndicators = new List<VisualElement>();

        notes = new List<(int, float)>();
    }
    void Start(){
        GameManager.instance.sigs.Subscribe("audio_loaded", this, "AudioLoaded");
        GameManager.instance.sigs.Subscribe("OnMouseBehavior", this, "OnMouseBehavior");
        time = -visibleAreaSize;
    }
    void Update(){
        int visibleIndStart = -1;
        int visibleIndFin = -1;
        for(int i = 0; i < notes.Count; i++){
            if(notes[i].Item2 + noteDelay >= time && visibleIndStart == -1){
                visibleIndStart = i;
            }
            if(notes[i].Item2 + noteDelay > time + visibleAreaSize){
                visibleIndFin = i - 1;
                break;
            }
        }
        int cnt = 0;
        for(int i = visibleIndStart ; i < visibleIndFin + 1; i++){
            VisualElement noteIndicator, noteNumIndicator;
            noteNumIndicator = new VisualElement();
            if(cnt < noteIndicators.Count){
                noteIndicator = noteIndicators[cnt];
                foreach(var item in noteIndicator.Children()){
                    if(item.ClassListContains("note-num-indicator")){
                        noteNumIndicator = (VisualElement)item;
                    }
                }
            }
            else{
                noteIndicator = new VisualElement();
                noteIndicator.AddToClassList("note-indicator");
                noteNumIndicator = new VisualElement();
                noteNumIndicator.AddToClassList("note-num-indicator");
                noteIndicator.Add(noteNumIndicator);
                noteDisplay.Add(noteIndicator);
                noteIndicators.Add(noteIndicator);
            }
            if(notes[i].Item1 > 5){
                noteIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[12]);
                noteNumIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[notes[i].Item1 + 6]);
            }
            else{
                noteIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[12]);
                noteNumIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[notes[i].Item1]);
            }
            noteIndicator.style.left = noteDisplay.worldBound.width * ((notes[i].Item2 + noteDelay - time) / visibleAreaSize);
            cnt++;
        }
        while(cnt < noteIndicators.Count){
            noteIndicators[cnt].style.left = -500; 
            cnt++;
        }
        time += Time.deltaTime;
    }
    void AudioLoaded(NoteData noteData){
        _noteData = noteData;
        bool cont = true;
        while(cont){
            int minInd = -1;
            float minVal = float.MaxValue;
            for(int i = 0; i < _noteData.notes.Count; i++){
                if(_noteData.notes[i].Count == 0){
                    continue;
                }            
                if(minVal > _noteData.notes[i][0]){
                    minVal = _noteData.notes[i][0];
                    minInd = i;
                }
            }
            if(minInd == -1){
                break;
            }
            notes.Add((minInd, minVal));
            _noteData.notes[minInd].RemoveAt(0);
        }
    }
    void OnMouseBehavior(int val, int lineCount){
        
    }
}