using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class DropDown{
    public VisualElement root;
    List<Action> actions;
    public Vector3 startPos;

    public DropDown((string, Action<Vector3>)[] elements){
        var dropDownVTA = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI/DropDown/DropDown.uxml");
        root = dropDownVTA.CloneTree();
        root.name = "drop-down";
        root.style.position = Position.Absolute;
        root.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
        root.style.width = 150;
        root.style.height = 25 * elements.Length;
        foreach(var element in elements){
            VisualElement elementVE = new VisualElement();
            elementVE.AddToClassList("dd-element");
            elementVE.RegisterCallback<MouseDownEvent>((e)=>{
                e.StopPropagation();
            });
            elementVE.RegisterCallback<MouseMoveEvent>((e)=>{
                e.StopPropagation();
            });
            elementVE.RegisterCallback<MouseUpEvent>((e)=>{
                if(e.button == (int)MouseButton.LeftMouse)
                    element.Item2.Invoke(new Vector3(root.style.left.value.value, root.style.top.value.value, 0));
                e.StopPropagation();
            });
            Label label = new Label();
            label.text = element.Item1;
            elementVE.Add(label);
            elementVE.style.justifyContent = Justify.Center;
            root.Q<VisualElement>("dd-elements").Add(elementVE);
        }
        root.focusable = true;
        root.RegisterCallback<FocusOutEvent>((e)=>{
            root.style.left = -500;
        });
    }
}