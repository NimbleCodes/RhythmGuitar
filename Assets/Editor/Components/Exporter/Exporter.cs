using UnityEngine;
using UnityEngine.UIElements;

class Exporter : Component{
    NoteData noteData;
    DataIO dataIO;

    public Exporter(NoteData _noteData) : base("Assets/Editor/Components/Exporter/Exporter.uxml"){
        noteData = _noteData;
        dataIO = new DataIO(noteData);

        Button exportBtn = rootVisualElement.Query<Button>("export_btn");
        exportBtn.clicked += ()=>{
            dataIO.Save();
        };
    }
}