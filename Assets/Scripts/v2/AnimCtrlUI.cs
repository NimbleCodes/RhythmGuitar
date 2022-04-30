using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

public class AnimCtrlUI : MonoBehaviour
{
    Spine.Unity.SkeletonGraphic skeletonGraphic;
    public Spine.AnimationState animationState;
    List<string> _states;
    public ReadOnlyCollection<string> states;

    void Awake(){
        skeletonGraphic     = GetComponent<Spine.Unity.SkeletonGraphic>();
        animationState      = skeletonGraphic.AnimationState;
        _states             = new List<string>();
        states              = _states.AsReadOnly();

        skeletonGraphic.SkeletonData.Animations.ForEach((Spine.Animation state)=>{
            _states.Add(state.Name);
        });
    }

    public void PlayAnim(string stateName, bool loop, float timeScale){
        if(!_states.Exists((i)=>i==stateName)){
            return;
        }
        animationState.SetAnimation(0, stateName, loop).TimeScale = timeScale;
        
    }
}