using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class HealthManager : MonoBehaviour
{

    #region variables
    //MaxHealth의 기본 기준은 100이다. 쓰이는 곳은 없으나 계산의 용이성을 위해 존재한다.
    float maxHealth = 100;
    //현재 체력. 개발의 용이성을 위해 존재한다.(디버그용임)
    float curHealth = 100;
    //MaxHealth가 자연적으로 소진되는 시간.
    float decrTime = 30;
    //decrTime의 최소 한계점. decrTime은 이 시점 이하로 내려가지 않는다.
    float decrLimit = 20;
    //hp와 Time 의 비율. hp 증감의 쉬운 계산을 위해 이용된다.
    float hpToTime;
    //현재 흘러가는 시간 
    float activeTime = 0;
    //True 이면 HP가 감소하기 시작한다.
    float correctBurger = 100;
    float lostIngre = 1;
    bool decrStart = false;
    bool areusure = true;
    public Action N_Action;
    public static HealthManager Instance;
    [SerializeField]
    public GameObject slidercontroll;
    //싱글턴, score 관리에서 health관리를 위함

    //재활용 시 체력 증가량
    float ingrReturned = 1f;

    Slider healthBar;
    #endregion
    void Awake(){
        Instance = this;
    }
    void Start()
    {
        healthBar = GetComponent<Slider>();
        //EventManager.eventManager.BurgerCompleteEvent += OnBurgerComplete;
        //EventManager.eventManager.GameOverEvent += PopLeaderboard;
        //EventManager.eventManager.IngrDestroyedEvent += minusHealth;
        //EventManager.eventManager.IngrReturnedEvent += OnIngrReturned;
        //EventManager.eventManager.GamePausedEvent += OnGamePaused;
        //EventManager.eventManager.GameResumeEvent += OnGameResume;
        startDecr();
    }
    void Update()
    {
        //test
        //hp 증감의 쉬운 계산을 위해 이용된다.
        hpToTime = decrTime/maxHealth;
        decrHealth();
        isGameOver();
    }

    void OnGamePaused()
    {
        decrStart = false;
    }
    void OnGameResume()
    {
        decrStart = true;
    }

    #region 
    void OnIngrReturned(int trignum)//IngreReturn시에 체력추가
    {
        addHealth(ingrReturned);
    }

    //감소를 시작한다. decrStart를 true로 설정한다.
    public void startDecr(){
        decrStart = true;
    }
    //감소를 멈춘다. decrStart를 false로 설정한다.
    public void stopDecr(){
        decrStart = false;
    }
    //hp만큼 체력을 회복한다. 
    public void addHealth(float correctBurger){
            curHealth += correctBurger;
            activeTime -= correctBurger*hpToTime;
    }
    //hp만큼 체력을 감소한다.
    public void minusHealth(){
            curHealth -= lostIngre;
            activeTime += lostIngre*hpToTime;
    }
    //decrTime을 감소시킨다.(hp가 다는 속도가 빨라지며, decrTime 은 decrLimit 이하로 내려가지 않는다.)
    public void minusTime(float time){
        if((decrTime - time) < decrLimit){
            decrTime = decrLimit;
        }else{
            decrTime -= time;
        }
    }
    public void recoverAllHP(){
        curHealth = maxHealth;
        activeTime = 0;
    }
    //HP가 시간에 따라 감소하는 것을 실행한다.
    public void decrHealth(){
        //실제로 HP를 감소시키는 기능을 하는 코드
        if(decrStart){
            activeTime += Time.deltaTime;
            float percent = activeTime/decrTime;
            curHealth -= Time.deltaTime / hpToTime;
            healthBar.value = Mathf.Lerp(1,0,percent);
        }else{
            healthBar.value = 1-(activeTime/decrTime);
        }

        //시간이 기준을 넘기지 않도록 한다.
        if(activeTime >= decrTime){
            activeTime = decrTime;
        }
        if(activeTime <= 0){
            activeTime = 0;
        }
        //체력이 기준을 넘기지 않도록 한다.
        if(curHealth <= 0){
            curHealth = 0;
        }
        if(curHealth >= maxHealth){
            curHealth = maxHealth;
        }
    }
    //체력이 모두 고갈되었는가를 bool로 return하는 함수.
    public void isGameOver(){
        if(curHealth <= 0){
            //EventManager.eventManager.Invoke_GameOverEvent();
            slidercontroll.SetActive(false);
        }
    }
    public void OnBurgerComplete(bool correct)
    {
        if(correct)
            addHealth(correctBurger);
        else{
            //Do Nothing
        }

    }

    //게임오버시 레더보드, 닉네임입력창 팝업.
    public void PopLeaderboard()
    {
        Action N_Action = () => Debug.Log("GameOver");
        //LeaderboardControll.Instance.ShowLeaderboard(N_Action);
        //NickNamePanel.Instance.ShowNickPanel(N_Action);
    }
    #endregion

}
