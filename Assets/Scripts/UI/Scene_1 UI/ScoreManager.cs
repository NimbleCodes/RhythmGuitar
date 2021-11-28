using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kgh.Signals;
public class ScoreManager : MonoBehaviour
{
    public NoteData noteData;
    private int MaxScore = 350000;
    private int MinScore;

    void MinScoreCalc(){
        print(noteData.notes.Count);
    }
}
