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
    List<(int lane, float timing,float timing2, int noteType)> notesSingleList;

    List<GameObject> arrowPool;
    List<GameObject> longArrows;
    public GameObject arrowPoolParent;
    public GameObject arrowPrefab;
    public GameObject longArrowPrefab;
    Sprite up, down;
    List<Sprite> longUp;
    List<Sprite> longDown;
    Sprite[] laneNum;
    public GameObject lane;
    public GameObject InputArrow;

    float timer;
    int numNotesInLane = 0;
    int visibleStart, visibleEnd;
    int wrong = 0;
    int right = 0;

    NoteHitFX fx;
    GameObject firstNote;

    int longNoteCheck = 0;

    enum NoteType{
        Touch = 1,
        Press = 2
    }

    private void Awake(){
        scene1Man = FindObjectOfType<Scene_1_Manager>();
        arrowPool = new List<GameObject>();
        longArrows = new List<GameObject>();
        notesSingleList = new List<(int lane, float timing, float timing2, int noteType)>();
        timer = -scene1Man.delay;

        longUp = new List<Sprite>();
        longDown = new List<Sprite>();

        up      = Resources.Load<Sprite>("ui_image/up");
        down    = Resources.Load<Sprite>("ui_image/down");
        longUp.Add(Resources.Load<Sprite>("longNote/longup"));
        longUp.Add(Resources.Load<Sprite>("longNote/longup2"));
        longUp.Add(Resources.Load<Sprite>("longNote/longup3"));
        longDown.Add(Resources.Load<Sprite>("longNote/longDown"));
        longDown.Add(Resources.Load<Sprite>("longNote/longDown2"));
        longDown.Add(Resources.Load<Sprite>("longNote/longDown3"));
        laneNum = new Sprite[9];
        laneNum[1] = Resources.Load<Sprite>("ui_image/1_green");
        laneNum[2] = Resources.Load<Sprite>("ui_image/2_green");
        laneNum[3] = Resources.Load<Sprite>("ui_image/3_green");
        laneNum[4] = Resources.Load<Sprite>("ui_image/4_green");
        laneNum[5] = Resources.Load<Sprite>("ui_image/1_red");
        laneNum[6] = Resources.Load<Sprite>("ui_image/2_red");
        laneNum[7] = Resources.Load<Sprite>("ui_image/3_red");
        laneNum[8] = Resources.Load<Sprite>("ui_image/4_red");     

        fx = FindObjectOfType<NoteHitFX>();   
    }
    private void Start(){
        GameManager.instance.sigs.Subscribe("OnMouseBehavior", this, "OnMouseBehavior");
        GameManager.instance.sigs.Subscribe("game_over", this, "GameOver");
        noteData = scene1Man.noteData;
        while(!AllLanesEmpty()){
            float minVal = float.MaxValue;
            float minVal2 = float.MaxValue;
            int minLane = -1;
            int minNoteType = -1;
            for(int i = 0; i < noteData.notes.Count; i++){
                if(noteData.notes[i].Count > 0){
                    float time = noteData.notes[i][0];
                    float time2 = noteData.notes[i][1];
                    int noteType = (int)noteData.notes[i][2];
                    if(time < minVal){
                        minVal = time;
                        minVal2 = time2;
                        minNoteType = noteType;
                        minLane = i;
                    }
                }
            }
            notesSingleList.Add((minLane, minVal, minVal2, minNoteType));
            noteData.notes[minLane].RemoveAt(0);
            noteData.notes[minLane].RemoveAt(0);
            noteData.notes[minLane].RemoveAt(0);
        }
    }
    private void Update(){
        while(notesSingleList.Count > 0 && notesSingleList[0].timing < timer){
            if(notesSingleList[0].noteType == (int)NoteType.Touch){
                notesSingleList.RemoveAt(0);
                fx.NoteHitFXStart(new Vector2(firstNote.GetComponent<RectTransform>().position.x, firstNote.GetComponent<RectTransform>().position.y));
                firstNote = null;
            }
            else if(notesSingleList[0].noteType == (int)NoteType.Press){
                if(notesSingleList[0].timing2 < timer){
                    notesSingleList.RemoveAt(0);
                    Image temp = firstNote.GetComponent<Image>();
                    Image[] tempChild = firstNote.GetComponentsInChildren<Image>();
                    temp.color = Color.white;
                    foreach(Image img in tempChild){
                        img.color = Color.white;
                    }
                    firstNote = null;
                }
                else{
                    Image temp = firstNote.GetComponent<Image>();
                    Image[] tempChild = firstNote.GetComponentsInChildren<Image>();
                    temp.color = new Color(0.5f,0.5f,0.5f,0.5f);
                    foreach(Image img in tempChild){
                        img.color = new Color(0.5f,0.5f,0.5f,0.5f);
                    }
                }
                break;
            }
        }
        int numTouchNotes = 0;
        int numPressNotes = 0;
        float laneWidth = lane.GetComponent<RectTransform>().rect.width - arrowPrefab.GetComponent<RectTransform>().rect.width / 2;
        for(int i = 0; i < notesSingleList.Count; i++){
            if(notesSingleList[i].noteType == (int)NoteType.Touch && notesSingleList[i].timing > timer + visibleAreaSize)
                break;
            if(notesSingleList[i].noteType == (int)NoteType.Press && notesSingleList[i].timing > timer + visibleAreaSize)
                break;
            GameObject arrowObj = null;
            if(notesSingleList[i].noteType == (int)NoteType.Touch){
                if(numTouchNotes < arrowPool.Count){
                    arrowObj = arrowPool[numTouchNotes];
                }
                else{
                    arrowObj = Instantiate(arrowPrefab);
                    arrowObj.transform.SetParent(arrowPoolParent.transform);
                    arrowPool.Add(arrowObj);
                }
                arrowObj.GetComponent<Image>().sprite = (notesSingleList[i].lane <= 4) ? up : down;
                arrowObj.transform.GetChild(0).GetComponent<Image>().sprite = laneNum[notesSingleList[i].lane + 1];
                arrowObj.GetComponent<RectTransform>().position = new Vector3(
                    arrowPrefab.GetComponent<RectTransform>().rect.width / 2 + laneWidth * ((notesSingleList[i].timing - timer) / visibleAreaSize),
                    lane.GetComponent<RectTransform>().position.y,
                    0
                );
                numTouchNotes++;
            }
            else if(notesSingleList[i].noteType == (int)NoteType.Press){
                if(numPressNotes < longArrows.Count){
                    arrowObj = longArrows[numPressNotes];
                }
                else{
                    arrowObj = Instantiate(longArrowPrefab);
                    arrowObj.transform.SetParent(arrowPoolParent.transform);
                    longArrows.Add(arrowObj);
                }
                GameObject longArrow1 = arrowObj.transform.GetChild(0).gameObject;
                GameObject longArrow2 = arrowObj.transform.GetChild(1).gameObject;
                GameObject lanenum = arrowObj.transform.GetChild(2).gameObject;

                arrowObj.GetComponent<Image>().sprite = (notesSingleList[i].lane <= 4) ? longUp[0] : longDown[0];
                longArrow1.GetComponent<Image>().sprite = (notesSingleList[i].lane <= 4) ? longUp[1] : longDown[1];
                longArrow2.GetComponent<Image>().sprite = (notesSingleList[i].lane <= 4) ? longUp[2] : longDown[2];
                lanenum.GetComponent<Image>().sprite = laneNum[notesSingleList[i].lane + 1];

                longArrow2.GetComponent<RectTransform>().localPosition = new Vector3((notesSingleList[i].timing2 - notesSingleList[i].timing) * laneWidth / visibleAreaSize, 0, 0);
                longArrow1.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    ((notesSingleList[i].timing2 - notesSingleList[i].timing) * laneWidth / visibleAreaSize) - 84.375f,
                    112.5f
                );
                longArrow1.GetComponent<RectTransform>().localPosition = new Vector3(
                    (notesSingleList[i].timing2 - notesSingleList[i].timing) * laneWidth /  (2 * visibleAreaSize),
                    0,
                    0
                );
                arrowObj.GetComponent<RectTransform>().position = new Vector3(
                    arrowPrefab.GetComponent<RectTransform>().rect.width / 2 + laneWidth * ((notesSingleList[i].timing - timer) / visibleAreaSize),
                    lane.GetComponent<RectTransform>().position.y,
                    0
                );
                numPressNotes++;
            }
            if(firstNote == null)
                firstNote = arrowObj;
        }
        for(int i = numTouchNotes; i < arrowPool.Count; i++){
            arrowPool[i].GetComponent<RectTransform>().localPosition = new Vector3();
        }
        for(int i = numPressNotes; i < longArrows.Count; i++){
            longArrows[i].GetComponent<RectTransform>().localPosition = new Vector3();
            GameObject longArrow1 = longArrows[i].transform.GetChild(0).gameObject;
            GameObject longArrow2 = longArrows[i].transform.GetChild(1).gameObject;
            longArrow2.GetComponent<RectTransform>().localPosition = new Vector3();
            longArrow1.GetComponent<RectTransform>().sizeDelta = new Vector2(84.375f, 112.5f);
            longArrow1.GetComponent<RectTransform>().localPosition = new Vector3();
        }
        timer += Time.deltaTime;
    }
    private void OnMouseBehavior(int direction, int lineCount){
        Debug.Log(direction + ", " + lineCount);
        if(numNotesInLane <= 0)
                return;
        //1->up stroke 2->down stroke
        if(notesSingleList[0].noteType == (int)NoteType.Touch){
            float diff = notesSingleList[0].timing - timer;
            int ansDir = (notesSingleList[0].lane <= 4) ? 1 : 2;
            int ansLC = (ansDir == 1) ? notesSingleList[0].lane : notesSingleList[0].lane - 4;
            // Debug.Log(diff + " : (" + ansDir + ", " + ansLC + ") : (" + direction + ", " + lineCount + ")");

            //diff 가 노트 이내 일 경우에만 입력이 되도록 수정
            notesSingleList.RemoveAt(0);
            numNotesInLane--;

            if(ansDir != direction || ansLC != lineCount)
                wrong++;
            float val = arrowPrefab.GetComponent<RectTransform>().rect.width * 2 / (lane.GetComponent<RectTransform>().rect.width - arrowPrefab.GetComponent<RectTransform>().rect.width / 2);
            if(diff < val){
                fx.NoteHitFXStart(new Vector2(firstNote.GetComponent<RectTransform>().localPosition.x, firstNote.GetComponent<RectTransform>().localPosition.y));
                right++;
            }
        }
        else{
            if(longNoteCheck == 0){
                float diff = notesSingleList[0].timing - timer;
                int ansDir = (notesSingleList[0].lane <= 4) ? 1 : 2;
                int ansLC = (ansDir == 1) ? notesSingleList[0].lane : notesSingleList[0].lane - 4;
                if(ansDir != direction || ansLC != lineCount)
                    wrong++;
            }

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
