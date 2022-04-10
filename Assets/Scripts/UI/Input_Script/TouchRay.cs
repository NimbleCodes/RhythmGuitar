using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using kgh.Signals;
public class TouchRay : MonoBehaviour
{
    public LayerMask layermask;
    Camera camera;
    public static TouchRay instance;
    public int lineCount= 0;
    private float MaxDis = 100f;
    public float Distance;//mouse drag distance
    public float MinMovement;
    public float[] diagonals = { 45, 135, 225, 315 };
    public float windowInDeg = 20f;
    public int dragDirection; //계산된 방향의 int값 1:UP, 2:DOWN
    public int SwipeEndCount;
    Vector2 startTouchPos, Direction;
    Switch _switch;
    void Awake(){
        camera = GetComponent<Camera>();
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        MinMovement = Mathf.Max(screenSize.x, screenSize.y) / 70f;
        _switch = GameManager.instance.sigs.Register("OnMouseBehavior", typeof(Action<int,int>));
        instance = this;
    }
    void Update()
    {
        touchControl();
    }
    void enalbeCollider(){
        GameObject[] lines;
        lines = GameObject.FindGameObjectsWithTag("Line");
        foreach(GameObject Line in lines){
            BoxCollider2D collider = Line.transform.GetComponent<BoxCollider2D>();
            collider.enabled = true;
        }
    }

    public int checkDirection_mouse(float Deg)
    {
        if (Distance < MinMovement)
        {
            //Debug.Log("Touch");
            return 0;
        }
        else if ((Deg > diagonals[3] + windowInDeg && Deg <= 360) ||
                   (Deg <= diagonals[0] - windowInDeg && Deg >= 0))
        {
            //Debug.Log("UP");
            return 1;
        }
        else if (Deg > diagonals[0] - windowInDeg && Deg <= diagonals[0] + windowInDeg)
        {
            //Debug.Log("UP_RIGHT");
            return 1;
        }
        else if (Deg > diagonals[1] - windowInDeg && Deg <= diagonals[1] + windowInDeg)
        {
            //Debug.Log("DOWN_RIGHT");
            return 2;
        }
        else if (Deg > diagonals[1] + windowInDeg && Deg <= diagonals[2] - windowInDeg)
        {
            //Debug.Log("DOWN");
            return 2;
        }
        else if (Deg > diagonals[2] - windowInDeg && Deg <= diagonals[2] + windowInDeg)
        {
            //Debug.Log("DOWN_LEFT");
            return 2;
        }
        else
        {
            return 1;
            //Debug.Log("UP_LEFT");
        }
    }
    void touchControl(){
        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos;
            Ray ray;
            RaycastHit2D[] hits;
            

            switch(touch.phase){
                case TouchPhase.Began:
                    Debug.Log("Touch Started");

                    Vector2 touchPosVector = new Vector2(touch.position.x,touch.position.y);
                    touchPos = Camera.main.ScreenToWorldPoint(touchPosVector);
                    ray = Camera.main.ScreenPointToRay(touchPosVector);
                    startTouchPos = Camera.main.ScreenToWorldPoint(touchPosVector);
                    break;
                
                case TouchPhase.Moved:
                    Debug.Log("Touch Moved");
                    touchPosVector = new Vector2(touch.position.x,touch.position.y);
                    touchPos = Camera.main.ScreenToWorldPoint(touchPosVector);
                    ray = Camera.main.ScreenPointToRay(touchPosVector);
                    hits = Physics2D.RaycastAll(touchPos,transform.position,MaxDis,layermask);
                    Scene_1.instance.Print(touchPos.x.ToString() + ", " + touchPos.y.ToString());
                    Direction = (touchPos - startTouchPos).normalized;
                    Distance = Vector2.Distance(touchPos, startTouchPos);

                    for(int i=0; i < hits.Length; i++)
                    {
                        RaycastHit2D hit = hits[i];
                        BoxCollider2D Linehit = hit.transform.GetComponent<BoxCollider2D>();

                        if(Linehit){
                            Linehit.enabled =false;
                            if(Linehit.name == "String_skel"){
                                StringAnimation.instance.Shake();
                            }
                            if(Linehit.name == "String_skel1"){
                                StringAnimation1.instance.Shake();
                            }
                            if(Linehit.name == "String_skel2"){
                                StringAnimation2.instance.Shake();
                            }
                            if(Linehit.name == "String_skel3"){
                                StringAnimation3.instance.Shake();
                            }
                            lineCount++;
                        }
                    }
                break;

                case TouchPhase.Ended:
                    float clockwiseDeg = 360f - Quaternion.FromToRotation(Vector2.up, Direction).eulerAngles.z;
                    dragDirection = checkDirection_mouse(clockwiseDeg);
                    _switch.Invoke(dragDirection, lineCount);
                    SwipeEndCount = lineCount;
                    lineCount = 0;
                    enalbeCollider();
                break;
            }

        }

    }
}
