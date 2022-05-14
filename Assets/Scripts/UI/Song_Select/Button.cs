using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    RectTransform rt;

    void Awake(){
        rt = GetComponent<RectTransform>();
    }
    void Update(){
        foreach(var touch in Input.touches){
            if(WithinBounds(touch.position)){
                SceneManager.LoadSceneAsync("VerticalTest");
            }
        }
    }
    bool WithinBounds(Vector3 touchPos){
        if(touchPos.x >= rt.position.x - rt.rect.width / 2 && touchPos.x <= rt.position.x + rt.rect.width / 2){
            if(touchPos.y >= rt.position.y - rt.rect.height / 2 && touchPos.y <= rt.position.y + rt.rect.height / 2){
                return true;
            }
        }
        return false;
    }
}
