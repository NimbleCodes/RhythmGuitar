using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.UIElements;
using EditorWndMods;
using System.Collections.Generic;
using System;

public class Composer : EditorWindow
{
    List<Module> modules;

    [MenuItem("Window/Custom/Composer")]
    public static void CreateWnd(){
        Composer wnd = GetWindowWithRect<Composer>(new Rect(0,0,800,400));
    }
    public void CreateGUI(){
        modules = new List<Module>();
        //Import uxml
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Composer/Composer.uxml");
        VisualElement visualTree = visualTreeAsset.Instantiate();
        visualTree.name = "composer";
        //Add functionality here
        AudioPlayer audioPlayer = new AudioPlayer();
        audioPlayer.rootVisualElement.style.height = position.height * 0.5f;
        visualTree.Add(audioPlayer.rootVisualElement);
        modules.Add(audioPlayer);
        //Add visualTree to rootVisualElement
        rootVisualElement.Add(visualTree);
    }
    private void Update() {
        //Debug.Log("Update: Composer");
        modules.ForEach((m)=>{
            m.Update();
        });
    }
    private void OnDestroy(){
        modules.ForEach((m)=>{
            m.Destroy();
        });
    }
}