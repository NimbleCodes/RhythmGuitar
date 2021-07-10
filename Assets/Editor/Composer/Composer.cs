using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Composer : EditorWindow
{
    static Observer observer;
    static Switch start, update, destroy;

    [MenuItem("Window/Custom Editors/Composer")]
    public static void OpenWindow()
    {
        observer    = new Observer();
        start       = observer.Register("start");
        update      = observer.Register("update");
        destroy     = observer.Register("destroy");

        Composer wnd = GetWindowWithRect<Composer>(new Rect(0,0,1000,600));
        wnd.titleContent.text = "Composer";
    }
    void CreateGUI()
    {
        AudioPlayer audioPlayer = new AudioPlayer(observer);
        audioPlayer.rootVisualElement.style.height = Length.Percent(10);
        rootVisualElement.Add(audioPlayer.rootVisualElement);

        AudioDisplay audioDisplay = new AudioDisplay(observer);
        audioDisplay.rootVisualElement.style.height = Length.Percent(25);
        rootVisualElement.Add(audioDisplay.rootVisualElement);

        NoteEditor noteEditor = new NoteEditor(observer);
        noteEditor.rootVisualElement.style.height = Length.Percent(40);
        rootVisualElement.Add(noteEditor.rootVisualElement);

        rootVisualElement.RegisterCallback<GeometryChangedEvent>(GeometryChanged);
    }
    void GeometryChanged(GeometryChangedEvent e){
        start.Invoke();
        rootVisualElement.UnregisterCallback<GeometryChangedEvent>(GeometryChanged);
    }
    void Update(){
        update.Invoke();
    }
    void OnDestroy(){
        destroy.Invoke();
    }
}