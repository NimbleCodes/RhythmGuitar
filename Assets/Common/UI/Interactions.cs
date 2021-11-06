#if UNITY_ENGINE

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace kgh.UI{
    public static class Interactions{
        const int numMouseBtns = 3;
        public static void MouseInteractions(
            VisualElement                   target,
            Action<int, Vector2>            click,
            Action<int, Vector2>            dragStart,
            Action<int, Vector2, Vector2>   drag,
            Action<int, Vector2, Vector2>   dragEnd
        ){
            Vector2[] mouseDownPos = new Vector2[numMouseBtns];
            Vector2[] diff = new Vector2[numMouseBtns];
            for(int i = 0; i < numMouseBtns; i++){
                mouseDownPos[i] = new Vector2(-1, -1);
                diff[i] = new Vector2(0, 0);
            }
            
            target.RegisterCallback<MouseDownEvent>((e)=>{
                mouseDownPos[e.button].x = e.localMousePosition.x;
                mouseDownPos[e.button].y = e.localMousePosition.y;
            });
            target.RegisterCallback<MouseMoveEvent>((e)=>{
                for(int i = 0; i < numMouseBtns; i++){
                    if(mouseDownPos[i].x != -1){
                        float xDiff = e.localMousePosition.x - mouseDownPos[i].x;
                        float yDiff = e.localMousePosition.y - mouseDownPos[i].y;
                        if(xDiff != 0 || yDiff != 0){
                            if(diff[i].x == 0 && diff[i].y == 0){
                                if(dragStart != null){
                                    dragStart.Invoke(i, mouseDownPos[i]);
                                }
                            }
                            else{
                                if(drag != null){
                                    drag.Invoke(i, mouseDownPos[i], diff[i]);
                                }
                            }
                        }
                        diff[i].x = xDiff;
                        diff[i].y = yDiff;
                    }
                }
            });
            target.RegisterCallback<MouseUpEvent>((e)=>{
                for(int i = 0; i < numMouseBtns; i++){
                    if(mouseDownPos[i].x != -1){
                        if(diff[i].x == 0 && diff[i].y == 0){
                            if(click != null){
                                click.Invoke(i, mouseDownPos[i]);
                            }
                        }
                        else{
                            if(dragEnd != null){
                                dragEnd.Invoke(i, mouseDownPos[i], diff[i]);
                            }
                        }
                        mouseDownPos[i].x = -1;
                        mouseDownPos[i].y = -1;
                        diff[i].x = 0;
                        diff[i].y = 0;
                    }
                }
            });
            target.RegisterCallback<MouseLeaveEvent>((e)=>{
                for(int i = 0; i < numMouseBtns; i++){
                    if(diff[i].x != 0 && diff[i].y != 0){
                        if(dragEnd != null){
                            dragEnd.Invoke(i, mouseDownPos[i], diff[i]);
                        }
                    }
                    mouseDownPos[i].x = -1;
                    mouseDownPos[i].y = -1;
                    diff[i].x = 0;
                    diff[i].y = 0;
                }
            });
        }
    }
}

#endif