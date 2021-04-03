using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class TestEditorWindow : EditorWindow
{
    AudioClip audioClip;
    GameObject audioSourceObj;
    AudioSource audioSource;
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
    }
    private void OnDestroy() {
        audioSource.Stop();
        GameObject.DestroyImmediate(audioSourceObj);
    }
    void OpenExplorer(){
        string path = EditorUtility.OpenFilePanel("Audio file explorer",
            "C:/Users/kylek/Desktop/Code/Unity/Projects/KusoGame/5/KusoRhythmGame/Assets/Resources/Audio",
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
}