using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class AudioPlayer : Component
{
    Observer observer;
    Switch audioImported, audioPlaying;
    Button playBtn, pauseBtn, stopBtn, openExplorerBtn;
    Slider volumeSlider;
    TextField pathDisplay;
    GameObject audioSourceObject;
    AudioSource audioSource;
    bool prevPlaying = false;

    public AudioPlayer(Observer _observer) : base("Assets/Editor/Composer/Components/AudioPlayer/AudioPlayer.uxml"){
        rootVisualElement.name = "audio_player";
        playBtn         = rootVisualElement.Query<Button>("play_btn");
        pauseBtn        = rootVisualElement.Query<Button>("pause_btn");
        stopBtn         = rootVisualElement.Query<Button>("stop_btn");
        openExplorerBtn = rootVisualElement.Query<Button>("openExplorer_btn");
        volumeSlider    = rootVisualElement.Query<Slider>("volume_slider");
        pathDisplay     = rootVisualElement.Query<TextField>("path_display");
        playBtn.clicked         += PlayBtnClicked;
        pauseBtn.clicked        += PauseBtnClicked;
        stopBtn.clicked         += StopBtnClicked;
        openExplorerBtn.clicked += OpenExplorerBtnClicked;
        volumeSlider.RegisterValueChangedCallback(VolumeSliderValueChanged);

        audioSourceObject   = new GameObject("AudioPlayer: audioSourceObject");
        audioSource         = audioSourceObject.AddComponent<AudioSource>();

        observer        = _observer;
        audioImported   = observer.Register("audio_imported");
        audioPlaying    = observer.Register("audio_playing");
        observer.Subscribe("start", Start);
        observer.Subscribe("update", Update);
        observer.Subscribe("destroy", Destroy);
    }
    public void Start(dynamic[] _args){
        
    }
    public void Update(dynamic[] _args){
        if(audioSource.isPlaying){
            audioPlaying.Invoke(new AudioPlayingEventArgs(
                audioSource.time
            ));
            prevPlaying = true;
        }
        if(prevPlaying && !audioSource.isPlaying){
            audioPlaying.Invoke(new AudioPlayingEventArgs(
                audioSource.time
            ));
        }
    }
    public void Destroy(dynamic[] _args){
        GameObject.DestroyImmediate(audioSourceObject);
    }
    #region VisualElement interaction event callbacks
    void PlayBtnClicked(){
        if(audioSource.clip != null){
            audioSource.Play();
        }
    }
    void PauseBtnClicked(){
        if(audioSource.clip != null){
            audioSource.Pause();
        }
    }
    void StopBtnClicked(){
        if(audioSource.clip != null){
            audioSource.Stop();
            audioSource.time = 0;
        }
    }
    void VolumeSliderValueChanged(ChangeEvent<float> e){
        audioSource.volume = e.newValue;
    }
    void OpenExplorerBtnClicked(){
        string path = EditorUtility.OpenFilePanel("AudioPlayer", "Assets/Resources/Audio", "mp3");
        if(path != ""){
            audioSource.clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(path));
            audioImported.Invoke(new AudioImportedEventArgs(
                audioSource.clip
            ));
            pathDisplay.value = path;
        }
    }
    #endregion
    #region External event callbacks

    #endregion
}
public class AudioImportedEventArgs{
    public AudioClip audioClip{private set; get;}
    public AudioImportedEventArgs(AudioClip _audioClip){
        audioClip = _audioClip;
    }
}
public class AudioPlayingEventArgs{
    public float time{private set; get;}
    public AudioPlayingEventArgs(float _time){
        time = _time;
    }
}