using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
public class MCAD : myUI.Component{
    List<List<VisualElement>> verticalIndicators;
    List<VisualElement> verticalIndicatorCollections;
    List<VisualElement> verticalIndicatorsBPM;
    VisualElement verticalIndicatorCollectionBPM;
    List<List<float>> splitData;
    bool mouseDownStart = false;
    float t0;
    DropDown dd;
    GameObject audioSourceObj;
    AudioSource audioSource;
    VisualElement playPosIndicator;
    VisualElement clipEndIndicator;

    public class MCADStates : States{
        MCAD component;
        AudioClip _audioClip;
        public AudioClip audioClip{
            set{
                dirty = true;
                _audioClip = value;
                component.OnAudioClipSet();
            }
            get{ return _audioClip; }
        }
        float _start, _size;
        public float start{
            set{
                if(audioClip != null){
                    if(audioClip.length < size){
                        _start = 0;
                    }
                    else{
                        _start = Mathf.Clamp(value, 0, audioClip.length - size);
                    }
                    dirty = true;
                }
            }
            get{ return _start; }
        }
        public float size{
            set{
                if(audioClip != null){
                    _size = Mathf.Clamp(value, 10, 60);
                    start = Mathf.Clamp(start, 0, audioClip.length - _size);
                    dirty = true;
                }
            }
            get{ return _size; }
        }
        int _bpm;
        public int bpm{
            set{
                _bpm = value;
                dirty = true;
            }
            get{ return _bpm; }
        }
        public MCADStates(MCAD _component) : base(_component){
            component = _component;
            _start = 0;
            _size = 30;
            _bpm = 120;
        }
    }
    public MCAD(){
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Multi-Channel Audio Display/MCAD.uxml");
        rootVisualElement = visualTreeAsset.CloneTree();
        rootVisualElement.style.width = Length.Percent(100);
        rootVisualElement.style.height = Length.Percent(100);
        rootVisualElement.name = "multi-channel-audio-display";
        rootVisualElement.RegisterCallback<MouseDownEvent>((e)=>{
            if(((MCADStates)states).audioClip == null){
                return;
            }
            if(e.button == (int)MouseButton.LeftMouse){
                mouseDownStart = true;
                t0 = ((MCADStates)states).start + ((MCADStates)states).size * (e.mousePosition.x / rootVisualElement.localBound.width);
            }   
            else if(e.button == (int)MouseButton.RightMouse){
                dd.root.style.left = e.localMousePosition.x;
                dd.root.style.top = e.localMousePosition.y;
                dd.root.BringToFront();
                dd.root.Focus();
            }
        });
        rootVisualElement.RegisterCallback<MouseMoveEvent>((e)=>{
            if(((MCADStates)states).audioClip == null){
                return;
            }
            if(mouseDownStart){
                float t1 = ((MCADStates)states).start + ((MCADStates)states).size * (e.mousePosition.x / rootVisualElement.localBound.width);
                ((MCADStates)states).start -= t1 - t0;
            }
        });
        rootVisualElement.RegisterCallback<MouseUpEvent>((e)=>{
            if(((MCADStates)states).audioClip == null){
                return;
            }
            if(e.button == (int)MouseButton.LeftMouse){
                mouseDownStart = false;
            }
        });
        rootVisualElement.RegisterCallback<MouseLeaveEvent>((e)=>{
            mouseDownStart = false;
        });

        (string, Action<Vector3>)[] elements = {
            ("Play", (e)=>{ 
                audioSource.Play();
                states.constUpdate = true;
                dd.root.style.left = -500;
            }),
            ("Pause", (e)=>{ 
                audioSource.Pause();
                states.constUpdate = false;
                dd.root.style.left = -500;
            }),
            ("Stop", (e)=>{ 
                audioSource.Stop();
                states.constUpdate = false;
                dd.root.style.left = -500;
            })
        };
        dd = new DropDown(elements);
        dd.root.style.left = -500;
        rootVisualElement.Add(dd.root);

        states = new MCADStates(this);
        verticalIndicators = new List<List<VisualElement>>();
        verticalIndicatorCollections = new List<VisualElement>();
        splitData = new List<List<float>>();

        verticalIndicatorsBPM = new List<VisualElement>();
        verticalIndicatorCollectionBPM = new VisualElement();
        verticalIndicatorCollectionBPM.name = "vertical-indicator-collection";
        verticalIndicatorCollectionBPM.style.flexDirection = FlexDirection.Row;
        verticalIndicatorCollectionBPM.style.alignItems = Align.Center;
        verticalIndicatorCollectionBPM.style.width = Length.Percent(100);
        verticalIndicatorCollectionBPM.style.height = Length.Percent(100);
        verticalIndicatorCollectionBPM.style.position = Position.Absolute;
        rootVisualElement.Add(verticalIndicatorCollectionBPM);

        audioSourceObj = new GameObject("AudioPlayer: audioSourceObj");
        audioSource = audioSourceObj.AddComponent<AudioSource>();

        playPosIndicator = new VisualElement();
        playPosIndicator.name = "play-pos-indicator";
        playPosIndicator.AddToClassList("vertical-indicator");
        playPosIndicator.style.backgroundColor = Color.HSVToRGB(0, 0.75f, 0.75f);
        playPosIndicator.style.left = 0;
        playPosIndicator.style.height = Length.Percent(100);
        playPosIndicator.style.position = Position.Absolute;
        rootVisualElement.Add(playPosIndicator);
        playPosIndicator.BringToFront();

        clipEndIndicator = new VisualElement();
        clipEndIndicator.AddToClassList("vertical-indicator");
        clipEndIndicator.style.backgroundColor = Color.magenta;
        clipEndIndicator.style.height = Length.Percent(100);
        clipEndIndicator.style.position = Position.Absolute;
        rootVisualElement.Add(clipEndIndicator);
    }
    protected override void _Update()
    {
        // Debug.Log("MCAD::Update");

        MCADStates _states = (MCADStates)states;
        List<List<float>> heights = new List<List<float>>();
        List<List<object>> locks = new List<List<object>>();
        for(int i = 0; i < verticalIndicators.Count; i++){
            heights.Add(new List<float>());
            locks.Add(new List<object>());
            for(int j = 0; j < verticalIndicators[i].Count; j++){
                heights[i].Add(-1);
                locks[i].Add(new object());
            }
        }
        List<Task> tasks = new List<Task>();
        Action<object> action = (_param)=>{
            float audioLength = (float)_param;
            for(int i = 0; i < verticalIndicators.Count; i++){
                for(int j = 0; j < verticalIndicators[i].Count; j++){
                    if(heights[i][j] != -1)
                        continue;
                    float samplesPerPacket = (splitData[i].Count / audioLength) * (_states.size / verticalIndicators[i].Count);
                    bool lockTaken = false;
                    try{
                        Monitor.TryEnter(locks[i][j], 0, ref lockTaken);
                        if(lockTaken){
                            float packetStart = (splitData[i].Count / audioLength) * _states.start + (j * samplesPerPacket);
                            float packetEnd = Mathf.Min(packetStart + samplesPerPacket, splitData[i].Count);
                            float sum = 0;
                            float cnt = packetStart;
                            while(cnt < packetEnd){
                                sum += Mathf.Abs(splitData[i][(int)cnt]);
                                cnt++;
                            }
                            heights[i][j] = 100 * (sum / (packetEnd - packetStart));
                        }
                    }
                    finally{
                        if(lockTaken)
                            Monitor.Exit(locks[i][j]);
                    }
                }
            }
        };
        for(int i = 0; i < 16; i++){
            var param = _states.audioClip.length;
            Task task = new Task(action, param);
            tasks.Add(task);
            task.Start();
        }
        Task.WaitAll(tasks.ToArray());
        tasks.ForEach((t)=>{
            t.Dispose();
        });
        for(int i = 0; i < verticalIndicators.Count; i++){
            for(int j = 0; j < verticalIndicators[i].Count; j++){
                verticalIndicators[i][j].style.height = Length.Percent(heights[i][j]);
            }
        }
        
        float packSize = 60.0f / _states.bpm;
        float time = Mathf.CeilToInt(_states.start / packSize) * packSize;
        int ind = 0;
        while(time < _states.start + _states.size){
            VisualElement curVertInd;
            if(ind < verticalIndicatorsBPM.Count){
                curVertInd = verticalIndicatorsBPM[ind];
            }
            else{
                curVertInd = new VisualElement();
                curVertInd.AddToClassList("vertical-indicator");
                curVertInd.style.height = Length.Percent(100);
                curVertInd.style.backgroundColor = Color.grey;
                curVertInd.style.position = Position.Absolute;
                curVertInd.BringToFront();
                verticalIndicatorsBPM.Add(curVertInd);
                verticalIndicatorCollectionBPM.Add(curVertInd);
            }
            curVertInd.style.left = Length.Percent(100 * (time - _states.start) / _states.size);
            time += packSize;
            ind++;
        }
        while(ind < verticalIndicatorsBPM.Count){
            verticalIndicatorsBPM[ind].style.left = -100;
            ind++;
        }
    
        if(audioSource.clip != null && audioSource.time >= _states.start && audioSource.time < _states.start + _states.size){
            playPosIndicator.style.left = rootVisualElement.worldBound.width * ((audioSource.time - _states.start) / _states.size);
        }
        else{
            playPosIndicator.style.left = -500;
        }
    
        if(_states.audioClip.length < _states.size){
            clipEndIndicator.style.left = Length.Percent(100 * (_states.audioClip.length /_states.size));
        }
        else{
            clipEndIndicator.style.left = -500;
        }
    }
    protected override void Synchronize()
    {
        //do nothing
    }
    void OnAudioClipSet(){
        MCADStates _states = (MCADStates)states;
        audioSource.clip = _states.audioClip;
        for(int i = verticalIndicators.Count; i < _states.audioClip.channels; i++){
            verticalIndicators.Add(new List<VisualElement>());
            VisualElement verticalIndicatorCollection = new VisualElement();
            verticalIndicatorCollection.name = "vertical-indicator-collection";
            verticalIndicatorCollection.style.flexDirection = FlexDirection.Row;
            verticalIndicatorCollection.style.alignItems = Align.Center;
            for(int j = 0; j < rootVisualElement.localBound.width; j++){
                VisualElement verticalIndicator = new VisualElement();
                verticalIndicator.AddToClassList("vertical-indicator");
                verticalIndicator.style.backgroundColor = new Color(1, 0.49f, 0.31f);
                verticalIndicatorCollection.Add(verticalIndicator);
                verticalIndicators[i].Add(verticalIndicator);
            }
            verticalIndicatorCollections.Add(verticalIndicatorCollection);
            rootVisualElement.Add(verticalIndicatorCollection);
        }
        for(int i = 0; i < verticalIndicatorCollections.Count; i++){
            verticalIndicatorCollections[i].style.width = Length.Percent(100);
            verticalIndicatorCollections[i].style.height = Length.Percent(100f / verticalIndicatorCollections.Count);
        }

        float[] samples = new float[_states.audioClip.channels * _states.audioClip.samples];
        _states.audioClip.GetData(samples, 0);
        splitData.Clear();
        for(int i = 0; i < _states.audioClip.channels; i++){
            splitData.Add(new List<float>());
        }
        // //2800 ms
        // for(int i = 0; i < _states.audioClip.samples; i++){
        //     for(int j = 0; j < _states.audioClip.channels; j++){
        //         splitData[j].Add(samples[i * _states.audioClip.channels + j]);
        //     }
        // }
        // 259 ms
        List<Task> splitTasks = new List<Task>();
        for(int i = 0; i < _states.audioClip.channels; i++){
            var param = (_states.audioClip.channels, _states.audioClip.samples, i);
            Task splitTask = new Task((_param)=>{
                dynamic param = _param;
                int numChannels = param.Item1;
                int numSamples = param.Item2;
                int channelNum = param.Item3;
                for(int i = 0; i < numSamples; i++){
                    splitData[channelNum].Add(samples[i * numChannels + channelNum]);
                }
            }, param);
            splitTasks.Add(splitTask);
            splitTask.Start();
        }
        Task.WaitAll(splitTasks.ToArray());
        splitTasks.ForEach((t)=>{
            t.Dispose();
        });
    }
    protected override void _Dispose()
    {
        GameObject.DestroyImmediate(audioSourceObj);
    }
}