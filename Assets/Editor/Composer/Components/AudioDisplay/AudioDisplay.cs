using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioDisplay : Component
{
    Observer observer;
    Switch audioJump, visibleAreaChanged;
    VisualElement wrapper, playPosIndicator, cursorPosIndicator;
    List<VisualElement> sampleIndicatorList, timeIndicatorList;
    float[] samples, channel1;
    float length;
    int frequency, channels;
    float bpm = 120;
    static class VisibleArea{
        public static float start   = 0;
        public static float size    = 20;
    }
    static class MouseState{
        public static float leftDownTime;
        public static bool leftIsDrag = false;
        public static float rightDownTime;
        public static bool rightIsDrag = false;
    }

    public AudioDisplay(Observer _observer) : base("Assets/Editor/Composer/Components/AudioDisplay/AudioDisplay.uxml"){
        rootVisualElement.name  = "audio_display";
        wrapper                 = rootVisualElement.Query<VisualElement>("wrapper");
        playPosIndicator        = rootVisualElement.Query<VisualElement>("playPos_indicator");
        cursorPosIndicator      = rootVisualElement.Query<VisualElement>("cursorPos_indicator");
        wrapper.RegisterCallback<MouseDownEvent>(WrapperMouseDown);
        wrapper.RegisterCallback<MouseMoveEvent>(WrapperMouseMove);
        wrapper.RegisterCallback<MouseUpEvent>(WrapperMouseUp);
        wrapper.RegisterCallback<MouseLeaveEvent>(WrapperMouseLeave);

        sampleIndicatorList = new List<VisualElement>();
        timeIndicatorList   = new List<VisualElement>();
        for(int i = 0; i < 120; i++){
            VisualElement timeIndicator = new VisualElement();
            timeIndicator.AddToClassList("indicator_ver");
            timeIndicator.style.backgroundColor = new Color(0.75f, 0.75f, 0.75f, 0.5f);
            timeIndicator.style.left = -50;
            wrapper.Add(timeIndicator);
            timeIndicatorList.Add(timeIndicator);
        }

        observer            = _observer;
        audioJump           = observer.Register("audio_jump");
        visibleAreaChanged  = observer.Register("visibleArea_changed");
        observer.Subscribe("start", Start);
        observer.Subscribe("update", Update);
        observer.Subscribe("destroy", Destroy);
    }
    public void Start(dynamic[] _args){
        for(int i = 0; i < (int)wrapper.worldBound.width; i++){
            VisualElement sampleIndicator = new VisualElement();
            sampleIndicator.AddToClassList("indicator_ver");
            sampleIndicator.style.backgroundColor = new Color(0, .75f, 1, .5f);
            sampleIndicator.style.left = i;
            sampleIndicator.style.bottom = 0;
            sampleIndicator.style.height = Length.Percent(0);
            wrapper.Add(sampleIndicator);
            sampleIndicatorList.Add(sampleIndicator);
        }
        observer.Subscribe("audio_imported", AudioImported);
        observer.Subscribe("audio_playing", AudioPlaying);
        observer.Subscribe("visibleArea_changed", VisibleAreaChanged);
    }
    public void Update(dynamic[] _args){
        if(samples != null){
            UpdateTimeIndicators();
        }
    }
    public void Destroy(dynamic[] _args){

    }
    void UpdateTimeIndicators(){
        float packSize = 60f / bpm;
        float time = 0;
        int cnt = 0;
        while(time < VisibleArea.start){
            time += packSize;
        }
        while(time < VisibleArea.start + VisibleArea.size){
            timeIndicatorList[cnt].style.left = wrapper.worldBound.width * ((time - VisibleArea.start) / VisibleArea.size);
            time += packSize;
            cnt++;
        }
        while(cnt < timeIndicatorList.Count){
            timeIndicatorList[cnt].style.left = -50;
            cnt++;
        }
    }
    void VisibleAreaChanged(dynamic[] _args){
        float start = VisibleArea.start * frequency;
        float finish = (VisibleArea.start + VisibleArea.size) * frequency;
        float packetSize = (finish - start) / wrapper.worldBound.width;
        int n = 0;
        for(float i = start; i < finish; i += packetSize){
            if(n > wrapper.worldBound.width)
                break;
            float sum = 0;
            for(int j = (int)i; j < (int)(i + packetSize); j++){
                sum += Mathf.Abs(channel1[Mathf.Clamp(j, 0, channel1.Length - 1)]);
            }
            float avg = sum / packetSize;
            sampleIndicatorList[n++].style.height = Length.Percent(avg * 100);
        }
    }
    #region VisualElement interaction event callbacks
    void WrapperMouseDown(MouseDownEvent e){
        if(samples != null){
            switch(e.button){
                case (int)MouseButton.LeftMouse:
                    MouseState.leftDownTime = VisibleArea.start + (VisibleArea.size * (e.localMousePosition.x / wrapper.worldBound.width));
                break;
                case (int)MouseButton.RightMouse:
                    MouseState.rightDownTime = VisibleArea.start + (VisibleArea.size * (e.localMousePosition.x / wrapper.worldBound.width));
                break;
            }
        }
    }
    void WrapperMouseMove(MouseMoveEvent e){
        cursorPosIndicator.style.left = e.localMousePosition.x;
        if(samples != null){
            switch(e.button){
                case (int)MouseButton.LeftMouse:
                    if(MouseState.leftDownTime != -1){
                        float curTime   = VisibleArea.start + VisibleArea.size * (e.localMousePosition.x / wrapper.worldBound.width);
                        float diff      = curTime - MouseState.leftDownTime;
                        if(Mathf.Abs(diff) > 0.05f){
                            VisibleArea.start = Mathf.Clamp(VisibleArea.start + (-diff), 0, length - VisibleArea.size);
                            visibleAreaChanged.Invoke(new VisibleAreaChangedEventArgs(VisibleArea.start, VisibleArea.size));
                        }
                        MouseState.leftIsDrag = true;
                    }
                break;
            }
        }
    }
    void WrapperMouseUp(MouseUpEvent e){
        if(samples != null){
            switch(e.button){
                case (int)MouseButton.LeftMouse:
                    float curTime   = VisibleArea.start + VisibleArea.size * (e.localMousePosition.x / wrapper.worldBound.width);
                    if(!MouseState.leftIsDrag){
                        audioJump.Invoke(new AudioJumpEventArgs(curTime));
                    }
                    MouseState.leftDownTime = -1;
                    MouseState.leftIsDrag   = false;
                break;
                case (int)MouseButton.RightMouse:
                    MouseState.rightDownTime    = -1;
                    MouseState.rightIsDrag      = false;
                break;
            }
        }
    }
    void WrapperMouseLeave(MouseLeaveEvent e){
        if(samples != null){
            MouseState.leftDownTime = -1;
            MouseState.leftIsDrag   = false;
            MouseState.rightDownTime    = -1;
            MouseState.rightIsDrag      = false;
        }
    }
    #endregion
    #region External event callbacks
    void AudioImported(dynamic[] _args){
        AudioImportedEventArgs args = _args[0];
        
        AudioClip audioClip = args.audioClip;
        samples = new float[audioClip.samples * audioClip.channels];
        channel1 = new float[audioClip.samples];
        frequency = audioClip.frequency;
        length = audioClip.length;
        channels = audioClip.channels;
        
        audioClip.GetData(samples, 0);
        for(int i = 0, j = 0; i < samples.Length; i+=2, j++){
            channel1[j] = samples[i];
        }
    }
    void AudioPlaying(dynamic[] _args){
        AudioPlayingEventArgs args = _args[0];
        if(args.time >= VisibleArea.start && args.time < VisibleArea.start + VisibleArea.size){
            playPosIndicator.style.left = wrapper.worldBound.width * ((args.time - VisibleArea.start) / VisibleArea.size);
        }
        else{
            playPosIndicator.style.left = -50;
        }
    }
    #endregion
}
public class AudioJumpEventArgs{
    public float jumpTo{private set; get;}
    public AudioJumpEventArgs(float _jumpTo){
        jumpTo = _jumpTo;
    }
}
public class VisibleAreaChangedEventArgs{
    public float start{private set; get;}
    public float size{private set; get;}
    public VisibleAreaChangedEventArgs(float _start, float _size){
        start = _start;
        size = _size;
    }
}