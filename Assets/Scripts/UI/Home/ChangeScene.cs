using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string SceneToLoad;
    string curState;

    public void LoadScene(){
        StartCoroutine(PlayAnimLoadScene());
    }

    IEnumerator PlayAnimLoadScene(){
        PlayAnimation.instance.Stroke1();

        yield return new WaitForSeconds(0.7f);

        SceneManager.LoadScene(SceneToLoad);
    }
}
