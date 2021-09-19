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
    NoteData noteData;
    public AudioSource audioSource;
    bool start = false;
    List<VisualElement> noteIndicators;
    float visibleAreaSize = 3f;
    public List<Sprite> noteIndicatorSprites;
    float time;

    void Awake(){
        uiDocument = GetComponent<UIDocument>();
        rootVisualElement = uiDocument.rootVisualElement;
        noteDisplay = rootVisualElement.Query<VisualElement>("note_display");
        noteIndicators = new List<VisualElement>();
    }
    void Start(){
        GameManager.instance.sigs.Subscribe("audio_loaded", this, "AudioLoaded");
        time = -visibleAreaSize;
    }
    void Update(){
        if(noteData != null){
            int cnt = 0;
            for(int i = 0; i < noteData.notes.Count; i++){
                for(int j = 0; j < noteData.notes[i].Count; j++){
                    if(noteData.notes[i][j] >= time && noteData.notes[i][j] < time + visibleAreaSize){
                        VisualElement noteIndicator;
                        if(cnt < noteIndicators.Count){
                            noteIndicator = noteIndicators[cnt];
                        }
                        else{
                            noteIndicator = new VisualElement();
                            noteIndicator.AddToClassList("note-indicator");
                            noteDisplay.Add(noteIndicator);
                            noteIndicators.Add(noteIndicator);
                        }
                        noteIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[i]);
                        noteIndicator.style.left = noteDisplay.worldBound.width * ((noteData.notes[i][j] - time) / visibleAreaSize);
                        cnt++;
                    }
                    if(noteData.notes[i][j] >= time + visibleAreaSize){
                        break;
                    }
                }
                while(cnt < noteIndicators.Count){
                    noteIndicators[cnt].style.left = -50;
                    cnt++;
                }
            }
            time += Time.deltaTime;
        }
    }
    void AudioLoaded(NoteData _noteData){
        noteData = _noteData;
        Debug.Log(noteData.notes.Count);
    }
}