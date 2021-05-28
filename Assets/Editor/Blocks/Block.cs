using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class Block
{
    public VisualElement rootVisualElement;
    List<Block> children;

    public Block(string uxmlPath)
    {
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        rootVisualElement = visualTreeAsset.Instantiate();
        children = new List<Block>();
    }
    public virtual void Update()
    {
        children.ForEach((c)=>c.Update());
    }
    public virtual void Destroy()
    {
        children.ForEach((c)=>c.Destroy());
    }
    public void AddSubBlock(Block subBlock)
    {
        children.Add(subBlock);
        rootVisualElement.Add(subBlock.rootVisualElement);
    }
    public void RemoveSubBlock(Block subBlock)
    {
        children.Remove(subBlock);
        rootVisualElement.Remove(subBlock.rootVisualElement);
    }
}
