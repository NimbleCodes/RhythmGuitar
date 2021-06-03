using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;

public class NoteEditor : Block
{
    AudioSource audioSource;
    float visibleAreaStart = 0;
    float visibleAreaSize = 60;
    const float minVisibleAreaSize = 10;
    NoteData noteData;

    const int poolSize = 50;
    Queue<VisualElement> sectionIndicatorPool;
    Queue<VisualElement> usedSectionIndicators;

    Queue<VisualElement> noteIndicatorPool;
    Queue<VisualElement> usedNoteIndicators;

    VisualElement timeDisplay, lanes, playPositionIndicator, cursorPositionIndicator, endPositionIndicator;

    const int defNumLanes = 4;
    List<VisualElement> laneList;
    
    public NoteEditor(AudioSource _audioSource) : base("Assets/Editor/Blocks/NoteEditor/NoteEditor.uxml")
    {
        rootVisualElement.name = "note_editor";
        audioSource = _audioSource;
        noteData = new NoteData();
        sectionIndicatorPool = new Queue<VisualElement>();
        usedSectionIndicators = new Queue<VisualElement>();
        noteIndicatorPool = new Queue<VisualElement>();
        usedNoteIndicators = new Queue<VisualElement>();
        laneList = new List<VisualElement>();
        var laneVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Blocks/NoteEditor/Lane.uxml");

        if((timeDisplay = rootVisualElement.Query<VisualElement>("time_display")) != null)
        {
            timeDisplay.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1);
            for(int i = 0; i < poolSize; i++){
                VisualElement newSectionIndicator = new VisualElement();
                newSectionIndicator.AddToClassList("indicator");
                newSectionIndicator.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1);
                newSectionIndicator.style.left = -1;
                newSectionIndicator.style.height = Length.Percent(50);
                Label newLabel = new Label("");
                newLabel.name = "timeLabel";
                newSectionIndicator.Add(newLabel);
                timeDisplay.Add(newSectionIndicator);
                sectionIndicatorPool.Enqueue(newSectionIndicator);
            }
        }
        if((lanes = rootVisualElement.Query<VisualElement>("lanes")) != null)
        {
            for(int i = 0; i < defNumLanes; i++)
            {
                VisualElement newLane = laneVisualTreeAsset.Instantiate();
                newLane.style.height = Length.Percent(100f/defNumLanes);
                newLane.style.borderTopWidth = 1;
                newLane.style.borderBottomWidth = 1;
                newLane.style.borderTopColor = new Color(0.5f, 0.5f, 0.5f, 1);
                newLane.style.borderBottomColor = new Color(0.5f, 0.5f, 0.5f, 1);
                newLane.name = i.ToString();
                newLane.RegisterCallback<MouseDownEvent>((e)=>{
                    if(audioSource.clip == null) return;
                    int laneNum = int.Parse(newLane.name);
                    float time = ((e.localMousePosition.x / lanes.worldBound.width) * visibleAreaSize) + visibleAreaStart;
                    noteData.notes[laneNum].Add(time);
                });
                lanes.Add(newLane);
                laneList.Add(newLane);
                noteData.notes.Add(new List<float>());
            }
            for(int i = 0; i < poolSize; i++)
            {
                VisualElement newNoteIndicator = new VisualElement();
                newNoteIndicator.AddToClassList("indicator");
                newNoteIndicator.style.backgroundColor = Color.magenta;
                newNoteIndicator.style.left = -1;
                newNoteIndicator.style.height = Length.Percent(100);
                noteIndicatorPool.Enqueue(newNoteIndicator);
            }
            lanes.RegisterCallback<MouseMoveEvent>((e)=>{
                cursorPositionIndicator.style.left = e.localMousePosition.x;
            });
            lanes.RegisterCallback<WheelEvent>((e)=>{
                visibleAreaSize += (e.delta.y < 0 ? -1 : 1) * 10;
                if(visibleAreaSize < minVisibleAreaSize)
                {
                    visibleAreaSize = minVisibleAreaSize;
                    return;
                }
            });
        }
        if((playPositionIndicator = rootVisualElement.Query<VisualElement>("playPosition_indicator")) != null)
        {
            playPositionIndicator.style.backgroundColor = Color.red;
            playPositionIndicator.style.left = -1;
        }
        if((cursorPositionIndicator = lanes.Query<VisualElement>("cursorPosition_indicator")) != null)
        {
            cursorPositionIndicator.style.backgroundColor = new Color(0.6f, 0.6f, 1);
            cursorPositionIndicator.style.left = -1;
        }
        if((endPositionIndicator = rootVisualElement.Query<VisualElement>("endPosition_indicator")) != null){
            endPositionIndicator.style.backgroundColor = Color.red;
            endPositionIndicator.style.right = -1;
            ((Label)endPositionIndicator.Query<Label>("time_label")).style.color = Color.red;
        }
    }
    void UpdateTimeDisplay()
    {
        float sectionSpace = visibleAreaSize / 20;
        float i = 0, cnt = 0;
        while(i < visibleAreaStart) i+=sectionSpace;
        while(i < visibleAreaStart + visibleAreaSize && i < audioSource.clip.length)
        {
            if(cnt < usedSectionIndicators.Count)
            {
                VisualElement temp = usedSectionIndicators.Dequeue();
                temp.style.left = ((i - visibleAreaStart) / (visibleAreaSize)) * timeDisplay.worldBound.width;
                Label temp2 = temp.Query<Label>("timeLabel");
                temp2.text = ((int)(Math.Floor(i / 60))).ToString() + ":" 
                                + ((int)(i % 60)).ToString("00") + ":" 
                                + ((int)((i - (int)i) * 100)).ToString("00");
                usedSectionIndicators.Enqueue(temp);
            }
            else
            {
                VisualElement temp = sectionIndicatorPool.Dequeue();
                temp.style.left = ((i - visibleAreaStart) / (visibleAreaSize)) * timeDisplay.worldBound.width;
                Label temp2 = temp.Query<Label>("timeLabel");
                temp2.text = ((int)(Math.Floor(i / 60))).ToString() + ":" 
                                + ((int)(i % 60)).ToString("00") + ":" 
                                + ((int)((i - (int)i) * 100)).ToString("00");
                usedSectionIndicators.Enqueue(temp);
            }
            i += sectionSpace;
            cnt++;
        }
        if(i >= audioSource.clip.length)
        {
            endPositionIndicator.style.left = ((audioSource.clip.length - visibleAreaStart) / (visibleAreaSize)) * timeDisplay.worldBound.width;
            Label temp = endPositionIndicator.Query<Label>("time_label");
            temp.text = ((int)(Math.Floor(audioSource.clip.length / 60))).ToString() + ":" 
                                + ((int)(audioSource.clip.length % 60)).ToString("00") + ":" 
                                + ((int)((audioSource.clip.length - (int)audioSource.clip.length) * 100)).ToString("00");
            temp.style.left = -45;
        }
        else
        {
            endPositionIndicator.style.left = timeDisplay.worldBound.width - 1;
            Label temp = endPositionIndicator.Query<Label>("time_label");
            temp.text = ((int)(Math.Floor((visibleAreaStart + visibleAreaSize) / 60))).ToString() + ":" 
                                + ((int)((visibleAreaStart + visibleAreaSize) % 60)).ToString("00") + ":" 
                                + ((int)(((visibleAreaStart + visibleAreaSize) - (int)(visibleAreaStart + visibleAreaSize)) * 100)).ToString("00");
            temp.style.left = -45;
        }
        while(cnt < usedSectionIndicators.Count)
        {
            VisualElement temp = usedSectionIndicators.Dequeue();
            temp.style.left =-1;
            ((Label)temp.Query<Label>("timeLabel")).text = "";
            sectionIndicatorPool.Enqueue(temp);
        }
    }
    void UpdateLaneDisplay(){
        int cnt = 0;
        for(int i = 0; i < defNumLanes; i++){
            noteData.notes[i].Sort();
            int j = 0;
            while(j < noteData.notes[i].Count && noteData.notes[i][j] < visibleAreaStart) j++;
            while(j < noteData.notes[i].Count && noteData.notes[i][j] < visibleAreaStart + visibleAreaSize)
            {
                if(cnt < usedNoteIndicators.Count)
                {
                    VisualElement temp = usedNoteIndicators.Dequeue();
                    temp.style.left = ((noteData.notes[i][j] - visibleAreaStart) / visibleAreaSize) * lanes.worldBound.width;
                    usedNoteIndicators.Enqueue(temp);
                    laneList[i].Add(temp);
                }
                else
                {
                    VisualElement temp = noteIndicatorPool.Dequeue();
                    temp.style.left = ((noteData.notes[i][j] - visibleAreaStart) / visibleAreaSize) * lanes.worldBound.width;
                    usedNoteIndicators.Enqueue(temp);
                    laneList[i].Add(temp);
                }
                cnt++;
                j++;
            }
            while(cnt < usedNoteIndicators.Count)
            {
                VisualElement temp = usedNoteIndicators.Dequeue();
                temp.style.left =-1;
                noteIndicatorPool.Enqueue(temp);
            }
        }
    }
    public override void Update()
    {
        base.Update();
        if(audioSource.clip != null)
        {
            visibleAreaStart = audioSource.time;
            UpdateTimeDisplay();
            UpdateLaneDisplay();
            playPositionIndicator.style.left = timeDisplay.worldBound.width * ((audioSource.time - visibleAreaStart) / visibleAreaSize);
            Label temp = playPositionIndicator.Query<Label>("time_label");
            float i = audioSource.time;
            temp.text = ((int)(Math.Floor(i / 60))).ToString() + ":" 
                                + ((int)(i % 60)).ToString("00") + ":" 
                                + ((int)((i - (int)i) * 100)).ToString("00");
            temp.style.color = Color.red;
        }
    }
    public override void Destroy()
    {
        base.Destroy();
    }
}