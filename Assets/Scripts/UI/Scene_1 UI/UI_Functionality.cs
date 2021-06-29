using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class UI_Functionality : MonoBehaviour
{
    VisualElement rootVisualElement;
    VisualElement noteDisplay;
    const int poolSize = 30;
    Queue<VisualElement> noteIndicatorPool;
    Queue<VisualElement> usedNoteIndicators;
    float visibleAreaSize = 10;

    void Start()
    {
        rootVisualElement   = GetComponent<UIDocument>().rootVisualElement;
        noteDisplay         = rootVisualElement.Query<VisualElement>("note_display");
        noteIndicatorPool   = new Queue<VisualElement>();
        for(int i = 0; i < poolSize; i++){
            VisualElement newNoteIndicator = new VisualElement();
            newNoteIndicator.style.position         = Position.Absolute;
            newNoteIndicator.style.width            = 50;
            newNoteIndicator.style.height           = 50;
            newNoteIndicator.style.backgroundColor  = Color.red;
            newNoteIndicator.style.left             = -100;
            noteIndicatorPool.Enqueue(newNoteIndicator);
            noteDisplay.Add(newNoteIndicator);
        }
        usedNoteIndicators  = new Queue<VisualElement>();
    }
    void Update(){
        AudioSource audioSource = GameManager.instance.audioSource;
        NoteData noteData = GameManager.instance.noteData;
        int cnt = 0;
        for(int i = 0; i < noteData.notes.Count; i++){
            for(int j = 0; j < noteData.notes[i].Count; j++){
                if(noteData.notes[i][j] < audioSource.time)
                    continue;
                if(noteData.notes[i][j] > audioSource.time + visibleAreaSize)
                    break;
                if(cnt < usedNoteIndicators.Count){

                }
                else{
                    
                }
            }
        }
    }
}
