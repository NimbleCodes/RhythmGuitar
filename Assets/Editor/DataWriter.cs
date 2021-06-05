using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataWriter : MonoBehaviour
{
    public NoteData N_data;

    public string WriteSheetInfo()
    {
        string data = "";

        data += "[SheetInfo]" +
            "\nAudioFileName=" + N_data.fileName +
            "\nAudioViewTime=" + N_data.previewTime +
            "\nImageFileName=" + N_data.imgFileName + "_Img" +
            "\nBPM=" + N_data.bpm +
            "\nOffset=" + N_data.offset +
            "\nBeat=44\nBit=32\nBar=80\n\n";

        return data;
    }

    public string WriteContentInfo()
    {
        string data = "";

        data += "[ContentInfo]" +
            "\nTitle=" + N_data.title +
            "\nArtist=" + N_data.artist +
            "\nSource=" + N_data.source +
            "\nDifficult=" + N_data.diff +
            "\n\n";

        return data;
    }

    public string WriteNoteInfo()
    {
        string data = "";
        
        data += "[NoteInfo]\n";

        foreach (int note in N_data.notes[0])
        {
            data += note.ToString() + ",1\n";
        }
        foreach (int note in N_data.notes[1])
        {
            data += note.ToString() + ",2\n";
        }
        foreach (int note in N_data.notes[2])
        {
            data += note.ToString() + ",3\n";
        }
        foreach (int note in N_data.notes[3])
        {
            data += note.ToString() + ",4\n";
        }

        return data;
    }
}