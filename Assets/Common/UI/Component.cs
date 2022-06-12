
using UnityEditor;
using UnityEngine.UIElements;

namespace kgh.UI{
    public class Component{
        public VisualElement rootVisualElement{private set; get;}
        public Component(string uxml){
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxml);
            rootVisualElement = visualTreeAsset.Instantiate();
        }
    }
}
