using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataParse
{
    NoteData Data;

    enum State
    {
        DataInfo,
        ContentInfo,
        NoteInfo
    }
    State state;
    public bool isfirstRead;

    public DataParse(NoteData _data)
    {
        Data = _data;
    }
    public void Parse(string data)
    {
        CheckCurrentMetadata(data);

        if (state == State.DataInfo)
            ParseDataInfo(data);
        else if (state == State.ContentInfo)
            ParseContentInfo(data);
        else if (state == State.NoteInfo)
            ParseNoteInfo(data);
    }

    void CheckCurrentMetadata(string data)
    {
        if (data == "[DataInfo]") state = State.DataInfo;
        else if (data == "[ContentInfo]") state = State.ContentInfo;
        else if (data == "[NoteInfo]") state = State.NoteInfo;
    }

    public void ParseDataInfo(string data)
    {
        string[] splitedData = new string[2];
        splitedData = data.Split('=');

        if (splitedData[0] == "AudioFileName")
            Data.fileName = splitedData[1];
        else if (splitedData[0] == "AudioViewTime")
            Data.previewTime = int.Parse(splitedData[1]);
        else if (splitedData[0] == "ImageFileName")
            Data.imgFileName = splitedData[1];
        else if (splitedData[0] == "BPM")
            Data.bpm = float.Parse(splitedData[1]);
        else if (splitedData[0] == "Offset")
            Data.offset = float.Parse(splitedData[1]);
    }

    public void ParseContentInfo(string data)
    {
        string[] splitedData = new string[2];
        splitedData = data.Split('=');

        if (splitedData[0] == "Title")
            Data.title = splitedData[1];
        else if (splitedData[0] == "Artist")
            Data.artist = splitedData[1];
        else if (splitedData[0] == "Source")
            Data.source = splitedData[1];
        else if (splitedData[0] == "Difficult")
            Data.diff = splitedData[1];
    }

    public void ParseNoteInfo(string data)
    {
        if(!isfirstRead) // [NoteInfo] 문자열은 무시한다.
        {
            isfirstRead = true;
            return;
        }

        string[] splitedData = new string[2];
        int time = 0;
        int lineNumber = 1;
        splitedData = data.Split(',');

        int.TryParse(splitedData[0], out time);
        int.TryParse(splitedData[1], out lineNumber);

        Data.notes[lineNumber - 1].Add(time);
        // if (lineNumber == 1)
        //     Data.notes[0].Add(time);
        // else if (lineNumber == 2)
        //     Data.notes[1].Add(time);
        // else if (lineNumber == 3)
        //     Data.notes[2].Add(time);
        // else if (lineNumber == 4)
        //     Data.notes[3].Add(time);
    }
}
