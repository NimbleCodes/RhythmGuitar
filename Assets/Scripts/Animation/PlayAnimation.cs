using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Event = Spine.Event;

public class PlayAnimation : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState animationState;

    public static PlayAnimation instance;
    public bool strokePlaying = false;
    

    public string[] stateArr = {"idle","stroke1","stroke2","stroke3"};
    public string curState;//아직 사용안함.

    protected string[] curAnim = new string[3];

    void Awake(){
        instance = this;
        init();
    }

    void init()
    {
        skeletonAnimation = this.GetComponent<SkeletonAnimation>();
        animationState = skeletonAnimation.AnimationState;

        animationState.Start += OnSpineStart;
        animationState.Complete += OnSpineComplete;
        animationState.Interrupt += OnSpineInterrupt;
        animationState.End += OnSpineEnd;
        animationState.Event += OnSpineEvent;
        
        curState = stateArr[0];
    }

    void Update()//Play animation by the state number
    {
        if (curState == stateArr[0])
        {
            Idle();
        }
        else if (curState == stateArr[1])
        {
            Stroke1();
        }
        else if (curState == stateArr[2])
        {
            Stroke2();
        }
        else if (curState == stateArr[3])
        {
            Stroke3();
        }
    }

    #region func for states
    void Idle()
    {
        PlayLoopAnimation(0,"idle",1);
    }

    public void Stroke1()
    {
        PlayOneAnimation(0,"stroke1",1);
    }

    public void Stroke2()
    {
        PlayOneAnimation(0,"stroke2",1);
    }

    public void Stroke3()
    {
        PlayOneAnimation(0,"stroke3",1);
    }
    #endregion


    #region eventCall

    void OnSpineStart(Spine.TrackEntry trackEntry)
    {

    }

    void OnSpineComplete(Spine.TrackEntry trackEntry)
    {
        if(curState == stateArr[1] && strokePlaying && GetCurAnimName(0) == "stroke1")
        {
            strokePlaying = false;
            Idle();
        }
        if(curState == stateArr[2] && strokePlaying && GetCurAnimName(0) == "stroke2")
        {
            strokePlaying = false;
            Idle();
        }

    }

    void OnSpineInterrupt(Spine.TrackEntry trackEntry)
    {
        if(curState == stateArr[1] && strokePlaying && GetCurAnimName(0) == "stroke1")
        {
            skeletonAnimation.state.ClearTracks();
            skeletonAnimation.skeleton.SetToSetupPose();
            Stroke2();

        }
       

    }

    void OnSpineEnd(Spine.TrackEntry trackEntry) // recurrsion 조심 End 쓰지 맙시다 시발럼아
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
        if(curAnim[index] == name)
            return;

        skeletonAnimation.state.SetAnimation(index,name,false).TimeScale = speed;
        curAnim[index] = name;
        curState = curAnim[index];
        strokePlaying = true;
    }

    public string GetCurAnimName(int index)
    {
        return skeletonAnimation.state.GetCurrent(index).Animation.Name;
    }
    #endregion



}
