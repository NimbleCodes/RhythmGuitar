using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class AudioPlayer : Component
{
    AudioSource audioSource;
    Observer observer;
    VisualElement cursorPositionIndicator, playPositionIndicator;
    VisualElement audioDisplay;
    DataIO dataIO;

    public AudioPlayer(AudioSource _audioSource, Observer _observer, NoteData noteData) : base("Assets/Editor/Components/AudioPlayer/AudioPlayer.uxml"){
        audioSource = _audioSource;
        observer    = _observer;
        dataIO      = new DataIO(noteData);
        cursorPositionIndicator = rootVisualElement.Query<VisualElement>("cursorPosition_indicator");
        playPositionIndicator   = rootVisualElement.Query<VisualElement>("playPosition_indicator");
        cursorPositionIndicator.style.left  = -1;
        playPositionIndicator.style.left    = -1;
        
        audioDisplay            = rootVisualElement.Query<VisualElement>("audio_display");
        audioDisplay.RegisterCallback<MouseMoveEvent>((e)=>{
            cursorPositionIndicator.style.left = e.localMousePosition.x;
        });
        audioDisplay.RegisterCallback<MouseDownEvent>((e)=>{
            if(audioSource.clip != null){
                float jmpPos = audioSource.clip.length * (e.localMousePosition.x / audioDisplay.worldBound.width);
                audioSource.time = jmpPos;
            }
        });

        Button openExplorerBtn, playBtn, pauseBtn, stopBtn;
        openExplorerBtn = rootVisualElement.Query<Button>("openExplorer_btn");
        playBtn         = rootVisualElement.Query<Button>("play_btn");
        pauseBtn        = rootVisualElement.Query<Button>("pause_btn");
        stopBtn         = rootVisualElement.Query<Button>("stop_btn");
        openExplorerBtn.clicked += ()=>{
            string path = EditorUtility.OpenFilePanel("", Application.dataPath, "mp3,txt");
            if(path != null){
                switch(Path.GetExtension(path)){
                    case ".mp3":
                        audioSource.clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(path));
                        audioDisplay.style.backgroundImage = PaintWaveformSpectrum(audioSource.clip, 0.5f, (int)audioDisplay.worldBound.width, (int)audioDisplay.worldBound.height, new Color(0,1,1,1));
                        noteData.fileName = Path.GetFileNameWithoutExtension(path);
                        break;
                    case ".txt":
                        dataIO.Load(path);
                        audioSource.clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes("Assets/Resources/Audio/" + noteData.fileName + ".mp3"));
                        audioDisplay.style.backgroundImage = PaintWaveformSpectrum(audioSource.clip, 0.5f, (int)audioDisplay.worldBound.width, (int)audioDisplay.worldBound.height, new Color(0,1,1,1));
                        break;
                }
            }
        };
        playBtn.clicked += ()=>{
            if(audioSource.clip != null){
                audioSource.Play();
            }
        };
        pauseBtn.clicked += ()=>{
            if(audioSource.clip != null){
                audioSource.Pause();
            }
        };
        stopBtn.clicked += ()=>{
            if(audioSource.clip != null){
                audioSource.Stop();
                audioSource.time = 0;
            }
        };

        Slider volumeSlider;
        volumeSlider = rootVisualElement.Query<Slider>("volume_slider");
        volumeSlider.RegisterValueChangedCallback((e)=>{
            audioSource.volume = e.newValue;
        });

        observer.Subscribe("update",    Update);
        //observer.Subscribe("destroy",   Destroy);
    }
    public Texture2D PaintWaveformSpectrum(AudioClip audio, float saturation, int width, int height, Color col) 
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        float[] samples = new float[audio.samples];
        float[] waveform = new float[width];
        audio.GetData(samples, 0);
        int packSize = ( audio.samples / width ) + 1;
        int s = 0;
        for (int i = 0; i < audio.samples; i += packSize) 
        {
            waveform[s] = Mathf.Abs(samples[i]);
            s++;
        }
        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++) 
            {
                tex.SetPixel(x, y, new Color(0.15f, 0.15f, 0.15f, 1));
            }
        }
        for (int x = 0; x < waveform.Length; x++) 
        {
            for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++)
            {
                tex.SetPixel(x, ( height / 2 ) + y, col);
                tex.SetPixel(x, ( height / 2 ) - y, col);
            }
        }
        tex.Apply();
        return tex;
    }
    void Update(System.Object[] parameters){
        if(audioSource.clip != null){
            playPositionIndicator.style.left = audioDisplay.worldBound.width * (audioSource.time / audioSource.clip.length);
        }
    }
    void Destroy(System.Object[] parameters){

    }
}