using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KusoGame.Signals;

public class line3 : MonoBehaviour
{
    [SerializeField] GameObject Input_line3;
    public Switch inputSwitch;

    private void OnMouseEnter()
    {
        // if(line1.instance.swipping == true){
            
        // }
        inputSwitch.Invoke(3);
    }
}
