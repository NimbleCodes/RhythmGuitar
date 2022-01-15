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
    bool geometryChanged = false;
    public NoteData noteData{
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

    void Awake(){
        uiDocument = GetComponent<UIDocument>();
        rootVisualElement = uiDocument.rootVisualElement;
        noteDisplay = rootVisualElement.Query<VisualElement>("note_display");
        noteIndicators = new List<VisualElement>();
    }
    void Start(){
        GameManager.instance.sigs.Subscribe("audio_loaded", this, "AudioLoaded");
        GameManager.instance.sigs.Subscribe("OnMouseBehavior", this, "OnMouseBehavior");
        time = -visibleAreaSize;
    }
    void Update(){
        if(noteData != null){
            int cnt = 0;
            for(int i = 0; i < noteData.notes.Count; i++){
                for(int j = 0; j < noteData.notes[i].Count; j++){
                    if(noteData.notes[i][j] + noteDelay >= time && noteData.notes[i][j] + noteDelay < time + visibleAreaSize){
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
                        if(i > 5){
                            noteIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[13]);
                            noteNumIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[i + 6]);
                        }
                        else{
                            noteIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[12]);
                            noteNumIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[i]);
                        }
                        noteIndicator.style.left = noteDisplay.worldBound.width * ((noteData.notes[i][j] + noteDelay - time) / visibleAreaSize);
                        cnt++;
                    }
                    if(noteData.notes[i][j] + noteDelay >= time + visibleAreaSize){
                        break;
                    }
                }
                while(cnt < noteIndicators.Count){
                    noteIndicators[cnt].style.left = -500; 
                    cnt++;
                }
            }
            time += Time.deltaTime;
        }
        // int closestNote = -1;
        // float closestNoteVal = float.MaxValue;
        // for(int i = 0; i < noteData.notes.Count; i++){
        //     for(int j = 0; j < noteData.notes[i].Count; j++){
        //         if(noteData.notes[i][j] < audioSource.time)
        //             continue;
        //         if(noteData.notes[i][j] >= audioSource.time + visibleAreaSize)
        //             break;
        //         if(noteData.notes[i][j] < closestNoteVal){
        //             closestNote = i;
        //             closestNoteVal = noteData.notes[i][j];
        //         }
        //     }
        // }
        //Debug.Log(closestNote);
    }
    void AudioLoaded(NoteData _noteData){
        noteData = _noteData;
        // Debug.Log(noteData.notes.Count);
    }
    void OnMouseBehavior(int val, int lineCount){
        
    }
}