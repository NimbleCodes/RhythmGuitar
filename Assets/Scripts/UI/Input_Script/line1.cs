using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using kgh.Signals;
public class line1 : MonoBehaviour
{
    [SerializeField] GameObject Input_line1;
    Camera Camera;
    public LayerMask LayerMask;
    public static line1 instance;
    public Vector3 mousePos;
    public float Distance;//mouse drag distance
    float MaxDis = 999f;//raycast ray max distance
    public Vector2 TouchPos;Dictionary<int, Vector2> touchStartPos;
    private Vector2 Direction;
    public bool swiped = false;
    public int dragDirection;
    public event Action<int> userInputEvent;
    public float MinMovement;
    public float[] diagonals = { 45, 135, 225, 315 };
    public float windowInDeg = 20f;
    public int lineCount = 0;
    public int SwipeEndCount =0;
    Switch _switch;
    void Start(){
        Camera = GetComponent<Camera>();
    }
    void Update(){
        processMobileInput();//유니티 모바일일때
        ProcessInput();//PC
    }
    void Awake(){
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        MinMovement = Mathf.Max(screenSize.x, screenSize.y) / 70f;
        
        instance = this;
    }
    public void ProcessInput()// 유저의 드래그 방향을 알기 위함.
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            mousePos = Input.mousePosition;
            swiped = false;
        }
        else if (Input.GetMouseButton(0) == true)
        {
            Direction = (Input.mousePosition - mousePos).normalized;
            Distance = Vector2.Distance(Input.mousePosition, mousePos);
            RayAll();
        }
        else if (Input.GetMouseButtonUp(0) == true)
        {
            float clockwiseDeg = 360f - Quaternion.FromToRotation(Vector2.up, Direction).eulerAngles.z;
            dragDirection = checkDirection_mouse(clockwiseDeg);
            if(userInputEvent!=null) userInputEvent.Invoke(lineCount);
                if(lineCount == 1){
                    PlayAnimation.instance.Stroke1();
                }if(lineCount == 2){
                    PlayAnimation.instance.Stroke2();
                }if(lineCount == 3){
                    PlayAnimation.instance.Stroke3();
                }if(lineCount == 4){
                    PlayAnimation.instance.Stroke1();
            }
            enalbeCollider();
            swiped = true;
            SwipeEndCount = lineCount;
            lineCount = 0;
        }
    }
    void processMobileInput()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {//터치시작 좌표 저장
                TouchPos = new Vector2(t.position.x, t.position.y);
                swiped = false;
            }
            else if (t.phase == TouchPhase.Moved)
            {
                Vector2 currentTouchPos = new Vector2(t.position.x, t.position.y);
                Direction = (currentTouchPos - TouchPos).normalized;
                Distance = Vector2.Distance(currentTouchPos, TouchPos);
                
                RayAll();
            }
            else if (t.phase == TouchPhase.Ended)
            {
                //Vector2 endPos = Camera.main.ScreenToWorldPoint(t.position);
                //Vector2 dir = (endPos - touchStartPos[t.fingerId]).normalized;
                float clockwiseDeg = 360f - Quaternion.FromToRotation(Vector2.up, Direction).eulerAngles.z;
                int dirCode = checkDirection_mouse(clockwiseDeg);
                touchStartPos.Remove(t.fingerId);
                if (userInputEvent != null) userInputEvent.Invoke(dirCode);
                
                if(lineCount == 1){
                    PlayAnimation.instance.Stroke1();
                }if(lineCount == 2){
                    PlayAnimation.instance.Stroke2();
                }if(lineCount == 3){
                    PlayAnimation.instance.Stroke3();
                }if(lineCount == 4){
                    PlayAnimation.instance.Stroke1();
                
                swiped = true;
                SwipeEndCount = lineCount;
                lineCount = 0;
                _switch.Invoke(SwipeEndCount);
            }
            }
        }
    }
    public int checkDirection_mouse(float Deg)
    {
        if (Distance < MinMovement)
        {
            return 0;
        }
        else if ((Deg > diagonals[3] + windowInDeg && Deg <= 360) ||
                   (Deg <= diagonals[0] - windowInDeg && Deg >= 0))
        {
            return 1;
        }
        else if (Deg > diagonals[0] - windowInDeg && Deg <= diagonals[0] + windowInDeg)
        {
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

    void RayAll(){
        #if UNITY_EDITOR_WIN
            mousePos = Input.mousePosition;
            mousePos = Camera.ScreenToWorldPoint(mousePos);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, transform.right, MaxDis,LayerMask);
        #endif

        #if UNITY_ANDROID
            TouchPos = Camera.ScreenToWorldPoint(TouchPos);
            RaycastHit2D[] hits = Physics2D.RaycastAll(TouchPos,transform.right,MaxDis);
        #endif

        for(int i=0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                BoxCollider2D Linehit = hit.transform.GetComponent<BoxCollider2D>();

                if(Linehit){
                    Linehit.enabled =false;
                    lineCount++;
                }
            }
    }

    void enalbeCollider(){
        GameObject[] lines;
        lines = GameObject.FindGameObjectsWithTag("Line");
        foreach(GameObject Line in lines){
            BoxCollider2D collider = Line.transform.GetComponent<BoxCollider2D>();
            collider.enabled = true;
        }
    }
}
