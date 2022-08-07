using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteSolution : MonoBehaviour
{
    public Collider2D collider1, collider2;
    float StartTime,EndTime,col_distance;

    GameObject longNote;

    void cal_Notesize(){
        col_distance = Vector2.Distance(collider1.transform.position,collider2.transform.position);
        longNote.transform.localScale += new Vector3(col_distance,0);                               //계산한 collider들의 거리만큼 롱노트 x크기 조절

    }

    void longNoteInput(){
        
    }
}
