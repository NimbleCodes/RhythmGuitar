using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Evaluator : MonoBehaviour
{
    const float visibleAreaSize = 2.0f;

    Scene_1_Manager scene1Man;
    NoteData noteData;
    List<(int lane, float timing,float timing2)> notesSingleList;

    List<GameObject> arrowPool;
    public GameObject arrowPoolParent;
    public GameObject arrowPrefab;
    Sprite up, down;
    Sprite[] laneNum;
    public GameObject lane;

    float timer;
    int numNotesInLane = 0;
    int visibleStart, visibleEnd;
    int wrong = 0;
    int right = 0;

    private void Awake(){
        scene1Man = FindObjectOfType<Scene_1_Manager>();
        arrowPool = new List<GameObject>();
        notesSingleList = new List<(int lane, float timing, float timing2)>();
        timer = -scene1Man.delay;

        up      = Resources.Load<Sprite>("ui_image/up");
        down    = Resources.Load<Sprite>("ui_image/down");
        laneNum = new Sprite[9];
        laneNum[1] = Resources.Load<Sprite>("ui_image/1_green");
        laneNum[2] = Resources.Load<Sprite>("ui_image/2_green");
        laneNum[3] = Resources.Load<Sprite>("ui_image/3_green");
        laneNum[4] = Resources.Load<Sprite>("ui_image/4_green");
        laneNum[5] = Resources.Load<Sprite>("ui_image/1_red");
        laneNum[6] = Resources.Load<Sprite>("ui_image/2_red");
        laneNum[7] = Resources.Load<Sprite>("ui_image/3_red");
        laneNum[8] = Resources.Load<Sprite>("ui_image/4_red");        
    }
    private void Start(){
        GameManager.instance.sigs.Subscribe("OnMouseBehavior", this, "OnMouseBehavior");
        GameManager.instance.sigs.Subscribe("game_over", this, "GameOver");
        
        noteData = scene1Man.noteData;
        
        int cnt = 0;
        while(!AllLanesEmpty()){
            float minVal = float.MaxValue;
            float minVal2 = float.MaxValue;
            int minLane = -1;
            for(int i = 0; i < noteData.notes.Count; i++){
                if(noteData.notes[i].Count > 0){
                    if(noteData.notes[i][0] < minVal){
                        minVal = noteData.notes[i][0];
                        minLane = i;
                    }
                }
            }
            notesSingleList.Add((minLane, minVal, minVal2));
            // Debug.Log(notesSingleList[cnt].lane + ", " + notesSingleList[cnt].timing);
            cnt++;
            noteData.notes[minLane].RemoveAt(0);
        }
    }
    private void Update(){
        while(notesSingleList.Count > 0 && notesSingleList[0].timing < timer){
            notesSingleList.RemoveAt(0);
            wrong++;
        }
        
        int ind = 0;
        numNotesInLane = 0;
        while(notesSingleList.Count > ind && notesSingleList[ind].timing <= timer + visibleAreaSize){
            GameObject arrowObj;
            if(arrowPool.Count < ind + 1){
                arrowObj = Instantiate(arrowPrefab);
                arrowObj.transform.SetParent(arrowPoolParent.transform);
                arrowPool.Add(arrowObj);
            }
            else{
                arrowObj = arrowPool[ind];
            }
            arrowObj.GetComponent<Image>().sprite = (notesSingleList[ind].lane <= 4) ? up : down;
            arrowObj.transform.GetChild(0).GetComponent<Image>().sprite = laneNum[notesSingleList[ind].lane];
            //노트 포지션
            arrowObj.GetComponent<RectTransform>().position = new Vector3(
                ((lane.GetComponent<RectTransform>().rect.width - arrowPrefab.GetComponent<RectTransform>().rect.width / 2) * (notesSingleList[ind].timing - timer) / visibleAreaSize) + arrowPrefab.GetComponent<RectTransform>().rect.width / 2,
                lane.GetComponent<RectTransform>().position.y,
                0 //롱노트 계산시 현 계산식 2번필요
            );

            ind++;
            numNotesInLane++;
        }
        while(ind < arrowPool.Count){
            arrowPool[ind].GetComponent<RectTransform>().localPosition = new Vector3();
            ind++;
        }
        
        timer += Time.deltaTime;
    }
    private void OnMouseBehavior(int direction, int lineCount){
        //1->up stroke 2->down stroke
        if(numNotesInLane <= 0)
            return;
        
        float diff = notesSingleList[0].timing - timer;
        int ansDir = (notesSingleList[0].lane <= 4) ? 1 : 2;
        int ansLC = (ansDir == 1) ? notesSingleList[0].lane : notesSingleList[0].lane - 4;
        Debug.Log(diff + " : (" + ansDir + ", " + ansLC + ") : (" + direction + ", " + lineCount + ")");

        //diff 가 노트 이내 일 경우에만 입력이 되도록 수정
        notesSingleList.RemoveAt(0);
        numNotesInLane--;

        if(ansDir != direction || ansLC != lineCount)
            wrong++;
        float val = arrowPrefab.GetComponent<RectTransform>().rect.width * 2 / (lane.GetComponent<RectTransform>().rect.width - arrowPrefab.GetComponent<RectTransform>().rect.width / 2);
        if(diff < val){
            Debug.Log("Hit!");
            right++;
        }
    }
    void GameOver(){
        Debug.Log("Game Over");
    }
    bool AllLanesEmpty(){
        foreach(var lane in noteData.notes){
            if(lane.Count > 0){
                return false;
            }
        }
        return true;
    }

    void longNotePositioner(){   //롱노트 위치 + 사이즈 조정
        int ind = 0;
        numNotesInLane = 0;
        while(notesSingleList.Count > ind && notesSingleList[ind].timing <= timer + visibleAreaSize){
            GameObject arrowObj;
            Vector3 endPoint;
            Vector3 median;
            float longNoteWidth = 0f;
            if(arrowPool.Count < ind + 1){
                arrowObj = Instantiate(arrowPrefab);
                arrowObj.transform.SetParent(arrowPoolParent.transform);
                arrowPool.Add(arrowObj);
            }
            else{
                arrowObj = arrowPool[ind];
            }
            arrowObj.GetComponent<Image>().sprite = (notesSingleList[ind].lane <= 4) ? up : down;
            arrowObj.transform.GetChild(0).GetComponent<Image>().sprite = laneNum[notesSingleList[ind].lane];
            //노트 포지션
            arrowObj.GetComponent<RectTransform>().position = new Vector3( //롱노트 시작지점 좌표
                ((lane.GetComponent<RectTransform>().rect.width - arrowPrefab.GetComponent<RectTransform>().rect.width / 2) * (notesSingleList[ind].timing - timer) / visibleAreaSize) + arrowPrefab.GetComponent<RectTransform>().rect.width / 2,
                lane.GetComponent<RectTransform>().position.y,
                0 //롱노트 계산시 현 계산식 2번필요
            );
            endPoint = new Vector3(// 롱노트 끝지점 좌표
                ((lane.GetComponent<RectTransform>().rect.width - arrowPrefab.GetComponent<RectTransform>().rect.width / 2) * (notesSingleList[ind].timing2 - timer) / visibleAreaSize) + arrowPrefab.GetComponent<RectTransform>().rect.width / 2,
                lane.GetComponent<RectTransform>().position.y,
                0
            );
            longNoteWidth = Vector3.Distance(arrowObj.transform.position, endPoint); // 기본적으로 y값, z값이 고정이기 때문에 Distance로 넓이 계산가능
            median = Vector3.Lerp(arrowObj.transform.position, endPoint, 0.5f);
            arrowObj.transform.position = median;
            arrowObj.GetComponent<RectTransform>().sizeDelta = new Vector2(longNoteWidth, arrowObj.GetComponent<RectTransform>().rect.height);//노트 크기 설정
            ind++;
            numNotesInLane++;
        }
    }
}
