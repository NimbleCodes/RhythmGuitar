using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteHitFX : MonoBehaviour
{
    public GameObject noteHitFXPrefab;
    public GameObject temp;
    Animator tempAnimator;

    void Awake(){
        temp = Instantiate(noteHitFXPrefab);
        temp.transform.SetParent(gameObject.transform);
        temp.GetComponent<RectTransform>().localPosition = new Vector3(-1000,0,0);
        tempAnimator = temp.GetComponent<Animator>();
    }
    public void NoteHitFXStart(Vector2 position){
        temp.GetComponent<Animator>().Play("idle", -1, 0f);
        temp.GetComponent<RectTransform>().position = new Vector3(position.x, position.y, 0);
        temp.GetComponent<Animator>().SetBool("note_hit_event", true);
    }
}
