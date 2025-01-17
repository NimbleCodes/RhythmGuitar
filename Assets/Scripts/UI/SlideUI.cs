using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kgh.Signals;

public class SlideUI : MonoBehaviour
{
    public GameObject[] strings;
    List<float> stringYPos;
    Vector2 stringsXBound;
    StringAnimationUI string0;
    StringAnimationUI1 string1;
    StringAnimationUI2 string2;
    StringAnimationUI3 string3;

    int prevDirection = -1;
    Vector3 prevTouchPos;
    bool validTouch;
    Switch inputSwitch;

    Vector3 startPos;

    void Awake(){
        inputSwitch = GameManager.instance.sigs.Register("OnMouseBehavior", typeof(Action<int,int>));
    }
    void Start(){
        stringYPos = new List<float>();
        stringsXBound = new Vector2();
        stringsXBound.x = strings[0].GetComponent<BoxCollider2D>().bounds.min.x;
        stringsXBound.y = strings[0].GetComponent<BoxCollider2D>().bounds.max.x;
        
        for(int i = 0; i < strings.Length; i++){
            stringYPos.Add(strings[i].GetComponent<RectTransform>().position.y);
        }

        string0 = strings[0].GetComponent<StringAnimationUI>();
        string1 = strings[1].GetComponent<StringAnimationUI1>();
        string2 = strings[2].GetComponent<StringAnimationUI2>();
        string3 = strings[3].GetComponent<StringAnimationUI3>();

        startPos = new Vector3(-1, -1, 0);
    }
    void Update(){
        validTouch = false;
        int lineCount = 0;
        foreach(var touch in Input.touches){
            if(touch.phase == TouchPhase.Began){
                startPos.x = touch.position.x;
                startPos.y = touch.position.y;
                prevTouchPos = touch.position;
            }
            else if(touch.phase == TouchPhase.Canceled){
                if(prevDirection != -1)
                    inputSwitch.Invoke(prevDirection, lineCount);
            }
            else if(touch.phase == TouchPhase.Ended){
                if(prevDirection != -1)
                    inputSwitch.Invoke(prevDirection, lineCount);
            }
            else if(touch.phase == TouchPhase.Moved){
                float min, max;
                min = Mathf.Min(touch.position.y, prevTouchPos.y);
                max = Mathf.Max(touch.position.y, prevTouchPos.y);
                if(stringYPos[0] >= min && stringYPos[0] <= max){
                    string0.Shake();
                    lineCount++;
                }
                if(stringYPos[1] >= min && stringYPos[1] <= max){
                    string1.Shake();
                    lineCount++;
                }
                if(stringYPos[2] >= min && stringYPos[2] <= max){
                    string2.Shake();
                    lineCount++;
                }
                if(stringYPos[3] >= min && stringYPos[3] <= max){
                    string3.Shake();
                    lineCount++;
                }
                prevTouchPos = touch.position;
                int direction;
                if(touch.position.y < prevTouchPos.y){
                    //direction down
                    direction = 0;
                }else{
                    //direction up
                    direction = 1;
                }
                if(prevDirection != -1 && prevDirection != direction){
                    inputSwitch.Invoke(direction, lineCount);
                    lineCount = 0;
                    prevDirection = -1;
                }
                prevDirection = direction;
            }
            else if(touch.phase == TouchPhase.Stationary){

            }
        }
    }
}
