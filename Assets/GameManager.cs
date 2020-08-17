using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    
    private Dictionary<int, int> spacing = new Dictionary<int, int>() {
        {8, -1340},
        {7, -1290},
        {6, -1200},
        {5, -1165},
        {4, -1135},
        {3, -1090},
        {2, -1075},
    };

    public Image background;
    public GameObject canvas;
    public Text[] otherPlayersNames;
    
    private GameObject hand;
    public GameObject cardPrefab;
    public Texture2D cardDeck;
    public Sprite[] sprites;
    
    private void Start()  {
        var _response = ClientTCP.getResponseFromServer(true, "Matched started");
        if(_response != Response.RECEIVED_CARD_LIST)
            ClientTCP.getResponseFromServer(true, "Getting cards");
        
        this.hand = GameObject.Find("Hand");
        this.sprites = Resources.LoadAll<Sprite>(this.cardDeck.name);
        Debug.Log(GlobalInfo.playerCards.Count);
        this.hand.GetComponent<HorizontalLayoutGroup>().spacing = spacing[GlobalInfo.playerCards.Count];
        
        for (var _i = 0; _i < GlobalInfo.playerCards.Count; _i++) {
            var _card = Instantiate(this.cardPrefab, new Vector3(), Quaternion.identity);
            var _spriteChild = _card.transform.GetChild(0).gameObject;
            
            _spriteChild.GetComponent<SpriteRenderer>().sprite = this.getCardSprite(GlobalInfo.playerCards[_i].Value, GlobalInfo.playerCards[_i].Suit);
            _card.transform.SetParent(this.hand.transform);
            var _rectTransform = _card.GetComponent<RectTransform>();
            _rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
            var _position = _rectTransform.position;
            _position.Set(_position.x, _position.y, -_i);
        }

        var _enemyHand = GameObject.Find("EnemyHand1");
        _enemyHand.GetComponent<HorizontalLayoutGroup>().spacing = spacing[GlobalInfo.playerCards.Count];
        
        for (var _i = 0; _i < GlobalInfo.playerCards.Count; _i++) {
            var _card = Instantiate(this.cardPrefab, new Vector3(), Quaternion.identity);
            var _spriteChild = _card.transform.GetChild(0).gameObject;
            
            _spriteChild.GetComponent<SpriteRenderer>().sprite = this.sprites[10];
            _card.transform.SetParent(_enemyHand.transform);
            var _rectTransform = _card.GetComponent<RectTransform>();
            _rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
            var _position = _rectTransform.position;
            _position.Set(_position.x, _position.y, -_i);
        }
        
        this.background.GetComponent<RectTransform>().sizeDelta = this.canvas.GetComponent<RectTransform>().sizeDelta;
        for (var _i = 0; _i < GlobalInfo.otherPlayers.Count; _i++)
            this.otherPlayersNames[_i].text = GlobalInfo.otherPlayers[_i].UserName;
    }
    
    private void Update() {
        
    }
    
    private Sprite getCardSprite(int _value, Suit _suit) {
        var _spritePos = 11 * ((int) _suit) + (_value - (_value <= 7 ? 1 : 3));
        return this.sprites[_spritePos];
    }

    // private Sprite getCardSprite(int _value, Suit _suit) {
    //     return null;
    // }

    // private Transform[] divideTable() {
    //     
    // }
}
