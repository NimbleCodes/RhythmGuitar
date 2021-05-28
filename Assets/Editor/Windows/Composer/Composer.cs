using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class Composer : EditorWindow
{
    static int windowWidth = 1000;
    static int windowHeight = 600;
    Block composerBlock;
    [MenuItem("Window/Custom Editors/Composer")]
    public static void OpenWindow()
    {
        Composer window = GetWindowWithRect<Composer>(new Rect(0,0,windowWidth,windowHeight));
    }
    public void CreateGUI()
    {
        composerBlock = new Block("Assets/Editor/Windows/Composer/Composer.uxml");
        composerBlock.rootVisualElement.name = "composer";

        AudioPlayer audioPlayer = new AudioPlayer();
        audioPlayer.rootVisualElement.style.height = position.height * 0.4f;
        composerBlock.AddSubBlock(audioPlayer);

        NoteEditor noteEditor = new NoteEditor(audioPlayer.audioSource);
        noteEditor.rootVisualElement.style.height = position.height * 0.6f;
        composerBlock.AddSubBlock(noteEditor);
        
        rootVisualElement.Add(composerBlock.rootVisualElement);
    }
    void Update()
    {
        composerBlock.Update();
    }
    void OnDestroy()
    {
        composerBlock.Destroy();
    }
}