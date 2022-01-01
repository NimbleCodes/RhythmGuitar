using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using kgh.Signals;

public class Character_0 : MonoBehaviour
{
    AnimCtrl animCtrl;
    Dictionary<string, Dictionary<string, string>> transitions;
    string curState;
    public event Action<string> signal;

    void Awake(){
        animCtrl = GetComponent<AnimCtrl>();
        transitions = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(Resources.Load<TextAsset>("v2/Character_0").text);
        signal += (string msg)=>{
            curState = transitions[curState][msg];
            animCtrl.PlayAnim(curState, false, 1.0f);
        };
    }
    void Start(){
        animCtrl.animationState.Complete += (Spine.TrackEntry trackEntry)=>{ signal.Invoke("spine-complete"); };

        curState = "idle";
        animCtrl.PlayAnim("idle", false, 1.0f);
    }
}
public class Transition
{
    public string dest;
    public string eventType;
}