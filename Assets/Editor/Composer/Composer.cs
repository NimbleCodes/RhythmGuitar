using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using UnityEditor.UIElements;

public class Composer : EditorWindow
{
    AudioClip audioClip;
    GameObject audioSourceObj;
    AudioSource audioSource;
    Texture2D audioWaveform;
    Texture2D cursorPositionIndicator;
    Texture2D playPositionIndicator;
    VisualElement playPositionIndicatorVE;
    VisualElement waveformImg;

    [MenuItem("Window/Custom/Composer")]
    public static void ShowExample()
    {
        Composer wnd = GetWindow<Composer>();
        wnd.titleContent = new GUIContent("Composer");
        wnd.maxSize = new Vector2(700, 400);
        wnd.minSize = new Vector2(700, 400);
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        audioSourceObj = new GameObject();
        audioSource = audioSourceObj.AddComponent<AudioSource>();
        audioSource.volume = 0.5f;
        audioSourceObj.hideFlags = HideFlags.HideAndDontSave;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Composer/Composer.uxml");
        VisualElement visualTreeInst = visualTree.Instantiate();

        //Add functionality here
        audioWaveform = new Texture2D(700, 400);
        for(int i = 0; i < audioWaveform.width; i++){
            for(int j = 0; j < audioWaveform.height; j++){
                audioWaveform.SetPixel(i, j, Color.black);
            }
        }
        audioWaveform.Apply();
        cursorPositionIndicator = new Texture2D(1, 1);
        cursorPositionIndicator.SetPixel(0,0,new Color(.8f, .8f, .8f, .5f));
        cursorPositionIndicator.Apply();
        waveformImg = visualTreeInst.Query<VisualElement>("waveform-img");
        VisualElement cursorPositionIndicatorVE = visualTreeInst.Query<VisualElement>("cursor-position-indicator");
        Label cursorPositionTooltip = visualTreeInst.Query<Label>("tooltip");
        waveformImg.style.backgroundImage = audioWaveform;
        cursorPositionIndicatorVE.style.backgroundImage = cursorPositionIndicator;
        waveformImg.RegisterCallback<MouseMoveEvent>((e)=>{
            cursorPositionIndicatorVE.style.left = e.localMousePosition.x;
            cursorPositionTooltip.text = "00:00:00";
            if(e.localMousePosition.x > 350){
                cursorPositionTooltip.style.left = -cursorPositionTooltip.worldBound.size.x;
            }
            else{
                cursorPositionTooltip.style.left = 0;
            }
        });
        waveformImg.RegisterCallback<MouseDownEvent>((e)=>{
            Debug.Log("hi");
            float gotopos = e.localMousePosition.x / waveformImg.worldBound.size.x;
            audioSource.time = audioClip.length * gotopos;
        });
        
        playPositionIndicator = new Texture2D(1,1);
        playPositionIndicator.SetPixel(0, 0, new Color(1, .55f, 0));
        playPositionIndicator.Apply();
        playPositionIndicatorVE = visualTreeInst.Query<VisualElement>("play-position-indicator");
        playPositionIndicatorVE.style.backgroundImage = playPositionIndicator;

        //Buttons
        Button playBtn = visualTreeInst.Query<Button>("play-btn");
        playBtn.clicked += ()=>{
            if(audioSource.isPlaying) audioSource.UnPause();
            else audioSource.Play();
        };
        Button pauseBtn = visualTreeInst.Query<Button>("pause-btn");
        pauseBtn.clicked += ()=>{
            audioSource.Pause();
        };
        Button stopBtn = visualTreeInst.Query<Button>("stop-btn");
        stopBtn.clicked += ()=>{
            audioSource.time = 0;
            audioSource.Stop();
        };
        Button openExplorerBtn = visualTreeInst.Query<Button>("open-explorer-btn");
        openExplorerBtn.clicked += ()=>{
            string path = EditorUtility.OpenFilePanel("Audio file explorer", Application.dataPath + "/Resources/Audio", "mp3");
            if(path.Length == 0) return;
            if(audioClip != null) DestroyImmediate(audioClip);
            audioClip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(path));
            audioSource.clip = audioClip;
            //waveform
            float[] samples = new float[audioClip.samples];
            float[] waveform = new float[audioWaveform.width];
            audioClip.GetData(samples, 0);
            int packSize = (audioClip.samples / audioWaveform.width) + 1;
            for(int i = 0, s = 0; i < audioClip.samples; i+=packSize, s++){
                waveform[s] = Mathf.Abs(samples[i]);
            }
            for(int i = 0; i < audioWaveform.width; i++){
                for(int j = 0; j < audioWaveform.height; j++){
                    audioWaveform.SetPixel(i, j, Color.black);
                }   
            }
            for(int x = 0; x < audioWaveform.width; x++){
                for(int y = 0; y < waveform[x] * audioWaveform.height; y++){
                    audioWaveform.SetPixel(x, (audioWaveform.height/2)+y, Color.yellow);
                    audioWaveform.SetPixel(x, (audioWaveform.height/2)-y, Color.yellow);
                }
            }
            audioWaveform.Apply();
        };

        root.Add(visualTreeInst);
    }
    private void Update() {
        if(audioSource.isPlaying){
            //playPositionIndicator
            float curPlayPos = audioSource.time / audioClip.length;
            playPositionIndicatorVE.style.left = waveformImg.worldBound.size.x * curPlayPos;
            //Debug.Log(playPositionIndicatorVE.style.left);
        }
    }
    private void OnDestroy() {
        if(audioSourceObj != null) DestroyImmediate(audioSourceObj);
        if(audioClip != null) DestroyImmediate(audioClip);
    }
}