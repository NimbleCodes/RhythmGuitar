using UnityEngine.UIElements;
using UnityEditor;

public class Component 
{
    VisualElement _rootVisualElement;
    public VisualElement rootVisualElement{
        get{
            return _rootVisualElement;
        }
    }
    
    public Component(string uxml){
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxml);
        _rootVisualElement  = visualTreeAsset.Instantiate();
    }
}
