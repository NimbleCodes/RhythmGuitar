using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using kgh.Signals;

public class Speaker : MonoBehaviour
{
    AnimCtrl animCtrl;
    Dictionary<string, Dictionary<string, string>> transitions;
    string curState;
    public event Action<string> signal;

    void Awake(){
        animCtrl = GetComponent<AnimCtrl>();
        transitions = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(Resources.Load<TextAsset>("v2/Speaker").text);
        signal += (string msg)=>{
            if(transitions[curState].ContainsKey(msg)){
                curState = transitions[curState][msg];
                animCtrl.PlayAnim(curState, false, 1.0f);
            }
        };
    }
    void Start(){
        animCtrl.animationState.Complete += (Spine.TrackEntry trackEntry)=>{ signal.Invoke("spine-complete"); };

        curState = "VIBE";
        animCtrl.PlayAnim("VIBE", false, 2.0f);
    }
}