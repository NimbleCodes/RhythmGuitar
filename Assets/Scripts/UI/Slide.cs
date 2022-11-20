using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kgh.Signals;

public class Slide : MonoBehaviour
{
    public GameObject[] strings;
    List<float> stringYPos;
    Vector2 stringsXBound;
    StringAnimation string0;
    StringAnimation1 string1;
    StringAnimation2 string2;
    StringAnimation3 string3;
    const float maxInputTime = 1f;
    float inputTimer = 0;
    Vector3 prevTouchPos;
    bool validTouch;
    bool touchDir;
    int numStrings = 0;
    Switch inputSwitch;

    Vector3 prevTouchWorldPos;

    void Awake(){
        inputSwitch = GameManager.instance.sigs.Register("OnMouseBehavior", typeof(Action<int,int>));
    }
    void Start(){
        prevTouchWorldPos = new Vector3(-1,-1,-1);
        stringYPos = new List<float>();
        stringsXBound = new Vector2();
        stringsXBound.x = strings[0].GetComponent<BoxCollider2D>().bounds.min.x;
        stringsXBound.y = strings[0].GetComponent<BoxCollider2D>().bounds.max.x;
        
        for(int i = 0; i < strings.Length; i++){
            stringYPos.Add(strings[i].transform.position.y);
        }

        string0 = strings[0].GetComponent<StringAnimation>();
        string1 = strings[1].GetComponent<StringAnimation1>();
        string2 = strings[2].GetComponent<StringAnimation2>();
        string3 = strings[3].GetComponent<StringAnimation3>();
    }
    void Update(){
        Touch touch;
        if(Input.touches.Length > 0)
            touch = Input.GetTouch(0);
        else
            return;

        Vector3 touchWorldCoord = Camera.main.ScreenToWorldPoint(touch.position);
        switch(touch.phase){
            case TouchPhase.Began:
                Debug.Log("Touch Began");
            break;
            case TouchPhase.Moved:
                Debug.Log("Touch Moved");
            break;
            case TouchPhase.Stationary:
                Debug.Log("Touch Stationary");
            break;
            case TouchPhase.Ended:
                Debug.Log("Touch Ended");
            break;
            case TouchPhase.Canceled:
                Debug.Log("Touch Canceled");
            break;
        }
    }
}
