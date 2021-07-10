using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Drag_Manager : MonoBehaviour
{
  public static Drag_Manager instance;

    public event Action<int> userInputEvent;
    //public string inputStream;
    public Vector2 ScreenSize;
    public float MinMovement;
    public Vector2 Direction;
    public Vector2 TouchPos;
    public Vector3 mousePos;
    public float Distance;
    bool swiped = false;
    public float[] diagonals = { 45, 135, 225, 315 };
    public float windowInDeg = 20f;
    public bool isInputBlocked = false;
    Action<Vector2> SwipeDetect;
    Dictionary<int, Vector2> touchStartPos;

    private void Awake()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        MinMovement = Mathf.Max(screenSize.x, screenSize.y) / 14f;
        Debug.Log("MinSwipeDist:" + MinMovement);
        touchStartPos = new Dictionary<int, Vector2>();

        instance = this;
    }
    private void Update()
    {
        processMobileInput();//유니티 모바일일때
        ProcessInput();//PC
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
            return 2;
        }
        else if (Deg > diagonals[0] + windowInDeg && Deg <= diagonals[1] - windowInDeg)
        {
            //Debug.Log("RIGHT");
            return 3;
        }
        else if (Deg > diagonals[1] - windowInDeg && Deg <= diagonals[1] + windowInDeg)
        {
            //Debug.Log("DOWN_RIGHT");
            return 4;
        }
        else if (Deg > diagonals[1] + windowInDeg && Deg <= diagonals[2] - windowInDeg)
        {
            //Debug.Log("DOWN");
            return 5;
        }
        else if (Deg > diagonals[2] - windowInDeg && Deg <= diagonals[2] + windowInDeg)
        {
            //Debug.Log("DOWN_LEFT");
            return 6;
        }
        else if (Deg > diagonals[2] + windowInDeg && Deg <= diagonals[3] - windowInDeg)
        {
            //Debug.Log("LEFT");
            return 7;
        }
        else
        {
            return 8;
            //Debug.Log("UP_LEFT");
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
                swiped = true;
            }
        }
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
            bool swipeDetected = checkSwipe(mousePos, Input.mousePosition);
            Direction = (Input.mousePosition - mousePos).normalized;
            Distance = Vector2.Distance(Input.mousePosition, mousePos);
            if (swipeDetected)
            {
                onSwipeDetected(Direction);
            }
        }
        else if (Input.GetMouseButtonUp(0) == true)
        {
            float clockwiseDeg = 360f - Quaternion.FromToRotation(Vector2.up, Direction).eulerAngles.z;
            int temp = checkDirection_mouse(clockwiseDeg);
            if(userInputEvent!=null) userInputEvent.Invoke(temp);
            //inputStream += temp.ToString() + ",";
            swiped = true;
        }
    }
    public bool checkSwipe(Vector3 downPos, Vector3 currentPos)
    {
        Vector2 currentSwipe = currentPos - downPos;

        if (swiped == true)
        {//터치됨, 스와프는 완료된게 아닌상태
            return false;
        }
        if (isInputBlocked == true)
        {//무언가가 인풋을 막고있을때
            return false;
        }
        if (currentSwipe.magnitude >= MinMovement)
        {
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
        swiped = true;
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
