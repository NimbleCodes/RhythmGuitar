using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TestWindow : EditorWindow
{
    [MenuItem("Custom Editors/TestWindow")]
    public static void OpenWindow()
    {
        TestWindow wnd = GetWindow<TestWindow>();
        wnd.titleContent = new GUIContent("TestWindow");
        wnd.minSize = new Vector2(960, 540);
        wnd.maxSize = new Vector2(960, 540);
    }

    App myApp;
    public void CreateGUI()
    {
        myApp = new App();
        rootVisualElement.Add(myApp.rootVisualElement);
    }
    void Update(){
        myApp.Update();
    }
    void OnDestroy(){
        myApp.Dispose();
    }
}
class App : myUI.Component{
    AudioClip audioClip;
    MCAD mcad;
    ImportExport impexp;
    NoteEditor noteEditor;

    public App(){
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/TestWindow/TestWindow.uxml");
        rootVisualElement = visualTreeAsset.CloneTree();
        rootVisualElement.style.width = Length.Percent(100);
        rootVisualElement.style.height = Length.Percent(100);

        mcad = new MCAD();
        rootVisualElement.Q<VisualElement>("mcad").Add(mcad.rootVisualElement);
        noteEditor = new NoteEditor();
        rootVisualElement.Q<VisualElement>("note-editor").Add(noteEditor.rootVisualElement);
        impexp = new ImportExport();
        rootVisualElement.Q<VisualElement>("import-export").Add(impexp.rootVisualElement);
        
        //FIND A BETTER WAY
        impexp.lanes = noteEditor.lanes;

        children.Add(mcad);
        children.Add(noteEditor);
        children.Add(impexp);
        mcad.parent = this;
        noteEditor.parent = this;
        impexp.parent = this;
    }
    protected override void _Update()
    {
        // Debug.Log("App::Update");
    }
    protected override void Synchronize()
    {
        MCAD.MCADStates mcadStates = (MCAD.MCADStates)mcad.states;
        NoteEditor.NoteEditorStates noteEditorStates = (NoteEditor.NoteEditorStates)noteEditor.states;
        ImportExport.ImportExportStates impexpStates = (ImportExport.ImportExportStates)impexp.states;

        if(impexpStates.audioClip != mcadStates.audioClip){
            mcadStates.audioClip = impexpStates.audioClip;
            noteEditorStates.audioClip = impexpStates.audioClip;
        }

        if(mcadStates.start != noteEditorStates.start)
            noteEditorStates.start = mcadStates.start;
        if(mcadStates.size != noteEditorStates.size)
            noteEditorStates.size = mcadStates.size;
    }
    protected override void _Dispose()
    {
        //do nothing
    }
}