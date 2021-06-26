using UnityEngine.UIElements;
using UnityEditor;

public class Component
{
    protected VisualElement _rootVisualElement;
    public VisualElement rootVisualElement{
        get{
            return _rootVisualElement;
        }
    }

    public Component(string uxmlPath){
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        if(visualTreeAsset != null){
            _rootVisualElement = visualTreeAsset.Instantiate();
        }
    }
}
