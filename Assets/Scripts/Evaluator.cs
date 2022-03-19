using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Scene_1_Manager))]
public class Evaluator : MonoBehaviour
{
    Scene_1_Manager scene_1;
    const float visibleAreaSize = 3.0f;
    const float lastChance = 0.1f;
    const float window = 0.25f;

    private void Awake(){
        scene_1 = GetComponent<Scene_1_Manager>();
    }
    private void Start(){
        GameManager.instance.sigs.Subscribe("OnMouseBehavior", this, "OnMouseBehavior");
        GameManager.instance.sigs.Subscribe("game_over", this, "GameOver");
    }
    private void OnMouseBehavior(int direction, int lineCount){
        //1->up stroke 2->down stroke
        // Debug.Log(direction + ", " + lineCount);
    }
    void GameOver(){
        Debug.Log("Game Over");
    }
}
