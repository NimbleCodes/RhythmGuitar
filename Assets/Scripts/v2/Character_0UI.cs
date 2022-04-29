using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using kgh.Signals;

public class Character_0UI : MonoBehaviour
{
    AnimCtrlUI animCtrl;
    Dictionary<string, Dictionary<string, string>> transitions;
    string curState;
    public event Action<string> signal;

    void Awake(){
        animCtrl = GetComponent<AnimCtrlUI>();
        transitions = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(Resources.Load<TextAsset>("v2/Character_0").text);
        signal += (string msg)=>{
            if(transitions[curState].ContainsKey(msg)){
                curState = transitions[curState][msg];
                animCtrl.PlayAnim(curState, false, 1.0f);
            }
        };
    }
    void Start(){
        animCtrl.animationState.Complete += (Spine.TrackEntry trackEntry)=>{ signal.Invoke("spine-complete"); };
        GameManager.instance.sigs.Subscribe("OnMouseBehavior", this, "OnMouseBehavior");

        curState = "idle";
        animCtrl.PlayAnim("idle", false, 1.0f);
    }
    void OnMouseBehavior(int key, int lineCount){
        signal.Invoke("mouse-" + key);
    }
}