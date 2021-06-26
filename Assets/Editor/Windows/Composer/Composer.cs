using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Composer : EditorWindow
{
    static GameObject audioSourceObject;
    static AudioSource audioSource;
    static Observer observer;
    static NoteData noteData;
    EventHandle update, destroy;

    [MenuItem("Window/Custom Editors/Composer")]
    public static void OpenWindow(){
        observer          = new Observer();
        audioSourceObject = new GameObject("Composer: audioSourceObject");
        audioSource       = audioSourceObject.AddComponent<AudioSource>();
        noteData          = new NoteData();
        Composer window   = GetWindowWithRect<Composer>(new Rect(0,0,1000,600));
    }
    void CreateGUI(){
        update  = observer.Register("update");
        destroy = observer.Register("destroy");

        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Windows/Composer/Composer.uxml");
        VisualElement visualTree = visualTreeAsset.Instantiate();
        visualTree.style.height = Length.Percent(100);

        AudioPlayer audioPlayer = new AudioPlayer(audioSource, observer, noteData);
        audioPlayer.rootVisualElement.style.height = Length.Percent(40);
        visualTree.Add(audioPlayer.rootVisualElement);
        
        NoteEditor noteEditor = new NoteEditor(audioSource, noteData, observer);
        noteEditor.rootVisualElement.style.height = Length.Percent(50);
        visualTree.Add(noteEditor.rootVisualElement);

        Exporter exporter = new Exporter(noteData);
        exporter.rootVisualElement.style.height = Length.Percent(10);
        visualTree.Add(exporter.rootVisualElement);

        rootVisualElement.Add(visualTree);
    }
    void Update(){
        if(update != null){
            update.Invoke();
        }
    }
    void OnDestroy(){
        if(destroy != null){
            destroy.Invoke();
            Object.DestroyImmediate(audioSourceObject);
        }
    }
}