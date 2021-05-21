using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class Lane : Block
{
    VisualElement laneLabel;
    VisualElement laneNotes;
    public Lane(int laneNum)
        : base("Assets/Editor/Composer/Blocks/NoteEditor/Lanes/Lane.uxml")
    {
        laneLabel = rootVisualElement.Query<VisualElement>("lane_label");
        laneNotes = rootVisualElement.Query<VisualElement>("lane_notes");
    }
}