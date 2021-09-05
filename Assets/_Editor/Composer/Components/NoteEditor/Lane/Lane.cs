#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using kgh.Signals;

public class Lane : kgh.UI.Component
{
    Exchange exchange;
    Switch laneOp;
    Button remove, moveUp, moveDown;
    VisualElement notes;
    float start, size, bpm;

    public Lane(Exchange _exchange, Switch _laneOp, List<float> noteList) : base("Assets/_Editor/Composer/Components/NoteEditor/Lane/Lane.uxml"){
        laneOp = _laneOp;
        exchange = _exchange;
        exchange.Subscribe("visible_area_changed", this, "VisibleAreaChanged");
        exchange.Subscribe("bpm_changed", this, "BpmChanged");

        start = 0;
        size = 15;
        bpm = 120;

        remove = rootVisualElement.Query<Button>("remove");
        moveUp = rootVisualElement.Query<Button>("move_up");
        moveDown = rootVisualElement.Query<Button>("move_down");
        notes = rootVisualElement.Query<VisualElement>("notes");

        Color backgroundColor = rootVisualElement.style.backgroundColor.value;
        backgroundColor.a = 0.25f;
        rootVisualElement.style.backgroundColor = backgroundColor;
        remove.clicked += ()=>{
            laneOp.Invoke(0, this);
        };
        moveUp.clicked += ()=>{
            laneOp.Invoke(1, this);
        };
        moveDown.clicked += ()=>{
            laneOp.Invoke(2, this);
        };
        kgh.UI.Interactions.MouseInteractions(
            notes,
            (button, mouseDownPos)=>{
                if(button == (int)MouseButton.LeftMouse){
                    float time = start + size * mouseDownPos.x / notes.worldBound.width;
                    float packSize = 60f / bpm;
                    noteList.Add(Mathf.Floor(time / packSize) * packSize);
                    laneOp.Invoke(3, this);
                }
            },
            null,
            null, 
            (button, mouseDownPos, delta)=>{
                float startRaw = start + size * mouseDownPos.x / notes.worldBound.width;
                float endRaw = start + size * (mouseDownPos.x + delta.x) / notes.worldBound.width; 
                float packSize = 60f / bpm;
                float startTime = Mathf.Floor(startRaw / packSize) * packSize;
                float endTime = Mathf.Floor(endRaw / packSize) * packSize;
                if(button == (int)MouseButton.LeftMouse){
                    while(startTime <= endTime){
                        if(noteList.FindIndex((n)=>n==startTime) == -1){
                            noteList.Add(startTime);
                        }
                        startTime += packSize;
                    }
                }
                if(button == (int)MouseButton.RightMouse){
                    int i = 0;
                    while(i < noteList.Count){
                        if(noteList[i] < startTime){
                            i++;
                            continue;
                        }
                        if(noteList[i] > endTime){
                            break;
                        }
                        noteList.RemoveAt(i);
                    }
                }
                laneOp.Invoke(3, this);
            }
        );
    }
    void VisibleAreaChanged(float _start, float _size){
        start = _start;
        size = _size;
    }
    void BpmChanged(float _bpm){
        bpm = _bpm;
    }
}
#endif