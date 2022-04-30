using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using kgh.Signals;

public class SpeakerUI : MonoBehaviour
{
    AnimCtrlUI animCtrlUI;
    Dictionary<string, Dictionary<string, string>> transitions;
    string curState;
    public event Action<string> signal;

    void Awake(){
        animCtrlUI = GetComponent<AnimCtrlUI>();
        transitions = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(Resources.Load<TextAsset>("v2/Speaker").text);
        signal += (string msg)=>{
            if(transitions[curState].ContainsKey(msg)){
                curState = transitions[curState][msg];
                animCtrlUI.PlayAnim(curState, false, 1.0f);
            }
        };
    }
    void Start(){
        animCtrlUI.animationState.Complete += (Spine.TrackEntry trackEntry)=>{ signal.Invoke("spine-complete"); };

        curState = "VIBE";
        animCtrlUI.PlayAnim("VIBE", false, 2.0f);
    }
}