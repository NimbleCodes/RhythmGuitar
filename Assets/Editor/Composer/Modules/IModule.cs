using UnityEngine;

namespace EditorWndMods {
    public abstract class Module {
        public virtual void Update(){
            //Debug.Log("Update: Module");
            //Implement update functionality here
        }
        public virtual void Destroy(){
            //Implement clean up here
        }
    }
}