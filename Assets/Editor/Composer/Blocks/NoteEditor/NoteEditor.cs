using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class NoteEditor : Block
{
    List<VisualElement> lanes;
    VisualElement cursorPosIndicator;
    VisualElement editor;
    int curVisibleAreaStart = 0;
    int curVisibleAreaSize = 60;
    NoteData noteData;
    Queue<VisualElement> noteIndicator;
    public NoteEditor() 
        : base("Assets/Editor/Composer/Blocks/NoteEditor/NoteEditor.uxml")
    {
        rootVisualElement.name = "note_editor";
        lanes = new List<VisualElement>();
        cursorPosIndicator = rootVisualElement.Query<VisualElement>("cursor_pos_indicator");
        editor = rootVisualElement.Query<VisualElement>("editor");
        noteData = new NoteData();
        noteData.Init();
        #region Functionality
        #region Editor
        var laneVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Composer/Blocks/NoteEditor/Lanes/Lane.uxml");
        for(int i = 0; i < 4; i++)
        {
            VisualElement newLane = laneVisualTreeAsset.Instantiate();
            VisualElement laneLabel = newLane.Query<VisualElement>("lane_label");
            VisualElement laneNotes = newLane.Query<VisualElement>("lane_notes");
            Label laneNum = newLane.Query<Label>("lane_num");
            laneNum.text = (i+1).ToString();
            newLane.style.height = Length.Percent(25);
            newLane.RegisterCallback<MouseDownEvent>((e)=>{
                float a = (e.localMousePosition.x - laneLabel.worldBound.width) / laneNotes.worldBound.width;
                int b = curVisibleAreaStart + (int)(curVisibleAreaSize * a);
                switch(laneNum.text){
                    case "1":
                        noteData.noteLine1.Add(b);
                        break;
                    case "2":
                        noteData.noteLine2.Add(b);
                        break;
                    case "3":
                        noteData.noteLine3.Add(b);
                        break;
                    case "4":
                        noteData.noteLine4.Add(b);
                        break;
                }
                UpdateDisplay();
            });
            newLane.RegisterCallback<WheelEvent>((e)=>{
                curVisibleAreaSize += (int)(e.delta.y * -1) * 10;
                UpdateDisplay();
            });
            lanes.Add(newLane);
            editor.Add(newLane);
        }
        cursorPosIndicator.BringToFront();
        editor.RegisterCallback<MouseMoveEvent>((e)=>{
            VisualElement laneLabel = editor.Query<VisualElement>("lane_label");
            float laneLabelXMax = laneLabel.worldBound.xMax;
            if(e.localMousePosition.x > laneLabelXMax)
                cursorPosIndicator.style.left = e.localMousePosition.x - 2;
        });
        #endregion
        #region Buttons

        #endregion
        #endregion

        noteIndicator = new Queue<VisualElement>();
        var noteIndicatorVTAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Composer/Blocks/NoteEditor/NoteIndicator.uxml");
        for(int i = 0; i < 100; i++){
            VisualElement newNoteIndicator = noteIndicatorVTAsset.Instantiate();
            newNoteIndicator.style.minHeight = 5;
            newNoteIndicator.style.maxWidth = 5;
            noteIndicator.Enqueue(newNoteIndicator);
        }
        VisualElement temp = noteIndicator.Dequeue();
        VisualElement temp2 = lanes[0].Query<VisualElement>("lane_notes");
        temp.BringToFront();
        temp.style.left = 50;
        temp2.Add(temp);
        noteIndicator.Enqueue(temp);
    }
    private void UpdateDisplay(){
        
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Destroy()
    {
        base.Destroy();
    }
}