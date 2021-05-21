using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class AudioPlayer : Block
{
    GameObject audioSourceObj;
    AudioSource audioSource;
    VisualElement audioDisplay;
    VisualElement cursorPosIndicator;
    VisualElement playPosIndicator;

    public AudioPlayer() 
        : base("Assets/Editor/Composer/Blocks/AudioPlayer/AudioPlayer.uxml")
    {
        rootVisualElement.name = "audio_player";
        audioSourceObj = new GameObject("AudioPlayer:audioSourceObj");
        audioSourceObj.hideFlags = HideFlags.HideAndDontSave;
        audioSource = audioSourceObj.AddComponent<AudioSource>();
        audioDisplay = rootVisualElement.Query<VisualElement>("audio_display");
        cursorPosIndicator = rootVisualElement.Query<VisualElement>("cursor_pos_indicator");
        playPosIndicator = rootVisualElement.Query<VisualElement>("play_pos_indicator");
        #region Functionality
        #region Buttons
        Button audioPlayBtn, audioPauseBtn, audioStopBtn, openExplorerBtn;
        audioPlayBtn    = rootVisualElement.Query<Button>("audio_play_btn");
        audioPauseBtn   = rootVisualElement.Query<Button>("audio_pause_btn");
        audioStopBtn    = rootVisualElement.Query<Button>("audio_stop_btn");
        openExplorerBtn = rootVisualElement.Query<Button>("open_explorer_btn");
        audioPlayBtn.clicked    += ()=>{
            if(audioSource.clip == null) return;
            audioSource.Play();
        };
        audioPauseBtn.clicked   += ()=>{
            if(audioSource.clip == null) return;
            audioSource.Pause();
        };
        audioStopBtn.clicked    += ()=>{
            if(audioSource.clip == null) return;
            audioSource.Stop();
            audioSource.time = 0;
        };
        openExplorerBtn.clicked += ()=>{
            if(audioSource.isPlaying) audioSource.Stop();
            string path = EditorUtility.OpenFilePanel("Audio Player", Application.dataPath, "mp3, mp4");
            if(path != null)
            {
                if(audioSource.clip != null)
                {
                    audioSource.time = 0;
                    Texture2D.DestroyImmediate(audioDisplay.style.backgroundImage.value.texture);
                }
                audioSource.clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(path));
                audioDisplay.style.backgroundImage = PaintWaveformSpectrum(audioSource.clip, 0.5f, 2000, 500, new Color(0,1,1));
            }
        };
        #endregion        
        #region Audio Display
        audioDisplay.RegisterCallback<MouseMoveEvent>((e)=>{
            cursorPosIndicator.style.left = e.localMousePosition.x;
        });
        audioDisplay.RegisterCallback<MouseDownEvent>((e)=>{
            if(audioSource.clip == null) return;
            audioSource.time = audioSource.clip.length * (e.localMousePosition.x / audioDisplay.worldBound.size.x);
        });
        #endregion
        #endregion
    }
    public override void Update()
    {
        if(audioSource.clip == null) return;
        if(audioSource.clip.length - audioSource.time < 0.005f)
        {
            audioSource.Stop();
            audioSource.time = 0;
            playPosIndicator.style.left = 0;
        }
        else
        {
            playPosIndicator.style.left = audioDisplay.worldBound.size.x * (audioSource.time / audioSource.clip.length);
        }
    }
    public Texture2D PaintWaveformSpectrum(AudioClip audio, float saturation, int width, int height, Color col) 
    {
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
    public override void Destroy()
    {
        GameObject.DestroyImmediate(audioSourceObj);
        Texture2D.DestroyImmediate(audioDisplay.style.backgroundImage.value.texture);
    }
}