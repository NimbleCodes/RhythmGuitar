using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public abstract class Block
{
    protected List<Block> subBlocks;
    protected VisualElement _rootVisualElement;
    public VisualElement rootVisualElement
    {
        get
        {
            return _rootVisualElement;
        }
    }
    
    public Block(string uxmlPath)
    {
        subBlocks = new List<Block>();
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        _rootVisualElement = visualTreeAsset.Instantiate();
    }
    //must execute if sub-blocks exist
    virtual public void Update()
    {
        subBlocks.ForEach((block)=>{block.Update();});
    }
    //must execute if sub-blocks exist
    virtual public void Destroy()
    {
        subBlocks.ForEach((block)=>{block.Destroy();});
    }
}
