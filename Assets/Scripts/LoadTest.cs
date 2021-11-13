using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTest : MonoBehaviour
{
    AudioSource audioSource;

    void Awake(){
        audioSource = GetComponent<AudioSource>();

        // if(Application.platform == RuntimePlatform.Android){
            Debug.LogError("ANDROID!");
            TextAsset txtAsset = Resources.Load<TextAsset>("Texts/HelloWorld");
            Debug.LogError(txtAsset == null);
            Debug.LogError(txtAsset.text);

            string temp = "Haru Modoki (Asterisk DnB Remix Cut)";
            audioSource.clip = Resources.Load<AudioClip>(temp + "/" + temp);
            Debug.Log(audioSource.clip == null);
            audioSource.Play();
        // }
        // else{
            // Debug.LogError("NOT ANDROID!");
        // }
    }
}
