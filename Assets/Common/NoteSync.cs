using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSync : MonoBehaviour
{
    Player player;
    AudioSource music;

    AudioSource playSync;
    public AudioClip SyncClip;

    public GameObject dataObj;
    NoteData data;

    float musicBPM;
    float stdBPM = 60.0f;
   
    public float oneBeatTime = 0f;
    public float beatPerSample = 0f;

    public float bitPerSec = 0f;
    public float bitPerSample = 0f;

    float frequency = 0f;
    float nextSample = 0f;
    
    public float offset; // 음악의 시작점(초)
    public float offsetForSample; // 음악의 시작점(샘플)

    public float scrollSpeed; //곡 bpm에 따른 기본 배속
    public float userSpeedRate;

    void Start()
    {
        player = FindObjectOfType<Player>();

        playSync = GetComponent<AudioSource>();
        music = FindObjectOfType<SongManager>().GetComponent<AudioSource>();
        data = dataObj.GetComponent<Scene_1>()._noteData;
        //generator = GameObject.Find("GeneratorNote").GetComponent<GeneratorNote>();  //노트 생성자에 적용.

        scrollSpeed = 10.0f;
        userSpeedRate = 1f;

        musicBPM = data.bpm;
        // 현재곡의 주파수값
        frequency = music.clip.frequency;
        // 시작점
        offset = data.offset;
        // 시작점 초를 샘플로 변환
        offsetForSample = frequency * offset;
        // 한박자 시간값
        oneBeatTime = (stdBPM / musicBPM);// * (musicBeat / stdBeat);
        // 첫박자 샘플값(오프셋)
        nextSample += offsetForSample;
        // 32비트기준 1비트의 시간값
        //bitPerSec = stdBPM / (8 * musicBPM);
        // 32비트기준 1비트의 샘플값
        //bitPerSample = bitPerSec * playMusic.clip.frequency;
        // 1바 시간값    
    }

    IEnumerator PlayTik()
    {   
        // 초당 44100 샘플값 증가 프리퀀시값인 44100나누면 정확히 나눠떨어짐
        if (music.timeSamples >= nextSample)
        {
            playSync.PlayOneShot(SyncClip); // 사운드 재생
            beatPerSample = oneBeatTime * frequency;
            nextSample += beatPerSample;
        }
        yield return null;
    }

}
