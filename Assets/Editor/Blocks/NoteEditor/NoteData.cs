using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteData
{
    [Header("Sheet Info")]
    public string fileName;
    public string imgFileName;
    public int previewTime;
    public float bpm;
    public float offset;

    [Header("Content Info")]
    public string title;
    public string artist;
    public string source;
    public string sheet;
    public string diff;

    [Header("Note Info")]
    public List<List<float>> notes;
    public NoteData()
    {
        notes = new List<List<float>>();
    }
}