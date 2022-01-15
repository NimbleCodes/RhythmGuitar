using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Scene_1_Manager))]
public class Evaluator : MonoBehaviour
{
    Scene_1_Manager scene_1;
    const float visibleAreaSize = 3.0f;
    List<int> indexVals;
    List<float> evalVals;
    const float lastChance = 0.1f;
    const float window = 0.25f;

    private void Awake(){
        scene_1 = GetComponent<Scene_1_Manager>();
    }
    private void Start(){
        GameManager.instance.sigs.Subscribe("OnMouseBehavior", this, "OnMouseBehavior");
        GameManager.instance.sigs.Subscribe("game_over", this, "GameOver");
        for(int i = 0; i < scene_1.noteData.notes.Count; i++){
            indexVals.Add(0);
        }
    }
    private void OnMouseBehavior(int direction, int lineCount){
        //Debug.Log(direction + ", " + lineCount);
    }
    void GameOver(){
        Debug.Log("Game Over");
    }
}
