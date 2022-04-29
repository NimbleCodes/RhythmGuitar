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
    const float maxInputTime = 1f;
    float inputTimer = 0;
    Vector3 prevTouchPos;
    bool validTouch;
    bool touchDir;
    int numStrings = 0;
    Switch inputSwitch;

    void Awake(){
        inputSwitch = GameManager.instance.sigs.Register("OnMouseBehavior", typeof(Action<int,int>));
    }
    void Start(){
        stringYPos = new List<float>();
        stringsXBound = new Vector2();
        stringsXBound.x = strings[0].GetComponent<BoxCollider2D>().bounds.min.x;
        stringsXBound.y = strings[0].GetComponent<BoxCollider2D>().bounds.max.x;
        
        // Debug.Log(stringsXBound.x + ", " + stringsXBound.y);
        
        // string tot = "";
        for(int i = 0; i < strings.Length; i++){
            stringYPos.Add(strings[i].GetComponent<RectTransform>().position.y);
            // tot += strings[i].GetComponent<RectTransform>().position.y + ", ";
        }
        // Debug.Log(tot);

        string0 = strings[0].GetComponent<StringAnimationUI>();
        string1 = strings[1].GetComponent<StringAnimationUI1>();
        string2 = strings[2].GetComponent<StringAnimationUI2>();
        string3 = strings[3].GetComponent<StringAnimationUI3>();
    }
    void Update(){
        Touch touch;
        if(Input.touches.Length > 0)
            touch = Input.GetTouch(0);
        else
            return;
        
        if(touch.position.x >= stringsXBound.x && touch.position.x <= stringsXBound.y){
            switch(touch.phase){
                case TouchPhase.Began:
                    validTouch = false;
                    if(touch.position.y >= strings[0].transform.position.y){
                        validTouch = true;
                        touchDir = true;
                    }
                    if(touch.position.y <= strings[(strings.Length - 1)].transform.position.y){
                        validTouch = true;
                        touchDir = false;
                    }
                break;
                case TouchPhase.Moved:
                    if(validTouch){
                        if(inputTimer > maxInputTime){
                            touch.phase = TouchPhase.Ended;
                        }
                        else {
                            if(
                                ((touchDir && stringYPos[0] > touch.position.y && stringYPos[0] < prevTouchPos.y) ||
                                (!touchDir && stringYPos[0] < touch.position.y && stringYPos[0] > prevTouchPos.y))
                            )
                            {
                                string0.Shake();
                                numStrings++;
                            }
                            if(
                                ((touchDir && stringYPos[1] > touch.position.y && stringYPos[1] < prevTouchPos.y) ||
                                (!touchDir && stringYPos[1] < touch.position.y && stringYPos[1] > prevTouchPos.y))
                            )
                            {
                                string1.Shake();   
                                numStrings++;     
                            }
                            if(
                                ((touchDir && stringYPos[2] > touch.position.y && stringYPos[2] < prevTouchPos.y) ||
                                (!touchDir && stringYPos[2] < touch.position.y && stringYPos[2] > prevTouchPos.y))
                            )
                            {
                                string2.Shake();    
                                numStrings++;    
                            }
                            if(
                                ((touchDir && stringYPos[3] > touch.position.y && stringYPos[3] < prevTouchPos.y) ||
                                (!touchDir && stringYPos[3] < touch.position.y && stringYPos[3] > prevTouchPos.y))
                            )
                            {
                                string3.Shake();   
                                numStrings++;
                            }
                        }
                        inputTimer += Time.deltaTime;
                        prevTouchPos = touch.position;
                    }
                break;
                case TouchPhase.Ended:
                    inputSwitch.Invoke((touchDir ? 0 : 1), numStrings);

                    validTouch = false;
                    inputTimer = 0;
                    numStrings = 0;
                break;
            }
        }
        else{
            validTouch = false;
        }
    }
}
