using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using kgh.Signals;
public class line1 : MonoBehaviour
{
    [SerializeField] GameObject Input_line1;
    [SerializeField] private Camera mainCamera;
    public static line1 instance;
    public Vector3 mousePos;
    public float Distance;
    public Vector2 TouchPos;Dictionary<int, Vector2> touchStartPos;
    public Vector2 Direction;
    public bool swiped = false;
    public bool swipping = false;
    public bool swipeDetected = false;
    public event Action<int> userInputEvent;
    public bool isInputBlocked = false;
    public float MinMovement;
    Action<Vector2> SwipeDetect;
    public float[] diagonals = { 45, 135, 225, 315 };
    public float windowInDeg = 20f;
    public int lineCount = 0;
    public int SwipeEndCount =0;
    Switch _switch;
    Ray ray;
    void Update(){
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        processMobileInput();//유니티 모바일일때
        ProcessInput();//PC
    }
    void Awake(){
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        MinMovement = Mathf.Max(screenSize.x, screenSize.y) / 70f;
        // Debug.Log("MinSwipeDist:" + MinMovement);
        
        _switch = GameManager.instance.sigs.Register("OnMouseBehavior" , typeof(Action<int>));//이벤트 발생시, 몇라인인지 int 값 반환
        
        instance = this;
    }
    public void ProcessInput()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            mousePos = Input.mousePosition;
            swiped = false;
        }
        else if (Input.GetMouseButton(0) == true)
        {
            swipeDetected = checkSwipe(mousePos, Input.mousePosition);
            Direction = (Input.mousePosition - mousePos).normalized;
            Distance = Vector2.Distance(Input.mousePosition, mousePos);
            if (swipeDetected)
            {
                onSwipeDetected(Direction);
            }
            if(Physics.Raycast(ray, out RaycastHit raycastHit)){
                lineCount++;
            }
        }
        else if (Input.GetMouseButtonUp(0) == true)
        {
            float clockwiseDeg = 360f - Quaternion.FromToRotation(Vector2.up, Direction).eulerAngles.z;
            int temp = checkDirection_mouse(clockwiseDeg);
            if(userInputEvent!=null) userInputEvent.Invoke(temp);
            //inputStream += temp.ToString() + ",";
                if(lineCount == 1){
                    PlayAnimation.instance.Stroke1();
                }if(lineCount == 2){
                    PlayAnimation.instance.Stroke2();
                }if(lineCount == 3){
                    PlayAnimation.instance.Stroke3();
                }if(lineCount == 4){
                    PlayAnimation.instance.Stroke1();
            }
            swiped = true;
            swipping = false;
            swipeDetected = false;
            SwipeEndCount = lineCount;
            lineCount = 0;
            // Debug.Log(SwipeEndCount);
            _switch.Invoke(SwipeEndCount);
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
                bool swipeDetected = checkSwipe(TouchPos, currentTouchPos);
                Direction = (currentTouchPos - TouchPos).normalized;
                Distance = Vector2.Distance(currentTouchPos, TouchPos);
                if (swipeDetected)
                {
                    onSwipeDetected(Direction);
                }
            }
            else if (t.phase == TouchPhase.Ended)
            {
                //Vector2 endPos = Camera.main.ScreenToWorldPoint(t.position);
                //Vector2 dir = (endPos - touchStartPos[t.fingerId]).normalized;
                float clockwiseDeg = 360f - Quaternion.FromToRotation(Vector2.up, Direction).eulerAngles.z;
                int dirCode = checkDirection_mouse(clockwiseDeg);
                touchStartPos.Remove(t.fingerId);
                if (userInputEvent != null) userInputEvent.Invoke(dirCode);
                //inputStream += dirCode.ToString() + ",";
                
                if(lineCount == 1){
                    PlayAnimation.instance.Stroke1();
                }if(lineCount == 2){
                    PlayAnimation.instance.Stroke2();
                }if(lineCount == 3){
                    PlayAnimation.instance.Stroke3();
                }if(lineCount == 4){
                    PlayAnimation.instance.Stroke1();
                
                swiped = true;
                swipping = false;
                swipeDetected = false;
                SwipeEndCount = lineCount;
                lineCount = 0;
                // Debug.Log(SwipeEndCount);
                _switch.Invoke(SwipeEndCount);
            }
            }
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
    public bool checkSwipe(Vector3 downPos, Vector3 currentPos)
    {
        Vector2 currentSwipe = currentPos - downPos;

        if (swiped == true)
        {//터치됨, 스와프는 완료된 상태
            //Debug.Log("false");
            swipping = false;
            return false;
        }
        if (isInputBlocked == true)
        {//무언가가 인풋을 막고있을때
            return false;
        }
        if (currentSwipe.magnitude >= MinMovement)
        {
            //Debug.Log("true");
            swipping = true;
            return true;
        }
        return false;
    }

    public void setOnSwipeDetected(Action<Vector2> onSwipeDetected)
    {
        SwipeDetect = onSwipeDetected;
    }
    public void onSwipeDetected(Vector2 swipeDirection)
    {
        //swiped = true;
        swipping = true;
        //SwipeDetect(swipeDirection);
    }
    public void blockInput()
    {
        isInputBlocked = true;
    }
    public void unBlockInput()
    {
        isInputBlocked = false;
    }
}
