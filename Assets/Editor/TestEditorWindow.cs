using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using UnityEngine.Windows;

public class TestEditorWindow : EditorWindow
{
    Image image;
    AudioClip audioClip;
    GameObject audioSourceObj;
    AudioSource audioSource;
    Texture2D texture;
    Slider progressBar;
    public TextElement time;
    public int width,height;
    public Color waveColor = Color.yellow;
    public Color bgColor = Color.black;
    public float sat = .5f;
    public int Min,Sec,Musicmin,Musicsec;
    enum audioClipState{
        play, pause, stop
    }
    [MenuItem("Window/TestEditorWindow")]
    public static void ShowExample()
    {
        TestEditorWindow wnd = GetWindow<TestEditorWindow>();
        wnd.titleContent = new GUIContent("TestEditorWindow");
    }
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/TestEditorWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML); 
        //Buttons
        Button openExplorerBtn = rootVisualElement.Query<Button>("OpenExplorerBtn");
        openExplorerBtn.clickable.clicked += OpenExplorer;

        Button playBtn = rootVisualElement.Query<Button>("Play");
        Button pauseBtn = rootVisualElement.Query<Button>("Pause");
        Button stopBtn = rootVisualElement.Query<Button>("Stop");
        playBtn.clickable.clicked += ()=>AudioControl(audioClipState.play);
        pauseBtn.clickable.clicked += ()=>AudioControl(audioClipState.pause);
        stopBtn.clickable.clicked += ()=>AudioControl(audioClipState.stop);

        Slider slide = new Slider();
        root.Add(slide);
    }
    private void OnDestroy() {
        audioSource.Stop();
        GameObject.DestroyImmediate(audioSourceObj);
    }
    void OpenExplorer(){
        string path = EditorUtility.OpenFilePanel("Audio file explorer",
            "Assets/Resources/Audio",
            "mp3,mp4"
        );
        if(path.Length != 0){
            audioClip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(path));
            if(audioSourceObj == null)
            {
                audioSourceObj = new GameObject("EditorAudioSource");
                audioSourceObj.hideFlags |= HideFlags.HideInHierarchy;
                audioSource = audioSourceObj.AddComponent<AudioSource>();
                audioSource.clip = audioClip;
                PaintWave();
            }
            if(audioSourceObj != null)
            {
                audioSourceObj = new GameObject("EditorAudioSource");
                audioSourceObj.hideFlags |= HideFlags.HideInHierarchy;
                audioSource = audioSourceObj.AddComponent<AudioSource>();
                audioSource.clip = audioClip;
                PaintWave();
            }
        }
    }
    void AudioControl(audioClipState state){
        switch((int)state){
            case 0:
                Debug.Log("Play");
                if(audioSource.isPlaying)
                    audioSource.UnPause();
                else
                    audioSource.Play();
            break;
            case 1:
                Debug.Log("Pause");
                audioSource.Pause();
            break;
            case 2:
                Debug.Log("Stop");
                audioSource.Stop();
            break;            
        }
    }

    void PaintWave(){
        if(audioClip != null){
            width = 1000;
            height = 200;
            texture = PaintWaveformSpectrum(audioClip, sat, width, height, waveColor);
            //img.overrideSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            ScrollView scrView = rootVisualElement.Query<ScrollView>("Waveform");
            VisualElement wfimg = scrView.Query("WaveformImg");
            wfimg.style.backgroundImage = texture;
        }
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

public void MoveProgressBarPos() // 음악진행에 의한
    {
        if(audioSource.clip != null)
            progressBar.value = audioSource.time / audioSource.clip.length;
    }
 void SetMusicLength()
    {
        int audioLength = (int)audioSource.clip.length;

        Min = audioLength / 60;
        Sec = audioLength - Min * 60;
    }

public void ChangeProgressTimeText()
    {
        int currentTime = (int)audioSource.time;

        if (currentTime != 0)
        {
            Musicmin = currentTime / 60;
            Musicsec = currentTime - Musicmin * 60;
        }

        time.text = string.Format("{0}:{1} / {2}:{3}", Musicmin, Musicsec, Min, Sec);
    }

}