#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using kgh.Signals;
using kgh.UI;

public class AudioPlayer : kgh.UI.Component
{
    Button playBtn, pauseBtn, stopBtn;
    Slider volumeSlider;
    TextField audioPathDisplay;
    Button openExplorerBtn;
    VisualElement audioDisplay;

    Exchange exchange;
    Switch visibleAreaChanged, loadFromSavedFile, audioLoaded;
    GameObject audioSourceObj;
    AudioSource audioSource;
    VisualElement sampleIndicatorCollection, playPosIndicator, cursorPosIndicator;
    VisualElement[] sampleIndicators;
    static class VisibleArea
    {
        public static float start = 0;
        public static float temp = 0;
        public static float size = 15;
    }
    float[] channel1;

    public AudioPlayer(Exchange _exchange) : base("Assets/_Editor/Composer/Components/AudioPlayer/AudioPlayer.uxml"){
        rootVisualElement.name = "audio_player";
        playBtn = rootVisualElement.Query<Button>("play_btn");
        pauseBtn = rootVisualElement.Query<Button>("pause_btn");
        stopBtn = rootVisualElement.Query<Button>("stop_btn");
        volumeSlider = rootVisualElement.Query<Slider>("volume_slider");
        audioPathDisplay = rootVisualElement.Query<TextField>("audio_path_display");
        openExplorerBtn = rootVisualElement.Query<Button>("open_explorer_btn");
        audioDisplay = rootVisualElement.Query<VisualElement>("audio_display");
        playPosIndicator = rootVisualElement.Query<VisualElement>("playPos_indicator");
        cursorPosIndicator = rootVisualElement.Query<VisualElement>("cursorPos_indicator");
        playPosIndicator.BringToFront();

        exchange = _exchange;
        visibleAreaChanged = exchange.Register("visible_area_changed", typeof(Action<float, float>));
        loadFromSavedFile = exchange.Register("load_from_saved_file", typeof(Func<string, string>));
        audioLoaded = exchange.Register("audio_loaded", typeof(Action<string>));
        exchange.Subscribe("start", this, "Start");
        exchange.Subscribe("update", this, "Update");
        exchange.Subscribe("destroy", this, "Destroy");
    }
    void Start(){
        audioSourceObj = new GameObject("AudioPlayer: audioSourceObj");
        audioSource = audioSourceObj.AddComponent<AudioSource>();
        sampleIndicatorCollection = new VisualElement();
        sampleIndicatorCollection.name = "sample_indicators";
        sampleIndicatorCollection.style.height = Length.Percent(100);
        audioDisplay.Add(sampleIndicatorCollection);
        sampleIndicators = new VisualElement[(int)audioDisplay.worldBound.width];
        for(int i = 0; i < (int)audioDisplay.worldBound.width; i++){
            sampleIndicators[i] = new VisualElement();
            sampleIndicators[i].AddToClassList("ver-indicator");
            sampleIndicators[i].style.backgroundColor = new Color(0, 0.5f, 1, 0.5f);
            sampleIndicators[i].style.left = i;
            sampleIndicators[i].style.bottom = 0;
            sampleIndicators[i].style.height = Length.Percent(0);
            sampleIndicatorCollection.Add(sampleIndicators[i]);
        }
        
        playBtn.clicked += ()=>{
            if(audioSource.clip == null){
                return;
            }
            audioSource.Play();
        };
        pauseBtn.clicked += ()=>{
            if(audioSource.clip == null){
                return;
            }
            audioSource.Pause();
        };
        stopBtn.clicked += ()=>{
            if(audioSource.clip == null){
                return;
            }
            audioSource.Stop();
            audioSource.time = 0;
        };
        volumeSlider.RegisterValueChangedCallback<float>((e)=>{
            audioSource.volume = e.newValue;
        });
        openExplorerBtn.clicked += ()=>{
            string path = EditorUtility.OpenFilePanel("AudioPlayer", "Assets/Resources/Audio", "mp3,txt");
            if(path == ""){
                return;
            }
            if(Path.GetExtension(path) == ".txt"){
                dynamic[] temp = loadFromSavedFile.Invoke(path);
                path = temp[0];
            }
            audioSource.clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(path));
            audioPathDisplay.value = path;
            if(audioSource.clip != null){
                float[] sampleValues = new float[audioSource.clip.samples * audioSource.clip.channels];
                channel1 = new float[audioSource.clip.samples];
                audioSource.clip.GetData(sampleValues, 0);
                for(int i = 0; i < channel1.Length; i++){
                    channel1[i] = sampleValues[2 * i];
                }
                VisibleArea.start = 0;
                VisibleArea.size = 15;
                VisibleAreaChanged();   
                audioLoaded.Invoke(Path.GetFileNameWithoutExtension(path));
            }
        };
        audioDisplay.RegisterCallback<MouseMoveEvent>((e)=>{
            cursorPosIndicator.style.left = e.localMousePosition.x;
        });
        audioDisplay.RegisterCallback<MouseLeaveEvent>((e)=>{
            cursorPosIndicator.style.left = -500;
        });
        audioDisplay.RegisterCallback<WheelEvent>((e)=>{
            float sign = Mathf.Sign(e.delta.y);
            float multiplier = (sign > 0) ? 2 : 0.5f;
            VisibleArea.size = Mathf.Clamp(VisibleArea.size * multiplier, 7.5f, 30);
            VisibleAreaChanged();
        });
        Interactions.MouseInteractions(
            audioDisplay,
            (button, mouseDownPos)=>{
                if(audioSource.clip == null){
                    return;
                }
                switch(button){
                    case (int)MouseButton.LeftMouse:
                        float jumpTo = VisibleArea.start + VisibleArea.size * (mouseDownPos.x / audioDisplay.worldBound.width);
                        audioSource.time = jumpTo;
                    break;
                } 
            },
            (button, mouseDownPos)=>{
                if(audioSource.clip == null){
                    return;
                }
                switch(button){
                    case (int)MouseButton.LeftMouse:
                        VisibleArea.temp = VisibleArea.start;
                    break;
                }
            },
            (button, mouseDownPos, diff)=>{
                if(audioSource.clip == null){
                    return;
                }
                switch(button){
                    case (int)MouseButton.LeftMouse:
                        float a = VisibleArea.size * (diff.x / audioDisplay.worldBound.width);
                        VisibleArea.start = Mathf.Clamp(VisibleArea.temp - a, 0, audioSource.clip.length - VisibleArea.size);
                        VisibleAreaChanged();
                    break;
                }
            },
            null
        );
        
    }
    void Update(){
        if(audioSource.clip != null && audioSource.time >= VisibleArea.start && audioSource.time < VisibleArea.start + VisibleArea.size){
            playPosIndicator.style.left = audioDisplay.worldBound.width * ((audioSource.time - VisibleArea.start) / VisibleArea.size);
        }
        else{
            playPosIndicator.style.left = -500;
        }
    }
    void Destroy(){
        GameObject.DestroyImmediate(audioSourceObj);
    }
    void VisibleAreaChanged(){
        int startInd = Mathf.RoundToInt(audioSource.clip.samples * VisibleArea.start / audioSource.clip.length);
        int endInd = Mathf.RoundToInt(audioSource.clip.samples * (VisibleArea.start + VisibleArea.size) / audioSource.clip.length);
        int samplesPerPix = Mathf.RoundToInt((float)(endInd - startInd) / audioDisplay.worldBound.width);

        for(int i = 0; i < sampleIndicators.Length; i++){
            int pixStartInd = startInd + samplesPerPix * i;
            int pixEndInd = Mathf.Clamp(startInd + samplesPerPix * (i + 1), pixStartInd, audioSource.clip.samples);
            float sum = 0;
            for(int j = pixStartInd; j < pixEndInd; j+=4){
                sum += Mathf.Abs(channel1[j]);
            }
            sampleIndicators[i].style.height = Length.Percent(100 * sum * 4/ (pixEndInd - pixStartInd));
        }
        visibleAreaChanged.Invoke(VisibleArea.start, VisibleArea.size);
    }
}
#endif