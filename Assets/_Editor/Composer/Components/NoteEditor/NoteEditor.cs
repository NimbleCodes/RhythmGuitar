#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using kgh.Signals;

public class NoteEditor : kgh.UI.Component
{
    Exchange exchange;
    Switch laneOp, bpmChanged;
    VisualElement laneDisplay, nextInsPos;
    ScrollView scrollView;
    Button addLaneBtn, exportBtn;
    TextField bpmInputField;
    static class VisibleArea{
        public static float start = 0;
        public static float size = 15;
    }
    int bpm = 120;
    List<Lane> lanes;
    List<VisualElement> timeIndicators;
    List<VisualElement> noteIndicators;
    NoteData noteData;
    DataIO dataIo;

    public NoteEditor(Exchange _exchange) : base("Assets/_Editor/Composer/Components/NoteEditor/NoteEditor.uxml"){
        rootVisualElement.name = "note_editor";
        nextInsPos = rootVisualElement.Query<VisualElement>("next_ins_pos");
        scrollView = rootVisualElement.Query<ScrollView>("scroll_view");
        addLaneBtn = rootVisualElement.Query<Button>("add_lane_btn");
        bpmInputField = rootVisualElement.Query<TextField>("bpm_input_field");
        laneDisplay = rootVisualElement.Query<VisualElement>("lane_display");
        exportBtn = rootVisualElement.Query<Button>("export_btn");

        lanes = new List<Lane>();
        timeIndicators = new List<VisualElement>();
        noteData = new NoteData();
        noteIndicators = new List<VisualElement>();
        dataIo = new DataIO(noteData);

        exchange = _exchange;
        exchange.Subscribe("start", this, "Start");
        laneOp = exchange.Register("lane_op", typeof(Action<int, Lane>));
        bpmChanged = exchange.Register("bpm_changed", typeof(Action<float>));
    }
    void Start(){
        bpmInputField.value = bpm.ToString();
        bpmInputField.RegisterValueChangedCallback<string>((e)=>{
            int result;
            if(!int.TryParse(e.newValue, out result)){
                bpmInputField.value = e.previousValue;
                return;
            }
            bpm = result;
            bpmInputField.value = bpm.ToString();
            VisibleAreaChanged(VisibleArea.start, VisibleArea.size);
            bpmChanged.Invoke(bpm);
        });
        addLaneBtn.clicked += ()=>{
            noteData.notes.Add(new List<float>());
            Lane newLane = new Lane(exchange, laneOp, noteData.notes[noteData.notes.Count - 1]);
            newLane.rootVisualElement.style.height = 75;
            lanes.Add(newLane);
            scrollView.Add(newLane.rootVisualElement);
            for(int i = 0; i < timeIndicators.Count; i++){
                timeIndicators[i].style.height = Mathf.Clamp(75 * lanes.Count, 0, laneDisplay.worldBound.height);
            }
        };
        exportBtn.clicked += ()=>{
            dataIo.Save();
        };

        exchange.Subscribe("visible_area_changed", this, "VisibleAreaChanged");
        exchange.Subscribe("lane_op", this, "LaneOp");
        exchange.Subscribe("load_from_saved_file", this, "LoadFromSavedFile");
        exchange.Subscribe("audio_loaded", this, "AudioLoaded");
    }
    void VisibleAreaChanged(float start, float size){
        VisibleArea.start = start;
        VisibleArea.size = size;
        RedrawLaneDisplay();
    }
    void LaneOp(int op, Lane callingLane){
        int ind = lanes.FindIndex((l)=>{return (l == callingLane);});
        switch(op){
            case 0:
                //REMOVE
                lanes.RemoveAt(ind);
                noteData.notes.RemoveAt(ind);
            break;
            case 1:
                //MOVE UP
                if(ind > 0){
                    Lane swap = lanes[ind - 1];
                    lanes[ind - 1] = lanes[ind];
                    lanes[ind] = swap;
                    
                    List<float> swap2 = noteData.notes[ind - 1];
                    noteData.notes[ind - 1] = noteData.notes[ind];
                    noteData.notes[ind] = swap2;
                }
            break;
            case 2:
                //MOVE DOWN
                if(ind < lanes.Count - 1){
                    Lane swap = lanes[ind + 1];
                    lanes[ind + 1] = lanes[ind];
                    lanes[ind] = swap;
                    
                    List<float> swap2 = noteData.notes[ind + 1];
                    noteData.notes[ind + 1] = noteData.notes[ind];
                    noteData.notes[ind] = swap2;
                }
            break;
        }
        RedrawLaneDisplay();
    }
    void RedrawLaneDisplay(){
        //REDRAW TIME INDICATORS
        float packSize = 60f / bpm;
        int n = Mathf.CeilToInt(VisibleArea.start / packSize);
        float time = n * packSize;
        int cnt = 0;
        while(time < VisibleArea.start + VisibleArea.size){
            if(cnt >= timeIndicators.Count){
                VisualElement newTimeIndicator = new VisualElement();
                newTimeIndicator.AddToClassList("time-indicator");
                newTimeIndicator.style.left = laneDisplay.worldBound.width * 0.125f + laneDisplay.worldBound.width * 0.875f * ((time - VisibleArea.start) / VisibleArea.size);
                newTimeIndicator.style.height = Mathf.Clamp(75 * lanes.Count, 0, laneDisplay.worldBound.height);
                timeIndicators.Add(newTimeIndicator);
                laneDisplay.Add(newTimeIndicator);
            }
            else{
                timeIndicators[cnt].style.left = laneDisplay.worldBound.width * 0.125f + laneDisplay.worldBound.width * 0.875f * ((time - VisibleArea.start) / VisibleArea.size);
                timeIndicators[cnt].style.height = Mathf.Clamp(75 * lanes.Count, 0, laneDisplay.worldBound.height);
            }
            cnt++;
            time += packSize;
        }
        while(cnt < timeIndicators.Count){
            timeIndicators[cnt].style.left = -500;
            cnt++;
        }
        //REDRAW LANES
        scrollView.Clear();
        scrollView.BringToFront();
        for(int i = 0; i < lanes.Count; i++){
            scrollView.Add(lanes[i].rootVisualElement);
        }
        //REDRAW NOTE INDICATORS
        cnt = 0;
        for(int i = 0; i < lanes.Count; i++){
            noteData.notes[i].Sort();
            for(int j = 0; j < noteData.notes[i].Count; j++){
                if(noteData.notes[i][j] < VisibleArea.start){
                    continue;
                }    
                if(noteData.notes[i][j] > VisibleArea.start + VisibleArea.size){
                    break;
                }
                if(cnt >= noteIndicators.Count){
                    VisualElement newNoteIndicator = new VisualElement();
                    newNoteIndicator.AddToClassList("note-indicator");
                    newNoteIndicator.style.left = -5 + laneDisplay.worldBound.width * 0.125f + laneDisplay.worldBound.width * 0.875f * (noteData.notes[i][j] - VisibleArea.start) / VisibleArea.size;
                    newNoteIndicator.style.top = -7.5f + 37.5f + 75f * i;
                    laneDisplay.Add(newNoteIndicator);
                    noteIndicators.Add(newNoteIndicator);
                }
                else{
                    noteIndicators[cnt].style.left = -5 + laneDisplay.worldBound.width * 0.125f + laneDisplay.worldBound.width * 0.875f * (noteData.notes[i][j] - VisibleArea.start) / VisibleArea.size;
                    noteIndicators[cnt].style.top = -7.5f + lanes[i].rootVisualElement.worldBound.height * 0.5f + lanes[i].rootVisualElement.worldBound.height * i;
                }
                cnt++;
            }
        }
        while(cnt < noteIndicators.Count){
            noteIndicators[cnt].style.left = -500;
            cnt++;
        }
    }
    string LoadFromSavedFile(string path){
        dataIo.Load(path);
        lanes.Clear();
        scrollView.Clear();
        for(int i = 0; i < noteData.notes.Count; i++){
            Lane newLane = new Lane(exchange, laneOp, noteData.notes[i]);
            newLane.rootVisualElement.style.height = 75;
            lanes.Add(newLane);
            scrollView.Add(newLane.rootVisualElement);
        }
        return "Assets/Resources/" + noteData.fileName + "/" + noteData.fileName + ".mp3";
    }
    void AudioLoaded(string fileName){
        noteData.fileName = fileName;
    }
}
#endif