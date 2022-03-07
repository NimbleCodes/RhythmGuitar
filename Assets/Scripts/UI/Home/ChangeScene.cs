using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string SceneToLoad;
    string curState;

    public void LoadScene(){
        PlayAnimation.instance.Stroke1();
        SceneManager.LoadScene(SceneToLoad);//버튼 클릭시 Scene 호출
    }
}
