using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kgh.Signals;
public class ScoreManager : MonoBehaviour
{
    public NoteData noteData;
    private int MaxScore = 350000;
    private int Score=0;
    public int judgment;//(1=perfect, 2 = good, 3 = miss)

    void MinScoreCalc(){
        print(noteData.notes.Count);
    }

    void ScoreManage(int judgment){
        if(judgment == 1){
            Score += 100;
        }if(judgment == 2){
            Score += 70;
        }if(judgment == 3){
            HealthManager.Instance.minusHealth();
        }

    }
}
