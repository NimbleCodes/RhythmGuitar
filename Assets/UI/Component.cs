using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace myUI
{
    public abstract class Component{
        public VisualElement rootVisualElement;
        Func<bool> isDirty;
        Action cleanUp;
        public class States{
            Component component;
            bool _dirty;
            private protected bool dirty{
                set{
                    if(value && component.parent != null)
                        component.parent.states.dirty = true;
                    _dirty = value;
                }
                get{ return _dirty; }
            }
            bool _constUpdate;
            public bool constUpdate{
                set{
                    if(value && component.parent != null)
                        component.parent.states.constUpdate = true;
                    _constUpdate = value;
                }
                get{ return _constUpdate; }
            }
            public States(Component _component){
                component = _component;
                component.isDirty = ()=>{ return (dirty | constUpdate); };
                component.cleanUp = ()=>{ dirty = false; };
            }
        }
        public States states;
        public Component parent;
        public List<Component> children;

        public Component(){
            rootVisualElement = new VisualElement();
            states = new States(this);
            children = new List<Component>();
        }
        public void Update(){
            if(isDirty.Invoke()){
                Synchronize();
                children.ForEach((c)=>c.Update());
                _Update();
                cleanUp.Invoke();
            }
        }
        protected abstract void _Update();
        protected abstract void Synchronize();
        public void Dispose(){
            children.ForEach((c)=>c.Dispose());
            _Dispose();
        }
        protected abstract void _Dispose();
    }
}