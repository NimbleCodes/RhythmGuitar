using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using kgh.Signals;

public class SongItemDisplay : MonoBehaviour
{
    [SerializeField] Button m_button = null;
    [SerializeField] Text   m_name   = null;
    public Text textName,textLevel,textArtist;
    public Image sprite;

    public delegate void SongItemDisplayDelegate(SongItem item);
    public event SongItemDisplayDelegate onClick;

    public SongItem item;
    public SongManager songManager;
    
    //이벤트가 변수값을 기억해 다른 선택지 눌렀다가 다시 같은것을 눌렀을때 씬 전환이 되지 않기 위해 Static을 사용.
    static string songCheck = "";
    static int clickCnt = 0;
    Player player;

    void Start()
    {
        //if (item != null) Prime(item);

        songManager = GameObject.Find("SongSelect").GetComponent<SongManager>();

        player = FindObjectOfType<Player>();

    }

    public void Prime(SongItem item)
    {
        this.item = item;
        if (textName != null)
            textName.text = item.songName;
        if (textLevel != null)
            textLevel.text = item.songLevel;
        if (textArtist != null)
            textArtist.text = item.songArtist;
        if (sprite != null)
            sprite.sprite = item.sprite;

        //onClick += PeekPanel.instance.OnPreview;
    }   
    public void SetScale( float scale )
		{
			m_button.GetComponent<CanvasGroup>().alpha = scale;
			m_button.transform.localScale = new Vector3( scale, scale, 1 );
		}
    public void Click()
    {
        string m_Name = textName.text;

        if (onClick != null)
            onClick.Invoke(item);
        else
        {
            // 같은 항목을 두번 클릭하면 플레이
            if (songCheck.Equals(m_Name)){
                clickCnt++;
            }
            else
            {
                clickCnt = 0;
                songCheck = m_Name;
                clickCnt++;
            }

            Debug.Log(m_Name);
            // 노래 미리듣기
            songManager.PlayAudioPreview(m_Name);
            
            PreviewPanel.instance.OnPreview(item);

            //Play씬 전환및 전달할 곡데이터
            if (clickCnt.Equals(2))
            {
                songManager.SelectSong(m_Name);
                clickCnt = 0; // ESC를 눌러 돌아왔을때 제대로 작동하기 위함
                //if (!player.isEditMode)
                    SceneManager.LoadSceneAsync("VerticalTest");
            }


        }
    }
}
