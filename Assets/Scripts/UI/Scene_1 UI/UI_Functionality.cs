using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using KusoGame.Signals;

public class UI_Functionality : MonoBehaviour
{
    VisualElement rootVisualElement;
    VisualElement noteDisplay;
    const int poolSize = 30;
    Queue<VisualElement> noteIndicatorPool;
    Queue<VisualElement> usedNoteIndicators;
    float visibleAreaSize = 3;
    float time = -3;

    Switch userInput;

    void Start()
    {
        userInput = GameManager.instance.sigs.Register("user_input", typeof(Action<string>));
        GameManager.instance.sigs.Subscribe("user_input", this, "UserInput");

        rootVisualElement   = GetComponent<UIDocument>().rootVisualElement;
        noteDisplay         = rootVisualElement.Query<VisualElement>("note_display");
        noteIndicatorPool   = new Queue<VisualElement>();
        for(int i = 0; i < poolSize; i++){
            VisualElement newNoteIndicator = new VisualElement();
            newNoteIndicator.style.position         = Position.Absolute;
            newNoteIndicator.style.width            = 25;
            newNoteIndicator.style.height           = 25;
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
                if(noteData.notes[i][j] < time)
                    continue;
                if(noteData.notes[i][j] > time + visibleAreaSize)
                    break;
                VisualElement noteIndicator;
                if(cnt < usedNoteIndicators.Count){
                    noteIndicator = usedNoteIndicators.Dequeue();
                }
                else{
                    noteIndicator = noteIndicatorPool.Dequeue();
                }
                noteIndicator.style.left = noteDisplay.worldBound.width * ((noteData.notes[i][j] - time) / visibleAreaSize);
                switch(i){
                    case 0:
                        noteIndicator.style.backgroundColor = Color.red;
                    break;
                    case 1:
                        noteIndicator.style.backgroundColor = Color.green;
                    break;
                    case 2:
                        noteIndicator.style.backgroundColor = Color.blue;
                    break;
                    case 3:
                        noteIndicator.style.backgroundColor = Color.black;
                    break;
                }
                Debug.Log(i);
                usedNoteIndicators.Enqueue(noteIndicator);
                cnt++;
            }
        }
        while(cnt < usedNoteIndicators.Count){
            VisualElement noteIndicator = usedNoteIndicators.Dequeue();
            noteIndicator.style.left = -100;
            noteIndicatorPool.Enqueue(noteIndicator);
        }
        time += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Space)){
            userInput.Invoke("0");
        }
    }
    void UserInput(string input){
        int convInp = int.Parse(input);
        NoteData noteData = GameManager.instance.noteData;
        int curInd = 0;
        Debug.Log(noteData.notes.Count);
        while(noteData.notes[convInp][curInd] < time && curInd < noteData.notes.Count){
            curInd++;
        }
        if(curInd >= noteData.notes.Count){
            return;
        }
        float nextNoteTime = noteData.notes[convInp][curInd];
        float diff = Mathf.Abs(nextNoteTime - time);
        if(diff < 0.25){
            Debug.Log("success");
        }
        else{
            Debug.Log("fail");
        }
    }
}
