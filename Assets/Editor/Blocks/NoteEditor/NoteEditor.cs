using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class NoteEditor : Block
{
    AudioSource audioSource;
    float visibleAreaStart = 0;
    float visibleAreaSize = 60;
    float minVisibleAreaSize = 60;
    NoteData noteData;

    const int poolSize = 50;
    Queue<VisualElement> sectionIndicatorPool;
    Queue<VisualElement> usedSectionIndicators;
    VisualElement timeDisplay, lanes, playPositionIndicator;
    
    public NoteEditor(AudioSource _audioSource) : base("Assets/Editor/Blocks/NoteEditor/NoteEditor.uxml")
    {
        rootVisualElement.name = "note_editor";
        audioSource = _audioSource;
        noteData = new NoteData();
        sectionIndicatorPool = new Queue<VisualElement>();
        usedSectionIndicators = new Queue<VisualElement>();

        if((timeDisplay = rootVisualElement.Query<VisualElement>("time_display")) != null){
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
        if((lanes = rootVisualElement.Query<VisualElement>("lanes")) != null){
            
        }
        if((playPositionIndicator = rootVisualElement.Query<VisualElement>("playPosition_indicator")) != null){
            playPositionIndicator.style.backgroundColor = Color.red;
            playPositionIndicator.style.left = -1;
        }
    }
    void UpdateTimeDisplay()
    {
        float sectionSpace = visibleAreaSize / 20;
        float i = 0, cnt = 0;
        while(i < visibleAreaStart) i+=sectionSpace;
        while(i < visibleAreaStart + visibleAreaSize)
        {
            if(cnt < usedSectionIndicators.Count)
            {
                VisualElement temp = usedSectionIndicators.Dequeue();
                temp.style.left = ((i - visibleAreaStart) / (visibleAreaSize)) * timeDisplay.worldBound.width;
                Label temp2 = temp.Query<Label>("timeLabel");
                temp2.text = ((int)(i / 60)).ToString() + ":" + (i % 60).ToString("00");
                usedSectionIndicators.Enqueue(temp);
            }
            else    
            {
                VisualElement temp = sectionIndicatorPool.Dequeue();
                temp.style.left = ((i - visibleAreaStart) / (visibleAreaSize)) * timeDisplay.worldBound.width;
                Label temp2 = temp.Query<Label>("timeLabel");
                temp2.text = ((int)(i / 60)).ToString() + ":" + (i % 60).ToString();
                usedSectionIndicators.Enqueue(temp);
            }
            i += sectionSpace;
            cnt++;
        }
        while(cnt < usedSectionIndicators.Count)
        {
            VisualElement temp = usedSectionIndicators.Dequeue();
            temp.style.left =-1;
            sectionIndicatorPool.Enqueue(temp);
        }
    }
    public override void Update()
    {
        base.Update();
        if(audioSource.clip != null)
        {
            if(audioSource.time + visibleAreaSize < audioSource.clip.length)
                visibleAreaStart = audioSource.time;
            UpdateTimeDisplay();
            //BUG: 스킵 기능 실행시 위치가 올바르지 못한 버그
            playPositionIndicator.style.left = timeDisplay.worldBound.width * ((audioSource.time - visibleAreaStart) / visibleAreaSize);
        }
    }
    public override void Destroy()
    {
        base.Destroy();
    }
}
class Lane : Block
{
    public Lane() : base("Assets/Editor/Blocks/NoteEditor/Lane.uxml")
    {

    }
}