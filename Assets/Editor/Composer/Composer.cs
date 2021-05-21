using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class Composer : EditorWindow
{
    List<Block> blocks;
    [MenuItem("Window/Custom Editor Windows/Composer")]
    public static void OpenWindow()
    {
        Composer composer = GetWindowWithRect<Composer>(new Rect(0,0,1000,600));
    }
    void CreateGUI(){
        blocks = new List<Block>();
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Composer/Composer.uxml");
        VisualElement visualTree = visualTreeAsset.Instantiate();
        visualTree.name = "composer";

        AudioPlayer audioPlayer = new AudioPlayer();
        audioPlayer.rootVisualElement.style.height = position.height * 0.3f;
        visualTree.Add(audioPlayer.rootVisualElement);
        blocks.Add(audioPlayer);
        
        NoteEditor noteEditor = new NoteEditor();
        noteEditor.rootVisualElement.style.height = position.height * 0.5f;
        visualTree.Add(noteEditor.rootVisualElement);
        blocks.Add(noteEditor);

        rootVisualElement.Add(visualTree);
    }
    void Update()
    {
        blocks.ForEach((block)=>{block.Update();});
    }
    void OnDestroy()
    {
        blocks.ForEach((block)=>{block.Destroy();});
    }
}