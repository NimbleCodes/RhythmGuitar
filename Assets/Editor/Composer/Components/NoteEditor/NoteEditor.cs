
public class NoteEditor : Component
{
    Observer observer;

    public NoteEditor(Observer _observer) : base("Assets/Editor/Composer/Components/NoteEditor/NoteEditor.uxml"){
        rootVisualElement.name = "note_editor";

        observer = _observer;
        observer.Subscribe("start", Start);
        observer.Subscribe("update", Update);
        observer.Subscribe("destroy", Destroy);
    }
    public void Start(dynamic[] _args){

    }
    public void Update(dynamic[] _args){

    }
    public void Destroy(dynamic[] _args){

    }
    #region VisualElement interaction event callbacks

    #endregion
    #region External event callbacks

    #endregion
}