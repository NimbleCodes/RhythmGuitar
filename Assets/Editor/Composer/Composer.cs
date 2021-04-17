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
        audioWaveform = new Texture2D(1000, 400);
        for(int i = 0; i < audioWaveform.width; i++){
            for(int j = 0; j < audioWaveform.height; j++){
                audioWaveform.SetPixel(i, j, Color.black);
            }
        }
        cursorPositionIndicator = new Texture2D(1, 400);
        for(int i = 0; i < cursorPositionIndicator.height; i++){
            
        }
        audioWaveform.Apply();
        VisualElement waveformImg = visualTreeInst.Query<VisualElement>("waveform-img");
        waveformImg.style.backgroundImage = audioWaveform;
        waveformImg.RegisterCallback<MouseOverEvent>((e)=>{
            //do something

        });
        waveformImg.RegisterCallback<MouseOutEvent>((e)=>{
            //do something
        });

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
            audioClip.GetData(samples, 0);
            //float[] packets = new float[]
        };

        root.Add(visualTreeInst);
    }
    private void OnDestroy() {
        if(audioSourceObj != null) DestroyImmediate(audioSourceObj);
        if(audioClip != null) DestroyImmediate(audioClip);
    }
}