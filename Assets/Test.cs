using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public void OnAnimationEnd(){
        GetComponent<Image>().sprite = Resources.Load<Sprite>("FX/note_hit_00");
        GetComponent<RectTransform>().localPosition = new Vector3(-1000,0,0);   
        GetComponent<Animator>().SetBool("note_hit_event", false);
    }
}
