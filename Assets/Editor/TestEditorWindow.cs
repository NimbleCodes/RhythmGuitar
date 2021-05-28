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
    Slider slider;
    public NoteData N_data;
    public bool playing = false;
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
    
    void Update(){
            MoveProgressBarPos();
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

        ///////////// test
         string path = "Assets/Resources/Audio/Haru Modoki (Asterisk DnB Remix Cut).mp3";
        audioClip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(path));
        audioSourceObj = new GameObject("EditorAudioSource");
        audioSourceObj.hideFlags |= HideFlags.HideInHierarchy;
        audioSource = audioSourceObj.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        PaintWave();
        /////////// test
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
            /*if(audioSourceObj == null)
            {
                audioSourceObj = new GameObject("EditorAudioSource");
                audioSourceObj.hideFlags |= HideFlags.HideInHierarchy;
                audioSource = audioSourceObj.AddComponent<AudioSource>();
                audioSource.clip = audioClip;
                PaintWave();
            }*/
            if(audioSourceObj != null)
            {
                /*audioSourceObj = new GameObject("EditorAudioSource");
                audioSourceObj.hideFlags |= HideFlags.HideInHierarchy;
                audioSource = audioSourceObj.AddComponent<AudioSource>();*/
                audioSource.clip = audioClip;
                ChangePath();
            }
        }
    }
    void DestroyOnPathChange(){
    
    }
    void AudioControl(audioClipState state){
        switch((int)state){
            case 0:
                Debug.Log("Play");
                if(audioSource.isPlaying)
                    audioSource.UnPause();
                else
                    audioSource.Play();
                    playing = true;
            break;
            case 1:
                Debug.Log("Pause");
                audioSource.Pause();
                playing = false;
            break;
            case 2:
                Debug.Log("Stop");
                audioSource.Stop();
                slider.value = 0;
                playing = false;
            break;            
        }
    }

    void PaintWave(){
        if(audioClip != null){
            var root = this.rootVisualElement;
            width = 1000;
            height = 200;
            texture = PaintWaveformSpectrum(audioClip, sat, width, height, waveColor);
            slider = new Slider();
            root.Add(slider);
            slider.style.backgroundImage = texture;
            slider.style.height = 100;
            SetProgressBarLength();
            
        }
    }

    void ChangePath(){
        texture = PaintWaveformSpectrum(audioClip, sat, width, height, waveColor);
        slider.style.backgroundImage = texture;
        slider.value = 0;
        SetProgressBarLength();
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
        if(playing == true){
            if(audioSource.clip != null){
                slider.value = audioSource.time;
                }
        }else
        {   if(audioSource.clip != null)
                ControlProgressBarPos();
        }
    }
 void SetMusicLength()
    {
        int audioLength = (int)audioSource.clip.length;

        Min = audioLength / 60;
        Sec = audioLength - Min * 60;
    }
void SetProgressBarLength(){
    slider.lowValue = 0;
    slider.highValue = (int)audioSource.clip.length;
    Debug.Log(audioClip.length);
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

    public void ChangePos(float time)
    {
        float currentTime = audioSource.time;

        currentTime += time;
        currentTime = Mathf.Clamp(currentTime, 0f, audioClip.length - 0.0001f); // 클립 길이에 딱 맞게 자르면 오류가 발생하여 끄트머리 조금 싹뚝
       
        audioSource.time = currentTime; 
        //Debug.Log("현재 음악 위치 " + audioSource.time);
    }

    public void ChangePosByProgressBar(float pos)
    {
        float time =  pos;

        audioSource.time = time;
    }

    public void ControlProgressBarPos() // 사용자 조작에 의한
    {
        float pos = slider.value;
        ChangePosByProgressBar(pos);
    }

    void CalculatePos(float pos)
    {
        float value = audioSource.clip.length * pos;
    }

    void SaveObject(int line, float pos)
    {
        /*
        if (line == 1)
            N_data.noteLine1.Add((int)audioSource.time);
        else if (line == 2)
            N_data.noteLine2.Add((int)audioSource.time);
        else if (line == 3)
            N_data.noteLine3.Add((int)audioSource.time);
        else if (line == 4)
            N_data.noteLine4.Add((int)audioSource.time);
            */
    }

}