using System.Collections.Generic;
using UnityEngine;

public class NoteData
{
    [Header("Data Info")]
    public string fileName;
    public string imgFileName;
    public int previewTime;
    public float bpm;
    public float offset;

    [Header("Content Info")]
    public string title;
    public string artist;
    public string source;
    public string diff;

    [Header("Note Info")]
    public List<List<float>> notes;
    public List<(float, string)> events;

    public NoteData()
    {
        notes = new List<List<float>>();
        notes.Add(new List<float>());
        notes.Add(new List<float>());
        notes.Add(new List<float>());
        notes.Add(new List<float>());
    }
}