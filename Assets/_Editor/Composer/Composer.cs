#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using kgh.Signals;

public class Composer : EditorWindow
{
    string uxml = "Assets/_Editor/Composer/Composer.uxml";
    static Exchange exchange;
    static Switch start, update, destroy;

    [MenuItem("Window/Composer")]
    public static void CreateWindow(){
        exchange    = new Exchange();
        start       = exchange.Register("start", typeof(Action));
        update      = exchange.Register("update", typeof(Action));
        destroy     = exchange.Register("destroy", typeof(Action));

        Composer window = GetWindowWithRect<Composer>(new Rect(0, 0, 1280, 720));
    }
    private void CreateGUI(){
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxml);
        VisualElement visualTree = visualTreeAsset.Instantiate();
        visualTree.name = "composer";
        visualTree.style.width = Length.Percent(100);
        visualTree.style.height = Length.Percent(100);

        AudioPlayer audioPlayer = new AudioPlayer(exchange);
        audioPlayer.rootVisualElement.style.height = Length.Percent(34);
        visualTree.Add(audioPlayer.rootVisualElement);

        NoteEditor noteEditor = new NoteEditor(exchange);
        noteEditor.rootVisualElement.style.height = Length.Percent(66);
        visualTree.Add(noteEditor.rootVisualElement);

        rootVisualElement.Add(visualTree);
        rootVisualElement.RegisterCallback<GeometryChangedEvent>(OnGeoChanged);
    }
    void OnGeoChanged(GeometryChangedEvent e){
        start.Invoke();
        rootVisualElement.UnregisterCallback<GeometryChangedEvent>(OnGeoChanged);
    }
    void Update(){
        update.Invoke();
    }
    void OnDestroy(){
        destroy.Invoke();
    }
}
#endif