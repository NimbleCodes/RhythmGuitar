using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using System;

namespace EditorWndMods {
    public class AudioPlayer : Module {
        GameObject audioSourceObj;
        AudioSource audioSource;
        VisualElement _rootVisualElement;
        VisualElement _audioDisplay;
        VisualElement _playPosIndicator;
        Texture2D noAudioWaveformTex;
        Texture2D cursorPosIndicatorTex;
        Texture2D playPosIndicatorTex;
        Color cursorPosIndicatorColor = new Color(1,0.65f,0,1);
        Color playPosIndicatorColor = Color.red;
        public VisualElement rootVisualElement{
            get{
                return _rootVisualElement;
            }
        }
        public AudioPlayer(){
            //Create audio source object
            audioSourceObj = new GameObject("AudioSourceObject");
            audioSource = audioSourceObj.AddComponent<AudioSource>();
            audioSourceObj.hideFlags = HideFlags.HideAndDontSave;
            //Initial waveform texture
            noAudioWaveformTex = new Texture2D(1,1);
            noAudioWaveformTex.SetPixel(0,0,Color.black);
            noAudioWaveformTex.Apply();
            //Position indicators init
            cursorPosIndicatorTex = new Texture2D(1,1);
            playPosIndicatorTex = new Texture2D(1,1);
            cursorPosIndicatorTex.SetPixel(0,0,cursorPosIndicatorColor);
            playPosIndicatorTex.SetPixel(0,0,playPosIndicatorColor);
            cursorPosIndicatorTex.Apply();
            playPosIndicatorTex.Apply();
            //Import uxml
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Composer/Modules/AudioPlayer/AudioPlayer.uxml");
            _rootVisualElement = visualTreeAsset.Instantiate();
            _rootVisualElement.name = "audio_player";
            //Add functionality here
            //Audio waveform img
            VisualElement audioDisplay = _rootVisualElement.Query<VisualElement>("audio_display");
            _audioDisplay = audioDisplay;
            audioDisplay.style.backgroundImage = noAudioWaveformTex;
            VisualElement cursorPosIndicator = _rootVisualElement.Query<VisualElement>("cursor_pos_indicator");
            cursorPosIndicator.style.backgroundImage = cursorPosIndicatorTex;
            VisualElement playPosIndicator = _rootVisualElement.Query<VisualElement>("play_pos_indicator");
            _playPosIndicator = playPosIndicator;
            playPosIndicator.style.backgroundImage = playPosIndicatorTex;
            audioDisplay.RegisterCallback<MouseMoveEvent>((e)=>{
                cursorPosIndicator.style.left = e.localMousePosition.x;
            });
            audioDisplay.RegisterCallback<MouseDownEvent>((e)=>{
                float gotoPos = e.localMousePosition.x / audioDisplay.worldBound.size.x;
                audioSource.time = audioSource.clip.length * gotoPos;
            });
            //Buttons
            Button playBtn, pauseBtn, stopBtn, openExpBtn;
            playBtn = _rootVisualElement.Query<Button>("play_btn");
            pauseBtn = _rootVisualElement.Query<Button>("pause_btn");
            stopBtn = _rootVisualElement.Query<Button>("stop_btn");
            openExpBtn = _rootVisualElement.Query<Button>("open_exp_btn");
            playBtn.clicked += ()=>{
                //Debug.Log("Play!");
                audioSource.Play();
            };
            pauseBtn.clicked += ()=>{
                //Debug.Log("Pause!");
                audioSource.Pause();
            };
            stopBtn.clicked += ()=>{
                //Debug.Log("Stop!");
                audioSource.time = 0;
                audioSource.Stop();
                playPosIndicator.style.left = 0;
            };
            openExpBtn.clicked += ()=>{
                //Debug.Log("Open Explorer!");
                string path = EditorUtility.OpenFilePanel("Composer",Application.dataPath,"mp3");
                if(path.Length != 0){
                    if(audioSource.clip != null){
                        //changed audio file
                        audioSource.time = 0;
                    }
                    audioSource.clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(path));
                    audioDisplay.style.backgroundImage = PaintWaveformSpectrum(audioSource.clip, .5f, 2000, 500, new Color(0,1,1,1));
                }
            };
        }
        override public void Update(){
            //Debug.Log("Update: AudioPlayer");
            if(audioSource.isPlaying){
                float curPlayPos = audioSource.time / audioSource.clip.length;
                _playPosIndicator.style.left = _audioDisplay.worldBound.size.x * curPlayPos;
            }
            if(audioSource.clip != null){
                if(Math.Abs(audioSource.time - audioSource.clip.length) < 0.005f){
                    audioSource.time = 0;
                    audioSource.Stop();
                }
            }
        }
        override public void Destroy(){
            GameObject.DestroyImmediate(audioSourceObj);
        }
        public Texture2D PaintWaveformSpectrum(AudioClip audio, float saturation, int width, int height, Color col) {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float[] samples = new float[audio.samples];
            float[] waveform = new float[width];
            audio.GetData(samples, 0);
            int packSize = ( audio.samples / width ) + 1;
            int s = 0;
            for (int i = 0; i < audio.samples; i += packSize) {
                waveform[s] = Mathf.Abs(samples[i]);
                s++;
            }
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    tex.SetPixel(x, y, Color.black);
                }
            }
            for (int x = 0; x < waveform.Length; x++) {
                for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++) {
                    tex.SetPixel(x, ( height / 2 ) + y, col);
                    tex.SetPixel(x, ( height / 2 ) - y, col);
                }
            }
            tex.Apply();
            return tex;
        }
    }
}