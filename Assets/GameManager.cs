using System.Collections;
using System.Collections.Generic;
using Games;
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

    public Text myTurnText;
    
    private GameObject hand;
    public GameObject cardPrefab, enemyCardPrefab;
    public Texture2D cardDeck;
    public Sprite[] sprites;

    public Button playCardButton;

    private void Start()  {
        Debug.Log("Handling match starting.");
        var _response = ClientTCP.getResponseFromServer(true, "Matched started");
        Debug.Log($"Handled match started with response {_response}.");
        if (_response == Response.LOAD_MATCH_SCENE) {
            Debug.Log("Handling getting cards.");
            _response = ClientTCP.getResponseFromServer(true, "Getting cards");
            Debug.Log($"Handled got cards with response {_response}.");
        }

        this.hand = GameObject.Find("Hand");
        this.sprites = Resources.LoadAll<Sprite>(this.cardDeck.name);
        this.hand.GetComponent<HorizontalLayoutGroup>().spacing = spacing[GlobalInfo.playerCards.Count];
        
        for (var _i = 0; _i < GlobalInfo.playerCards.Count; _i++) {
            var _card = Instantiate(this.cardPrefab, new Vector3(), Quaternion.identity);
            
            var _script = _card.GetComponent<CardController>();
            _script.Value = GlobalInfo.playerCards[_i].Value;
            _script.Suit = GlobalInfo.playerCards[_i].Suit;

            var _spriteChild = _card.transform.GetChild(0).gameObject;
            _spriteChild.GetComponent<SpriteRenderer>().sprite = this.getCardSprite(GlobalInfo.playerCards[_i].Value, GlobalInfo.playerCards[_i].Suit);
            _card.transform.SetParent(this.hand.transform);
            var _rectTransform = _card.GetComponent<RectTransform>();
            _rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
            var _position = _rectTransform.position;
            _position.Set(_position.x, _position.y, -_i);
        }

        
        Debug.Log("Handling getting cards per player per turn.");
        _response = ClientTCP.getResponseFromServer(true, "Cards per player per turn");
        Debug.Log($"Handled got cards per player per turn with response {_response}.");

        var _enemyHand = GameObject.Find("EnemyHand1");
        _enemyHand.GetComponent<HorizontalLayoutGroup>().spacing = spacing[GlobalInfo.playerCards.Count];

        foreach (var _player in GlobalInfo.otherPlayersCardCount) {
            for (var _i = 0; _i < _player.Value; _i++) {
                var _card = Instantiate(this.enemyCardPrefab, new Vector3(), Quaternion.identity);
                _card.transform.SetParent(_enemyHand.transform);
                var _rectTransform = _card.GetComponent<RectTransform>();
                _rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
                var _position = _rectTransform.position;
                _position.Set(_position.x, _position.y, -_i);
            }
        }
        
        this.background.GetComponent<RectTransform>().sizeDelta = this.canvas.GetComponent<RectTransform>().sizeDelta;
        for (var _i = 0; _i < GlobalInfo.otherPlayers.Count; _i++)
            this.otherPlayersNames[_i].text = GlobalInfo.otherPlayers[_i].UserName;
        
        this.myTurnText.gameObject.SetActive(GlobalInfo.isMyTurn);
        this.playCardButton.enabled = GlobalInfo.isMyTurn;
        
        GameObject _cardOnTableParent = GameObject.Find("CardOnTable");
        var _cardOnTable = Instantiate(this.enemyCardPrefab, new Vector3(), Quaternion.identity);
        var _cardOnTableSprite = _cardOnTable.transform.GetChild(0).gameObject;
        TestGame _testGame = (TestGame)GlobalInfo.game;
        _cardOnTableSprite.GetComponent<SpriteRenderer>().sprite = this.getCardSprite(_testGame.getCardOnTable().Value, _testGame.getCardOnTable().Suit);
        _cardOnTable.transform.SetParent(_cardOnTableParent.transform);
        
        var _cardOnTableRectTransform = _cardOnTable.GetComponent<RectTransform>();
        _cardOnTableRectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
        var _cardOnTablePosition = _cardOnTableRectTransform.position;
        _cardOnTablePosition.Set(0, 0, -1);
        
    }

    public void onClickPlayButton() {
        var _selectedCard = GlobalInfo.playerCards.Find(_card => _card.SelectedToPlay);
        if(GlobalInfo.game.isMovementValid(_selectedCard.Value, _selectedCard.Suit))
            Debug.Log("Movimiento valido.");
        else
            Debug.Log("Movimiento erroneo.");
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
