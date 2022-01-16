using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Spine.Unity.SkeletonAnimation))]
public class AnimCtrl : MonoBehaviour
{
    Spine.Unity.SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState animationState;
    List<string> _states;
    public ReadOnlyCollection<string> states;

    void Awake(){
        skeletonAnimation   = GetComponent<Spine.Unity.SkeletonAnimation>();
        animationState      = skeletonAnimation.AnimationState;
        _states             = new List<string>();
        states              = _states.AsReadOnly();

        skeletonAnimation.skeleton.Data.Animations.ForEach((Spine.Animation state)=>{
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