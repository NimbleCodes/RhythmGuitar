using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataWriter
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

        for(int i = 0; i < N_data.notes.Count; i++){
            for(int j = 0; j < N_data.notes[i].Count; j++){
                data += N_data.notes[i][j].ToString() + "," + i.ToString() + N_data.notes[i][j+1].ToString() + "\n";
            }
        }

        return data;
    }
}