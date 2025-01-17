﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongDisplay : MonoBehaviour
{
    public Transform targetTransform;
    public SongItemDisplay itemDisplayPrefab;


    public void SetData(List<SongItem> items)
    {
        foreach(SongItem item in items)
        {
            SongItemDisplay display = (SongItemDisplay)Instantiate(itemDisplayPrefab);
            display.transform.SetParent(targetTransform, false);
            display.SetDisplayData(item);
        }

        
    }
}
