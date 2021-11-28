using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    void Update()
    {
        Debug.Log(GetClicke2Dobject() == null);
    }

    public GameObject GetClicke2Dobject(int layer = -1){
        GameObject target = null;

        int mask = 1 << layer;

        Vector2 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(pos, Vector2.zero);
        RaycastHit2D hit;
        hit = layer == -1 ? Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity) : Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, mask);

        if(hit){
            target = hit.collider.gameObject;
        }
        return target;
    }
}
