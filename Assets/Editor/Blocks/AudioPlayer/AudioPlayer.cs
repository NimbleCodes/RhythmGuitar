using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;

public class AudioPlayer : Block
{
    GameObject audioSourceObject;
    public AudioSource audioSource;
    string currentAudioPath;
    string filename;
    VisualElement audioWaveform, playPositionIndicator;
    NoteData noteData = new NoteData();
    public AudioPlayer() : base("Assets/Editor/Blocks/AudioPlayer/AudioPlayer.uxml")
    {
        rootVisualElement.name = "audio_player";
        VisualElement cursorPositionIndicator;
        Button playBtn, pauseBtn, stopBtn, openExplorerBtn;
        audioSourceObject = new GameObject("AudioPlayer:audioSourceObject");
        audioSource = audioSourceObject.AddComponent<AudioSource>();

        if((cursorPositionIndicator = rootVisualElement.Query<VisualElement>("cursorPosition_indicator")) != null)
        {
            cursorPositionIndicator.style.backgroundColor = new Color(1,1,0,1);
            cursorPositionIndicator.style.left = -1;
        }
        if((playPositionIndicator = rootVisualElement.Query<VisualElement>("playPosition_indicator")) != null)
        {
            playPositionIndicator.style.backgroundColor = Color.red;
            playPositionIndicator.style.left = -1;
        }
        if((audioWaveform = rootVisualElement.Query<VisualElement>("audio_waveform")) != null)
        {
            audioWaveform.style.backgroundColor = new Color(0.15f,0.15f,0.15f,1);
            audioWaveform.RegisterCallback<MouseMoveEvent>((e)=>{
                cursorPositionIndicator.style.left = e.localMousePosition.x;
            });
            audioWaveform.RegisterCallback<MouseDownEvent>((e)=>{
                if(audioSource.clip != null)
                {
                    float jumpPosition = audioSource.clip.length * (e.localMousePosition.x / audioWaveform.worldBound.xMax);
                    audioSource.time = jumpPosition;
                }
            });
        }
        if((playBtn = rootVisualElement.Query<Button>("play_btn")) != null)
        {
            playBtn.clicked += ()=>{
                audioSource.Play();
            };
        }
        if((pauseBtn = rootVisualElement.Query<Button>("pause_btn")) != null)
        {
            pauseBtn.clicked += ()=>{
                audioSource.Pause();
            };
        }
        if((stopBtn = rootVisualElement.Query<Button>("stop_btn")) != null)
        {
            stopBtn.clicked += ()=>{
                audioSource.Stop();
                audioSource.time = 0;
            };
        }
        if((openExplorerBtn = rootVisualElement.Query<Button>("openExplorer_btn")) != null)
        {
            openExplorerBtn.clicked += ()=>{
                string path = EditorUtility.OpenFilePanel("Import audio file", Application.dataPath, "mp3, mp4");
                if(path != null){
                    if(path == currentAudioPath) return;
                    audioSource.clip = null;
                    audioWaveform.style.backgroundImage = null;
                    audioSource.time = 0;
                }
                audioSource.clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(path));
                audioWaveform.style.backgroundImage = PaintWaveformSpectrum(audioSource.clip, 0.5f, (int)audioWaveform.worldBound.width, (int)audioWaveform.worldBound.height, new Color(0,1,1,1));
                currentAudioPath = path;
                filename = Path.GetFileName(path);
                Debug.Log(filename);
                noteData.fileName = filename;
            };
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
    public override void Update()
    {
        base.Update();
        if(audioSource.clip != null)
            playPositionIndicator.style.left = audioWaveform.worldBound.width * (audioSource.time / audioSource.clip.length);
    }
    public override void Destroy()
    {
        base.Destroy();
        GameObject.DestroyImmediate(audioSourceObject);
    }

    public void MusicInit(){
        currentAudioPath = Application.dataPath + "/Resources/Audio/" + noteData.fileName;
        audioSource.clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(currentAudioPath));
        audioWaveform.style.backgroundImage = PaintWaveformSpectrum(audioSource.clip, 0.5f, (int)audioWaveform.worldBound.width, (int)audioWaveform.worldBound.height, new Color(0,1,1,1));
    }

}