using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class NoteEditor : myUI.Component{
    List<VisualElement> verticalIndicators;
    VisualElement verticalIndicatorCollection;
    DropDown dd;
    List<VisualElement> horizontalIndicators;
    VisualElement horizontalIndicatorCollection;
    public List<List<Note>> lanes;
    bool mouseDownStart = false;
    int noteInpState = 1;
    List<VisualElement> noteIndicators;
    VisualElement noteIndicatorCollection;
    List<(VisualElement, VisualElement, VisualElement)> lNoteIndicators;
    VisualElement clipEndIndicator;
    VisualElement cursor;
    bool focused = false;

    public class NoteEditorStates : States{
        NoteEditor component;
        AudioClip _audioClip;
        public AudioClip audioClip{
            set{
                _audioClip = value;
                dirty = true;
            }
            get{ return _audioClip; }
        }
        float _start, _size;
        public float start{
            set{
                if(audioClip != null){
                    if(audioClip.length < size){
                        _start = 0;
                    }
                    else{
                        _start = Mathf.Clamp(value, 0, audioClip.length - size);
                    }
                    dirty = true;
                }
            }
            get{ return _start; }
        }
        public float size{
            set{
                if(audioClip != null){
                    _size = Mathf.Clamp(value, 10, 60);
                    start = Mathf.Clamp(start, 0, audioClip.length - _size);
                    dirty = true;
                }
            }
            get{ return _size; }
        }
        int _bpm;
        public int bpm{
            set{
                _bpm = value;
                dirty = true;
            }
            get{ return _bpm; }
        }
        int _numLanes;
        public int numLanes{
            set{
                dirty = true;
                _numLanes = value;
            }
            get{
                return _numLanes;
            }
        }
        float _t0, _t1;     //노트 생성 시 Update 실행시키기 위해
        public float t0{
            set{
                if(audioClip != null && value <= audioClip.length){
                    dirty = true;
                    _t0 = value;
                }
            }
            get{ return _t0; }
        }
        public float t1{
            set{
                if(audioClip != null){
                    dirty = true;
                    _t1 = Mathf.Clamp(value, 0, audioClip.length);
                }
            }
            get{ return _t1; }
        }
        float _cursorPosition;
        public float cursorPosition{
            set{
                dirty = true;
                _cursorPosition = value;
            }
            get { return _cursorPosition; }
        }
        public NoteEditorStates(NoteEditor _component) : base(_component){
            component = _component;
            _start = 0;
            _size = 30;
            _bpm = 174;
            _numLanes = 0;
            _cursorPosition = 0;
        }
    }
    public NoteEditor(){
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Note Editor/NoteEditor.uxml");
        rootVisualElement = visualTreeAsset.CloneTree();
        rootVisualElement.style.width = Length.Percent(100);
        rootVisualElement.style.height = Length.Percent(100);
        rootVisualElement.name = "note-editor";

        states = new NoteEditorStates(this);
        verticalIndicators = new List<VisualElement>();
        verticalIndicatorCollection = new VisualElement();
        verticalIndicatorCollection.name = "vertical-indicator-collection";
        verticalIndicatorCollection.style.flexDirection = FlexDirection.Row;
        verticalIndicatorCollection.style.alignItems = Align.Center;
        verticalIndicatorCollection.style.width = Length.Percent(100);
        verticalIndicatorCollection.style.height = Length.Percent(100);
        verticalIndicatorCollection.style.position = Position.Absolute;
        
        VisualElement noteDisplay = rootVisualElement.Q<VisualElement>("note-display");
        noteDisplay.Add(verticalIndicatorCollection);
        noteDisplay.focusable = true;
        noteDisplay.RegisterCallback<KeyDownEvent>((e)=>{
            switch(e.keyCode){
                case KeyCode.Alpha1:
                    noteInpState = 1;
                break;
                case KeyCode.Alpha2:
                    noteInpState = 2;
                break;
                case KeyCode.Alpha3:
                    noteInpState = 3;
                break;
            }
        });
        noteDisplay.RegisterCallback<MouseDownEvent>((e)=>{
            if(e.button == (int)MouseButton.LeftMouse){
                mouseDownStart = true;
                ((NoteEditorStates)states).t0 = ((NoteEditorStates)states).start + ((NoteEditorStates)states).size * (e.mousePosition.x / noteDisplay.localBound.width);
            }
        });
        noteDisplay.RegisterCallback<MouseMoveEvent>((e)=>{
            if(mouseDownStart){
                ((NoteEditorStates)states).t1 = ((NoteEditorStates)states).start + ((NoteEditorStates)states).size * (e.mousePosition.x / noteDisplay.localBound.width);
            }
            if(focused){
                NoteEditorStates _states = ((NoteEditorStates)states);
                _states.cursorPosition = _states.start + _states.size * (e.mousePosition.x / noteDisplay.worldBound.width);
            }
        });
        noteDisplay.RegisterCallback<MouseUpEvent>((e)=>{
            if(e.button == (int)MouseButton.LeftMouse){
                mouseDownStart = false;
                if(lanes.Count == 0)
                    return;
                NoteEditorStates _states = (NoteEditorStates)states;
                _states.t1 = _states.start + _states.size * (e.mousePosition.x / noteDisplay.localBound.width);

                //find lane num
                float minDiff = float.MaxValue;
                int minCnt = 0;
                int cnt = 0;
                foreach(var h in horizontalIndicators){
                    if(minDiff > Mathf.Abs(e.localMousePosition.y - h.localBound.y)){
                        minDiff = Mathf.Abs(e.localMousePosition.y - h.localBound.y);
                        minCnt = cnt;
                    }
                    cnt++;
                }

                if(noteInpState == 1){
                    float packSize = 60 / _states.bpm;
                    float time = Mathf.CeilToInt(_states.t0 / packSize) * packSize;
                    if(_states.t1 - _states.t0 > 0.05){
                        lanes[minCnt].Add(new Note(time - packSize, 1));
                        while(time < _states.t1){
                            lanes[minCnt].Add(new Note(time, 1));
                            time += packSize;
                        }
                    }
                    else{
                        lanes[minCnt].Add(new Note(time - packSize, 1));
                    }
                }
                else if(noteInpState == 2){
                    float packSize = 60 / _states.bpm;
                    float time = Mathf.CeilToInt(_states.t0 / packSize) * packSize;
                    float temp = time - packSize;
                    while(time < _states.t1){
                        time += packSize;
                    }
                    if(temp != time - packSize)
                        lanes[minCnt].Add(new Note(temp, time - packSize, 2));
                }
                else if(noteInpState == 3){
                    float packSize = 60 / _states.bpm;
                    float time = Mathf.CeilToInt(_states.t0 / packSize) * packSize;
                    float Ta = time - packSize;
                    for(int i = 0; i < lanes[minCnt].Count;){
                        if(lanes[minCnt][i].noteType == 1){
                            if(lanes[minCnt][i].timing >= Ta && lanes[minCnt][i].timing <= _states.t1)
                                lanes[minCnt].RemoveAt(i);
                            else
                                i++;
                        }
                        else if(lanes[minCnt][i].noteType == 2){
                            if(
                                (lanes[minCnt][i].timing >= Ta && lanes[minCnt][i].timing <= _states.t1) ||
                                (lanes[minCnt][i].timing2 >= Ta && lanes[minCnt][i].timing2 <= _states.t1) ||
                                (lanes[minCnt][i].timing <= Ta && lanes[minCnt][i].timing2 >= _states.t1)
                            ){
                                lanes[minCnt].RemoveAt(i);
                            }
                            else
                                i++;
                            }
                        }
                    }

                //remove overlapping notes
                if(noteInpState != 3){
                    lanes[minCnt].Sort((a, b)=>{
                        if(a.timing == b.timing){
                            return a.noteType.CompareTo(b.noteType);
                        }
                        return a.timing.CompareTo(b.timing);
                    });
                    for(int i = 0; i < lanes[minCnt].Count - 1;){
                        if(lanes[minCnt][i].noteType == 1 && lanes[minCnt][i+1].noteType == 1){
                            if(lanes[minCnt][i].timing == lanes[minCnt][i+1].timing){
                                lanes[minCnt].RemoveAt(i+1);
                            }
                            else
                                i++;
                        }
                        else if(lanes[minCnt][i].noteType == 2 && lanes[minCnt][i+1].noteType == 2){
                            if(
                                (lanes[minCnt][i].timing >= lanes[minCnt][i+1].timing && lanes[minCnt][i].timing <= lanes[minCnt][i+1].timing2) ||
                                (lanes[minCnt][i+1].timing >= lanes[minCnt][i].timing && lanes[minCnt][i+1].timing <= lanes[minCnt][i].timing2)
                            ){
                                lanes[minCnt][i].timing = (lanes[minCnt][i].timing > lanes[minCnt][i+1].timing ? lanes[minCnt][i+1].timing : lanes[minCnt][i].timing);
                                lanes[minCnt][i].timing2 = (lanes[minCnt][i].timing2 > lanes[minCnt][i+1].timing2 ? lanes[minCnt][i].timing2 : lanes[minCnt][i+1].timing2);
                                lanes[minCnt].RemoveAt(i+1);
                            }
                            else
                                i++;
                        }
                        else{
                            int longNote, shortNote;
                            longNote = (lanes[minCnt][i].noteType > lanes[minCnt][i + 1].noteType ? i : i + 1);
                            shortNote = (lanes[minCnt][i].noteType > lanes[minCnt][i + 1].noteType ? i + 1 : i);
                            if(lanes[minCnt][shortNote].timing >= lanes[minCnt][longNote].timing && lanes[minCnt][shortNote].timing <= lanes[minCnt][longNote].timing2){
                                lanes[minCnt].RemoveAt(shortNote);
                            }
                            else
                                i++;
                        }
                    }
                }

                _states.t0 = 0;
                _states.t1 = 0;
            }
            else if(e.button == (int)MouseButton.RightMouse){
                dd.root.style.left = e.localMousePosition.x;
                dd.root.style.top = e.localMousePosition.y;
                dd.root.BringToFront();
                focused = false;
            }
        });
        noteDisplay.RegisterCallback<MouseLeaveEvent>((e)=>{
            mouseDownStart = false;
        });
        noteDisplay.RegisterCallback<FocusInEvent>((e)=>{
            focused = true;
        });
        noteDisplay.RegisterCallback<FocusOutEvent>((e)=>{
            focused = false;
        });

        (string, Action)[] elements = {
            ("Add Lane", ()=>{
                if(((NoteEditorStates)states).audioClip != null){
                    lanes.Add(new List<Note>());
                    ((NoteEditorStates)states).numLanes++;

                    Vector3 hsvColor = new Vector3(0.15f * (lanes.Count - 1), 0.75f, 0.75f);
                    Color color = Color.HSVToRGB(hsvColor.x, hsvColor.y, hsvColor.z);

                    VisualElement horizontalIndicator = new VisualElement();
                    horizontalIndicator.AddToClassList("horizontal-indicator");
                    horizontalIndicator.style.backgroundColor = color;

                    horizontalIndicators.Add(horizontalIndicator);
                    horizontalIndicatorCollection.Add(horizontalIndicator);
                }
                dd.root.style.left = -500;
            }),
            ("Remove Lane", ()=>{
                Debug.Log("Remove Lane");
                dd.root.style.left = -500;
            })
        };
        dd = new DropDown(elements);
        dd.root.style.left = -500;
        noteDisplay.Add(dd.root);

        lanes = new List<List<Note>>();
        horizontalIndicators = new List<VisualElement>();
        horizontalIndicatorCollection = new VisualElement();
        horizontalIndicatorCollection.name = "horizontal-indicator-collection";
        horizontalIndicatorCollection.style.width = Length.Percent(100);
        horizontalIndicatorCollection.style.height = Length.Percent(100);
        noteDisplay.Add(horizontalIndicatorCollection);

        noteIndicators = new List<VisualElement>();
        noteIndicatorCollection = new VisualElement();
        noteIndicatorCollection.name = "note-indicator-collection";
        noteIndicatorCollection.style.width = Length.Percent(100);
        noteIndicatorCollection.style.height = Length.Percent(100);
        noteIndicatorCollection.style.position = Position.Absolute;
        noteDisplay.Add(noteIndicatorCollection);
        noteIndicatorCollection.BringToFront();

        lNoteIndicators = new List<(VisualElement, VisualElement, VisualElement)>();

        clipEndIndicator = new VisualElement();
        clipEndIndicator.AddToClassList("vertical-indicator");
        clipEndIndicator.style.backgroundColor = Color.magenta;
        clipEndIndicator.style.height = Length.Percent(100);
        clipEndIndicator.style.position = Position.Absolute;
        noteDisplay.Add(clipEndIndicator);

        cursor = new VisualElement();
        cursor.AddToClassList("vertical-indicator");
        cursor.style.backgroundColor = new Color(1, 0.49f, 0.31f);
        cursor.style.height = Length.Percent(100);
        cursor.style.position = Position.Absolute;
        noteDisplay.Add(cursor);
    }
    protected override void _Update(){
        // Debug.Log("NoteEditor::Update");
        
        NoteEditorStates _states = (NoteEditorStates)states;
        float packSize = 60.0f / _states.bpm;
        float time = Mathf.CeilToInt(_states.start / packSize) * packSize;
        int ind = 0;
        while(time < _states.start + _states.size){
            VisualElement curVertInd;
            if(ind < verticalIndicators.Count){
                curVertInd = verticalIndicators[ind];
            }
            else{
                curVertInd = new VisualElement();
                curVertInd.AddToClassList("vertical-indicator");
                curVertInd.style.height = Length.Percent(100);
                curVertInd.style.backgroundColor = Color.grey;
                curVertInd.style.position = Position.Absolute;
                verticalIndicators.Add(curVertInd);
                verticalIndicatorCollection.Add(curVertInd);
            }
            curVertInd.style.left = Length.Percent(100 * (time - _states.start) / _states.size);
            time += packSize;
            ind++;
        }
        while(ind < verticalIndicators.Count){
            verticalIndicators[ind].style.left = -100;
            ind++;
        }
    
        int cnt = 0;
        horizontalIndicators.ForEach((h)=>{
            h.style.top = Length.Percent(100f * (cnt + 1) / (_states.numLanes + 1));
            h.style.position = Position.Absolute;
            cnt++;
        });

        cnt = 0;
        int cnt2 = 0;
        for(int i = 0; i < lanes.Count; i++){
            for(int j = 0; j < lanes[i].Count; j++){
                if(lanes[i][j].noteType == 1){
                    if(lanes[i][j].timing < _states.start)
                        continue;
                    if(lanes[i][j].timing > _states.start + _states.size)
                        break;
                    VisualElement noteIndicator;
                    if(cnt < noteIndicators.Count){
                        noteIndicator = noteIndicators[cnt];
                    }
                    else{
                        noteIndicator = new VisualElement();
                        noteIndicator.AddToClassList("note-indicator");
                        noteIndicators.Add(noteIndicator);
                        noteIndicatorCollection.Add(noteIndicator);
                    }
                    noteIndicator.style.left = Length.Percent((100 * (lanes[i][j].timing - _states.start) / _states.size));
                    noteIndicator.style.top = horizontalIndicators[i].style.top;
                    noteIndicator.transform.position = new Vector3(-7.5f, -7.5f, 0);
                    noteIndicator.style.unityBackgroundImageTintColor = horizontalIndicators[i].style.backgroundColor;
                    cnt++;
                }
                if(lanes[i][j].noteType == 2){
                    if(lanes[i][j].timing2 < _states.start)
                        continue;
                    if(lanes[i][j].timing > _states.start + _states.size)
                        break;
                    (VisualElement, VisualElement, VisualElement) lNoteIndicator;
                    if(cnt2 < lNoteIndicators.Count){
                        lNoteIndicator = lNoteIndicators[cnt2];
                    }
                    else{
                        lNoteIndicator.Item1 = new VisualElement();
                        lNoteIndicator.Item2 = new VisualElement();
                        lNoteIndicator.Item3 = new VisualElement();
                        lNoteIndicator.Item1.AddToClassList("long-note-start");
                        lNoteIndicator.Item2.AddToClassList("long-note-finish");
                        lNoteIndicator.Item3.AddToClassList("long-note-middle");
                        lNoteIndicators.Add(lNoteIndicator);
                        noteIndicatorCollection.Add(lNoteIndicator.Item1);
                        noteIndicatorCollection.Add(lNoteIndicator.Item2);
                        noteIndicatorCollection.Add(lNoteIndicator.Item3);
                    }
                    
                    if(lanes[i][j].timing >= _states.start && lanes[i][j].timing2 <= _states.start + _states.size){
                        //start
                        lNoteIndicator.Item1.style.left = Length.Percent(100 * (lanes[i][j].timing - _states.start) / _states.size);
                        lNoteIndicator.Item1.style.top = horizontalIndicators[i].style.top;
                        lNoteIndicator.Item1.transform.position = new Vector3(-7.5f, -7.5f, 0);
                        lNoteIndicator.Item1.style.unityBackgroundImageTintColor = horizontalIndicators[i].style.backgroundColor;
                        //finish
                        lNoteIndicator.Item2.style.left = Length.Percent(100 * (lanes[i][j].timing2 - _states.start) / _states.size);
                        lNoteIndicator.Item2.style.top = horizontalIndicators[i].style.top;
                        lNoteIndicator.Item2.transform.position = new Vector3(-7.5f, -7.5f, 0);
                        lNoteIndicator.Item2.style.unityBackgroundImageTintColor = horizontalIndicators[i].style.backgroundColor;
                        //middle
                        lNoteIndicator.Item3.style.left = Length.Percent(lNoteIndicator.Item1.style.left.value.value);
                        lNoteIndicator.Item3.style.top = horizontalIndicators[i].style.top;
                        lNoteIndicator.Item3.transform.position = new Vector3(0, -7.5f, 0);
                        lNoteIndicator.Item3.style.width = Length.Percent(lNoteIndicator.Item2.style.left.value.value - lNoteIndicator.Item1.style.left.value.value);
                        lNoteIndicator.Item3.style.unityBackgroundImageTintColor = horizontalIndicators[i].style.backgroundColor;
                        cnt2++;
                    }
                    else{
                        if(lanes[i][j].timing < _states.start){
                            //finish
                            lNoteIndicator.Item2.style.left = Length.Percent(100 * (lanes[i][j].timing2 - _states.start) / _states.size);
                            lNoteIndicator.Item2.style.top = horizontalIndicators[i].style.top;
                            lNoteIndicator.Item2.style.unityBackgroundImageTintColor = horizontalIndicators[i].style.backgroundColor;
                            //middle
                            lNoteIndicator.Item3.style.left = Length.Percent(0);
                            lNoteIndicator.Item3.style.top = horizontalIndicators[i].style.top;
                            lNoteIndicator.Item3.style.width = Length.Percent(lNoteIndicator.Item2.style.left.value.value);
                            lNoteIndicator.Item3.style.unityBackgroundImageTintColor = horizontalIndicators[i].style.backgroundColor;
                            cnt2++;
                        }
                        else{
                            //start
                            lNoteIndicator.Item1.style.left = Length.Percent(100 * (lanes[i][j].timing - _states.start) / _states.size);
                            lNoteIndicator.Item1.style.top = horizontalIndicators[i].style.top;
                            lNoteIndicator.Item1.style.unityBackgroundImageTintColor = horizontalIndicators[i].style.backgroundColor;
                            //middle
                            lNoteIndicator.Item3.style.left = Length.Percent(lNoteIndicator.Item1.style.left.value.value);
                            lNoteIndicator.Item3.style.top = horizontalIndicators[i].style.top;
                            lNoteIndicator.Item3.style.width = Length.Percent(100 - lNoteIndicator.Item1.style.left.value.value);
                            lNoteIndicator.Item3.style.unityBackgroundImageTintColor = horizontalIndicators[i].style.backgroundColor;
                            cnt2++;
                        }
                    }
                }
            }
        }

        while(cnt < noteIndicators.Count){
            noteIndicators[cnt].style.left = -500;
            cnt++;
        }
        while(cnt2 < lNoteIndicators.Count){
            lNoteIndicators[cnt2].Item1.style.left = -500;
            lNoteIndicators[cnt2].Item2.style.left = -500;
            lNoteIndicators[cnt2].Item3.style.left = -500;
            lNoteIndicators[cnt2].Item3.style.width = 1;
            cnt2++;
        }
    
        if(_states.audioClip.length < _states.size){
            clipEndIndicator.style.left = Length.Percent(100 * (_states.audioClip.length /_states.size));
        }
        else{
            clipEndIndicator.style.left = -500;
        }
    
        if(_states.cursorPosition >= _states.start && _states.cursorPosition <= _states.start + _states.size){
            cursor.style.left = Length.Percent(((_states.cursorPosition - _states.start) / _states.size) * 100);
        }
        else{
            cursor.style.left = -500;
        }
    }
    protected override void Synchronize()
    {
        //do nothing
    }
    protected override void _Dispose()
    {
        //do nothing
    }
}
public class Note{
    public float timing;
    public float timing2;
    public int noteType;
    public Note(float _timing, int _noteType){
        timing = _timing;
        noteType = _noteType;
    }
    public Note(float _timing, float _timing2, int _noteType){
        timing = _timing;
        timing2 = _timing2;
        noteType = _noteType;
    }
}