using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NoteEditor : Component
{
    AudioSource audioSource;
    NoteData noteData;
    List<Lane> laneList;
    VisualElement lanes;
    VisualElement cursorPositionIndicator;
    public class VisibleArea{
        public float start;
        public float size;
        public VisibleArea(float _start, float _size){
            start = _start;
            size = _size;
        }
    }
    VisibleArea visibleArea;
    VisibleArea timeDisplay;
    Queue<VisualElement> timeIndicatorPool, timeIndicatorsInUse;
    const int poolSize = 60;
    const float timeIndicatorInterval = 5;
    VisualElement playPositionIndicator, endPositionIndicator;

    public NoteEditor(AudioSource _audioSource, NoteData _noteData, Observer observer) : base("Assets/Editor/Components/NoteEditor/NoteEditor.uxml"){
        audioSource = _audioSource;
        noteData    = _noteData;
        
        timeIndicatorPool      = new Queue<VisualElement>();
        timeIndicatorsInUse = new Queue<VisualElement>();
        for(int i = 0; i <  poolSize; i++){
            VisualElement newTimeIndicator = new VisualElement();
            newTimeIndicator.AddToClassList("indicator");
            newTimeIndicator.style.backgroundColor = new Color(1,1,1,0.5f);
            newTimeIndicator.style.left = -100;
            Label timeLabel = new Label();
            timeLabel.style.top = 15;
            newTimeIndicator.Add(timeLabel);
            timeIndicatorPool.Enqueue(newTimeIndicator);
            rootVisualElement.Add(newTimeIndicator);
        }
        
        playPositionIndicator   = rootVisualElement.Query<VisualElement>("playPosition_indicator");
        endPositionIndicator    = rootVisualElement.Query<VisualElement>("endPosition_indicator");
        cursorPositionIndicator = rootVisualElement.Query<VisualElement>("cursorPosition_indicator");
        playPositionIndicator.style.left    = -1;
        endPositionIndicator.style.left     = -1;
        cursorPositionIndicator.style.left  = -1;
        
        visibleArea = new VisibleArea(0, 60);
        
        lanes       = rootVisualElement.Query<VisualElement>("lanes");
        laneList    =  new List<Lane>();
        lanes.RegisterCallback<WheelEvent>((e)=>{
            float temp = Mathf.Abs(visibleArea.size + e.delta.y * 10f);
            if(temp <= 0 || temp >= 180){
                return;
            }
            visibleArea.size += e.delta.y * 10f;
        });
        lanes.RegisterCallback<MouseMoveEvent>((e)=>{
            cursorPositionIndicator.style.left = e.localMousePosition.x - 2;
        });
        for(int i = 0; i < 4; i++){
            laneList.Add(new Lane(audioSource, noteData.notes[i], noteData.events, visibleArea, observer));
        }
        laneList.ForEach((l)=>{
            l.rootVisualElement.style.height = Length.Percent(25);
            lanes.Add(l.rootVisualElement);
        });

        observer.Subscribe("update",    Update);
        //observer.Subscribe("destroy",   Destroy);
    }
    void Update(System.Object[] parameters){
        visibleArea.start = audioSource.time;
        if(audioSource.clip != null){
            float i = 0;
            int cnt = 0;
            while(i < visibleArea.start) i += timeIndicatorInterval;
            while(i >= visibleArea.start && i < visibleArea.start + visibleArea.size){
                VisualElement temp;
                if(cnt < timeIndicatorsInUse.Count){
                    temp = timeIndicatorsInUse.Dequeue();
                }
                else{
                    temp = timeIndicatorPool.Dequeue();
                }
                temp.style.left = rootVisualElement.worldBound.width * ((i - visibleArea.start) / visibleArea.size);
                Label timeLabel = temp.Query<Label>();
                timeLabel.text  = ((int)(Mathf.Floor(i / 60))).ToString() + ":" 
                                    + ((int)(i % 60)).ToString("00");
                timeIndicatorsInUse.Enqueue(temp);
                cnt++;
                i += timeIndicatorInterval;
            }
            while(cnt < timeIndicatorsInUse.Count){
                VisualElement temp = timeIndicatorsInUse.Dequeue();
                temp.style.left = -100;
                timeIndicatorPool.Enqueue(temp);
            }

            //playPositionIndicator
            Label tempLabel = playPositionIndicator.Query<Label>("time_label");
            tempLabel.text = SecToString(visibleArea.start);
            //endPositionIndicator
            tempLabel = endPositionIndicator.Query<Label>("time_label");
            tempLabel.text = SecToString(visibleArea.start + visibleArea.size);
            endPositionIndicator.style.left = rootVisualElement.worldBound.width;
            if(audioSource.clip.length >= visibleArea.start && audioSource.clip.length <= visibleArea.start + visibleArea.size){
                tempLabel.text = SecToString(audioSource.clip.length);
                endPositionIndicator.style.left = rootVisualElement.worldBound.width * ((audioSource.clip.length - visibleArea.start) / visibleArea.size);
            }
        }
    }
    string SecToString(float sec){
        return ((int)(Mathf.Floor(sec / 60))).ToString() + ":" 
            + ((int)(sec % 60)).ToString("00");
    }
    void Destroy(System.Object[] parameters){

    }
}