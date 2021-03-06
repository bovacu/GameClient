﻿using System;
using UnityEngine;


public class CardController : MonoBehaviour  {
    // Start is called before the first frame update
    private bool moved = false;
    private float initialZ;
    public GameObject child;
    public int Value;
    public Suit Suit;

    private void Start() {
        initialZ = child.GetComponent<RectTransform>().localPosition.z;
    }

    private void Update() {
        if (!GameManager.getCard(Value, Suit).SelectedToPlay) {
            var _rect = child.GetComponent<RectTransform>();
            var _position = _rect.localPosition;
            child.GetComponent<RectTransform>().localPosition = new Vector3(_position.x, 0, initialZ);
            
            var _parent = child.transform.parent;
            _parent.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
        }
    }

    public void OnMouseDown() {
        GameManager.getCard(Value, Suit).SelectedToPlay = !GameManager.getCard(Value, Suit).SelectedToPlay;

        if (GameManager.getCard(Value, Suit).SelectedToPlay) {
            for (var _i = 0; _i < GameManager.Game.getPlayerCards().Count; _i++) {
                if (GameManager.Game.getPlayerCards()[_i].Value == Value && GameManager.Game.getPlayerCards()[_i].Suit == Suit)
                    continue;
                GameManager.Game.getPlayerCards()[_i].SelectedToPlay = false;
            }
        }
        
        var _rect = child.GetComponent<RectTransform>();
        var _position = _rect.localPosition;
        var _toMove = (_rect.sizeDelta.x * 100 / 2f) * (GameManager.getCard(Value, Suit).SelectedToPlay ? 1 : -1);
        child.GetComponent<RectTransform>().localPosition = new Vector3(_position.x, _position.y + _toMove, _toMove > 0 ? -10f : initialZ);

        var _parent = child.transform.parent;
        var _collider = _parent.GetComponent<BoxCollider2D>();
        _collider.offset = new Vector2(_collider.offset.x, _collider.offset.y + _toMove);

        
        
    }
    
}
