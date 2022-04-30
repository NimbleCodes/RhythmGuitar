using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    float MaxDis = 999f;
    Vector3 MousePos;
    Camera Camera;

    void Start(){
        Camera = GetComponent<Camera>();
    }

    void Update(){
        if(Input.GetMouseButton(0)){
        RayAll();
    }
        
    }

    void Ray(){
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, MaxDis);
        hit.transform.GetComponent<SpriteRenderer>().color = Color.blue;
    }

    void RayAll(){
        MousePos = Input.mousePosition;
        MousePos = Camera.ScreenToWorldPoint(MousePos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(MousePos, transform.right, MaxDis);

        for(int i=0; i < hits.Length; i++)
            {
                
                RaycastHit2D hit = hits[i];
                SpriteRenderer ChangeColor = hit.transform.GetComponent<SpriteRenderer>();

                if(ChangeColor){
                    hit.transform.GetComponent<SpriteRenderer>().color = Color.blue;
                }
            }
    }
}
