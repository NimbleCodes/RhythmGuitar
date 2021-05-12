using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace EditorWndMods {
    public class EditModule : Module
    {
        VisualElement _rootVisualElement;
        public VisualElement rootVisualElement {
            get{
                return _rootVisualElement;
            }
        }
        int curLaneNum = 0;
        public EditModule()
        {
            //Import uxml
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Composer/Modules/EditModule/EditModule.uxml");
            _rootVisualElement = visualTreeAsset.Instantiate();
            _rootVisualElement.name = "edit_module";
            //Add functionality here
            VisualElement noteInfo = rootVisualElement.Query<VisualElement>("note_info");
            Button addLaneBtn = rootVisualElement.Query<Button>("add_lane_btn");
            addLaneBtn.clicked += ()=>{
                VisualElement newLaneObj = new VisualElement();
                newLaneObj.AddToClassList("laneObj");
                Label newLaneObj_laneNum = new Label(){text = (++curLaneNum).ToString()};
                newLaneObj_laneNum.AddToClassList("laneObj_laneNumber");
                VisualElement newLaneObj_noteInfo = new VisualElement();
                newLaneObj_noteInfo.AddToClassList("laneObj_laneInfo");
                newLaneObj_noteInfo.RegisterCallback<MouseDownEvent>((e)=>{
                    
                });
                newLaneObj.Add(newLaneObj_laneNum);
                newLaneObj.Add(newLaneObj_noteInfo);
                noteInfo.Add(newLaneObj);
            };
        }
        override public void Update(){

        }
        public override void Destroy()
        {
            
        }
    }
}