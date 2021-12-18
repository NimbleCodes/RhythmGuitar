using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Event = Spine.Event;

public class StringAnimation1 : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState animationState;

    public static StringAnimation1 instance;
    public bool stringShake = false;
    
    public string[] stateArr = {"IDLE","SHAKE"};
    public string curState;

    protected string[] curAnim = new string[3];

    void Awake(){
        instance = this;
        init();
    }

    void init()
    {
        skeletonAnimation = this.GetComponent<SkeletonAnimation>();
        animationState = skeletonAnimation.AnimationState;
        TrackEntry trackEntry = animationState.SetAnimation(0,"IDLE", true);
        animationState.Start += OnSpineStart;
        animationState.Complete += OnSpineComplete;
        animationState.Interrupt += OnSpineInterrupt;
        animationState.End += OnSpineEnd;
        animationState.Event += OnSpineEvent;
        
        curState = stateArr[0];
    }


    #region func for states
    void Idle()
    {
        PlayOneAnimation(0,"IDLE",1);
    }

    public void Shake()
    {
        PlayOneAnimation(0,"SHAKE",1);
    }
    #endregion


    #region eventCall

    void OnSpineStart(Spine.TrackEntry trackEntry)
    {

    }

    void OnSpineComplete(Spine.TrackEntry trackEntry)
    {
        if(curState == stateArr[1] && stringShake && GetCurAnimName(0) == "SHAKE")
        {
            stringShake = false;
            Idle();
        }
    }

    void OnSpineInterrupt(Spine.TrackEntry trackEntry)
    {

    }

    void OnSpineEnd(Spine.TrackEntry trackEntry) // recurrsion 조심 End 쓰지 맙시다/
    {
        
    }

    void OnSpineEvent(Spine.TrackEntry trackEntry, Event e)
    {

    }
    #endregion


    #region animationControl
    public void PlayLoopAnimation(int index, string name, float speed)
    {
        if(curAnim[index] == name)
            return;

        skeletonAnimation.state.SetAnimation(index,name,true).TimeScale = speed;
        curAnim[index] = name;
        curState = curAnim[index];
    }

    public void PlayOneAnimation(int index, string name, float speed)
    {

        skeletonAnimation.state.SetAnimation(index,name,false).TimeScale = speed;
        curAnim[index] = name;
        curState = curAnim[index];
        stringShake = true;
    }

    public string GetCurAnimName(int index)
    {
        return skeletonAnimation.state.GetCurrent(index).Animation.Name;
    }
    #endregion

}