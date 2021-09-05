using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class Lane : Component
{
    AudioSource audioSource;
    List<float> notes;
    NoteEditor.VisibleArea visibleArea;
    Queue<VisualElement> noteIndicatorPool, notesInUse;
    const int poolSize = 100;
    float mLDownTime = -1, mLUpTime = -1;
    float mRDownTime = -1, mRUpTime = -1;
    VisualElement dragBox;

    public Lane(AudioSource _audioSource, List<float> _notes, List<(float, string)> events, NoteEditor.VisibleArea _visibleArea, Observer observer) : base("Assets/Editor/Components/NoteEditor/Lanes/Lane.uxml"){
        audioSource         = _audioSource;
        notes               = _notes;
        visibleArea         = _visibleArea;
        dragBox             = rootVisualElement.Query<VisualElement>("dragBox");
        dragBox.style.left  = -10;
        dragBox.style.width = 0;
        notesInUse          = new Queue<VisualElement>();
        noteIndicatorPool   = new Queue<VisualElement>();
        for(int i = 0; i < poolSize; i++){
            VisualElement newNoteIndicator = new VisualElement();
            newNoteIndicator.AddToClassList("indicator");
            newNoteIndicator.style.backgroundColor = Color.magenta;
            newNoteIndicator.style.left = -1;
            newNoteIndicator.BringToFront();
            noteIndicatorPool.Enqueue(newNoteIndicator);
            rootVisualElement.Add(newNoteIndicator);
        }

        rootVisualElement.RegisterCallback<MouseDownEvent>((e)=>{
            if(audioSource.clip != null){
                switch(e.button){
                    case (int)MouseButton.LeftMouse:
                        mLDownTime = visibleArea.start + visibleArea.size * (e.localMousePosition.x / rootVisualElement.worldBound.width);
                        break;
                    case (int)MouseButton.RightMouse:
                        mRDownTime = visibleArea.start + visibleArea.size * (e.localMousePosition.x / rootVisualElement.worldBound.width);
                        break;
                }
            }
        });
        rootVisualElement.RegisterCallback<MouseMoveEvent>((e)=>{
            if(audioSource.clip != null){
                float downTime;
                if(mLDownTime != -1){
                    downTime = mLDownTime;
                }
                else if(mRDownTime != -1){
                    downTime = mRDownTime;
                }
                else{
                    return;
                }
                float mPosTime = visibleArea.start + visibleArea.size * (e.localMousePosition.x / rootVisualElement.worldBound.width);
                if(downTime > mPosTime){
                    dragBox.style.left  = rootVisualElement.worldBound.width * ((mPosTime - visibleArea.start) / visibleArea.size);
                    dragBox.style.width = rootVisualElement.worldBound.width * ((downTime - visibleArea.start) / visibleArea.size) - dragBox.style.left.value.value;
                }
                else{
                    dragBox.style.left  = rootVisualElement.worldBound.width * ((downTime - visibleArea.start) / visibleArea.size);
                    dragBox.style.width = rootVisualElement.worldBound.width * ((mPosTime - visibleArea.start) / visibleArea.size) - dragBox.style.left.value.value;
                }
            }
        });
        rootVisualElement.RegisterCallback<MouseUpEvent>((e)=>{
            if(audioSource.clip != null){
                switch(e.button){
                    case (int)MouseButton.LeftMouse:
                        mLUpTime = visibleArea.start + visibleArea.size * (e.localMousePosition.x / rootVisualElement.worldBound.width);
                        notes.Add(mLDownTime);
                        break;
                    case (int)MouseButton.RightMouse:
                        mRUpTime = visibleArea.start + visibleArea.size * (e.localMousePosition.x / rootVisualElement.worldBound.width);
                        if(mRDownTime > mRUpTime){
                            float temp  = mRDownTime;
                            mRDownTime  = mRUpTime;
                            mRUpTime    = temp;
                        }
                        if(mRUpTime - mRDownTime > 0.05f){
                            int i = 0;
                            while(i < notes.Count && notes[i] < mRDownTime) i++;
                            while(i < notes.Count && notes[i] >= mRDownTime && notes[i] < mRUpTime){
                                notes.RemoveAt(i);
                            }
                        }
                        else{
                            
                        }
                        break;
                }
                mLDownTime = mLUpTime = mRDownTime = mRUpTime = -1;
                dragBox.style.left = -10;
                dragBox.style.width = 0;
            }
        });

        observer.Subscribe("update",    Update);
        //observer.Subscribe("destroy",   Destroy);
    }
    void Update(System.Object[] parameters){
        if(audioSource.clip != null){
            //Update note indicators
            notes.Sort();
            int i = 0, cnt = 0;
            while(i < notes.Count && notes[i] < visibleArea.start) i++;
            while(i < notes.Count && notes[i] >= visibleArea.start && notes[i] < visibleArea.start + visibleArea.size){
                if(cnt < notesInUse.Count){
                    VisualElement temp = notesInUse.Dequeue();
                    temp.style.left = ((notes[i] - visibleArea.start) / visibleArea.size) * rootVisualElement.worldBound.width;
                    notesInUse.Enqueue(temp);
                }
                else{
                    VisualElement temp = noteIndicatorPool.Dequeue();
                    temp.style.left = ((notes[i] - visibleArea.start) / visibleArea.size) * rootVisualElement.worldBound.width;
                    notesInUse.Enqueue(temp);
                }
                cnt++;
                i++;
            }
            while(cnt < notesInUse.Count){
                VisualElement temp = notesInUse.Dequeue();
                temp.style.left =-1;
                noteIndicatorPool.Enqueue(temp);
            }
        }
    }
    void Destroy(System.Object[] parameters){

    }
}