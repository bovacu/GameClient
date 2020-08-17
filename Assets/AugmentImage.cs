using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AugmentImage : MonoBehaviour  {
    // Start is called before the first frame update
    public bool isOver = false;
    private float initialZ;

    private void Start() {
        initialZ = GetComponent<RectTransform>().localScale.z;
    }

    public void OnMouseOver() {
        Debug.Log("over");
        var _rect = GetComponent<RectTransform>();
        var _position = _rect.position;
        _position.Set(_position.x, _position.y, -10);
        _rect.localScale.Set(2.5f, 2.5f, 1f);
    }

    public void OnMouseExit() {
        var _rect = GetComponent<RectTransform>();
        var _position = _rect.position;
        _position.Set(_position.x, _position.y, this.initialZ);
        _rect.localScale.Set(1f, 1f, 1f);
    }
}
