using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Games;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static bool newUpdateInGame = false;
    public static int whoMadeTheUpdate = -1;
    public static bool updateMyHand = false;
    public static bool updateEnemyHand = false;
    public static bool justTurnUpdate = false;
    public static bool playWrongMoveAnimation = false;
    public static int winner = -1;
    
    private Dictionary<int, int> spacing = new Dictionary<int, int>() {
        {8, -1340},
        {7, -1290},
        {6, -1200},
        {5, -1165},
        {4, -1135},
        {3, -1090},
        {2, -1075},
        {1, -1075},
        {0, -1075},
    };

    public Image background;
    public GameObject canvas;
    public Text[] otherPlayersNames;

    public Text myTurnText, winText, wrongMovementText;
    public static Vector3 originalWrongMovementTextPosition;
    
    private GameObject hand;
    public GameObject cardPrefab, enemyCardPrefab;
    public Texture2D cardDeck;
    public Sprite[] sprites;

    public Button playCardButton, passTurnBtn, returnToMainMenuBtn;

    public ParticleSystem confetti;

    private void Start()  {
        originalWrongMovementTextPosition = wrongMovementText.GetComponent<RectTransform>().localPosition;
        Debug.Log(originalWrongMovementTextPosition);
        
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
        
        for (var _i = 0; _i < GlobalInfo.playerCards.Count; _i++)
            this.addCardToHand(GlobalInfo.playerCards[_i].Value, GlobalInfo.playerCards[_i].Suit, _i);


        Debug.Log("Handling getting cards per player per turn.");
        _response = ClientTCP.getResponseFromServer(true, "Cards per player per turn");
        Debug.Log($"Handled got cards per player per turn with response {_response}.");
        
        ClientTCP.clearResponsesQueue();

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
        this.passTurnBtn.enabled = GlobalInfo.isMyTurn;
        
        var _random = new System.Random();
        var _rotation = (_random.Next(10, 45)) * (_random.Next(0, 2) == 0 ? 1f : -1f);

        var _testGame = (TestGame) GlobalInfo.game;
        this.addCardToTheTable(_testGame.getCardOnTable().Value, _testGame.getCardOnTable().Suit, 0);
    }

    private void Update() {
        if (winner >= 0) {
            returnToMainMenuBtn.gameObject.SetActive(true);
            
            winText.gameObject.SetActive(true);
            winText.text = winner == GlobalInfo.playerInfo.id ? "You win!!" : "You loose...";

            if (winner == GlobalInfo.playerInfo.id) {
                this.confetti.gameObject.SetActive(true);
                this.confetti.Play();
            }

            playCardButton.enabled = false;
            passTurnBtn.enabled = false;
            myTurnText.gameObject.SetActive(false);
            winner = -1;
            GameManager.newUpdateInGame = false;
        }
        
        if (playWrongMoveAnimation) {
            var _transform = wrongMovementText.transform.localPosition;
            wrongMovementText.transform.localPosition = new Vector3(_transform.x, _transform.y + Time.deltaTime * 50, _transform.z);
            
            var _color = this.wrongMovementText.color;
            _color = Color.Lerp(_color, new Color(_color.r, _color.g, _color.b, 0), 5 * Time.deltaTime);
            this.wrongMovementText.color = _color;

            var _deckGameObject = GameObject.Find("Deck");
            var _cardSon = _deckGameObject.transform.GetChild(0).GetComponent<RectTransform>();
            if (_color.a <= 0 || _transform.y > _deckGameObject.GetComponent<RectTransform>().localPosition.y - _cardSon.sizeDelta.y / 2f) {
                playWrongMoveAnimation = false;
                wrongMovementText.gameObject.SetActive(false);
            }
        }
        
        if (GameManager.newUpdateInGame) {
            GameManager.newUpdateInGame = false;
            TestGame _testGame = (TestGame) GlobalInfo.game;
            var _random = new System.Random();
            var _rotation = (_random.Next(10, 45)) * (_random.Next(0, 2) == 0 ? 1f : -1f);
            this.addCardToTheTable(_testGame.getCardOnTable().Value, _testGame.getCardOnTable().Suit, _rotation);
            
            this.removeCardFromEnemyHand(GameManager.whoMadeTheUpdate, GameObject.Find("EnemyHand1"));
            
            this.myTurnText.gameObject.SetActive(GlobalInfo.isMyTurn);
            this.playCardButton.enabled = GlobalInfo.isMyTurn;
            this.passTurnBtn.enabled = GlobalInfo.isMyTurn;
        }

        if (justTurnUpdate) {
            justTurnUpdate = false;
            this.myTurnText.gameObject.SetActive(GlobalInfo.isMyTurn);
            this.playCardButton.enabled = GlobalInfo.isMyTurn;
            this.passTurnBtn.enabled = GlobalInfo.isMyTurn;
        }

        if (updateMyHand) {
            updateMyHand = false;

            var _zIndices = new List<float>();
            foreach (Transform _card in this.hand.transform)
                _zIndices.Add(_card.GetComponent<RectTransform>().localPosition.z);

            this.addCardToHand(GlobalInfo.playerCards.Last().Value, GlobalInfo.playerCards.Last().Suit, (int)_zIndices.Max() + 1);
        }

        if (updateEnemyHand) {
            updateEnemyHand = false;
            var _enemyHand = GameObject.Find("EnemyHand1");
            _enemyHand.GetComponent<HorizontalLayoutGroup>().spacing = spacing[GlobalInfo.otherPlayersCardCount[GlobalInfo.otherPlayers[0].Id]];
            
            var _card = Instantiate(this.enemyCardPrefab, new Vector3(), Quaternion.identity);
            _card.transform.SetParent(_enemyHand.transform);
            var _rectTransform = _card.GetComponent<RectTransform>();
            _rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
            var _position = _rectTransform.position;
            _position.Set(_position.x, _position.y, -1);
        }
    }
    
    public void onClickPlayButton() {
        if (!GlobalInfo.isMyTurn) return;

        var _selectedCard = GlobalInfo.playerCards.Find(_card => _card.SelectedToPlay);
        if (_selectedCard != null && GlobalInfo.game.isMovementValid(_selectedCard.Value, _selectedCard.Suit)) {
            // Adding the played card to the table.
            var _random = new System.Random();
            var _rotation = (_random.Next(10, 45)) * (_random.Next(0, 2) == 0 ? 1f : -1f);
            this.addCardToTheTable(_selectedCard.Value, _selectedCard.Suit, _rotation);
            
            // Removing it from my hand. Includes removing it from GlobalInfo.playerCards.
            this.removeCardFromMyHand(_selectedCard.Value, _selectedCard.Suit);
            ClientTCP.sendNewTestGameMovementPacket(_selectedCard.Value, _selectedCard.Suit);
            
            this.myTurnText.gameObject.SetActive(false);
            this.playCardButton.enabled = false;
            this.passTurnBtn.enabled = false;
            
            var _testGame = (TestGame) GlobalInfo.game;
            _testGame.DrawOnceAlready = false;
            
            if(GlobalInfo.playerCards.Count == 0)
                ClientTCP.sendTestGamePlayerWonMatch();
            
        } else {
            if (_selectedCard == null) {
                this.restartWrongMovementText("First select a card!");
                return;
            }
            
            this.restartWrongMovementText("Can't do that movement!");
        }
    }

    public void onClickPassTurnButton() {
        TestGame _testGame = (TestGame) GlobalInfo.game;
        
        if (GlobalInfo.playerCards.Any(_card => _testGame.isMovementValid(_card.Value, _card.Suit))) {
            this.restartWrongMovementText("You still have cards to play!");
            return;
        }
        
        if (!_testGame.DrawOnceAlready) {
            this.restartWrongMovementText("First draw a card");
            return;
        }
        
        ClientTCP.sendPassTurn();
        this.myTurnText.gameObject.SetActive(false);
        this.playCardButton.enabled = false;
        this.passTurnBtn.enabled = false;
        _testGame.DrawOnceAlready = false;
    }

    public void onClickReturnToMainMenu() {
        GlobalInfo.game = null;
        GlobalInfo.otherPlayers.Clear();
        GlobalInfo.playerCards.Clear();
        GlobalInfo.isMyTurn = false;
        GlobalInfo.otherPlayersCardCount.Clear();

        GlobalInfo.playerInfo.inMatch = false;
        GlobalInfo.playerInfo.matchId = -1;
        
        newUpdateInGame = false;
        whoMadeTheUpdate = -1;
        updateMyHand = false;
        updateEnemyHand = false;
        justTurnUpdate = false;
        playWrongMoveAnimation = false;

        SceneManager.LoadScene("MainMenu");
    }
    
    private Sprite getCardSprite(int _value, Suit _suit) {
        var _spritePos = 11 * ((int) _suit) + (_value - (_value <= 7 ? 1 : 3));
        return this.sprites[_spritePos];
    }

    private void addCardToTheTable(int _value, Suit _suit, float _rotation) {
        GameObject _cardOnTableParent = GameObject.Find("CardOnTable");
        var _cardOnTable = Instantiate(this.enemyCardPrefab, new Vector3(), Quaternion.identity);
        var _cardOnTableSprite = _cardOnTable.transform.GetChild(0).gameObject;
        _cardOnTableSprite.GetComponent<SpriteRenderer>().sprite = this.getCardSprite(_value, _suit);
        _cardOnTable.transform.SetParent(_cardOnTableParent.transform);
        
        var _cardOnTableRectTransform = _cardOnTable.GetComponent<RectTransform>();
        _cardOnTableRectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
        _cardOnTableRectTransform.localPosition = new Vector3(0, 0, -_cardOnTableParent.transform.childCount);
        _cardOnTableRectTransform.localEulerAngles = new Vector3(0, 0, _rotation);
        
        var _testGame = (TestGame) GlobalInfo.game;
        _testGame.getCardOnTable().Suit = _suit;
        _testGame.getCardOnTable().Value = _value;
    }

    private void addCardToHand(int _value, Suit _suit, int _zIndex) {
        var _card = Instantiate(this.cardPrefab, new Vector3(), Quaternion.identity);
            
        var _script = _card.GetComponent<CardController>();
        _script.Value = _value;
        _script.Suit = _suit;

        var _spriteChild = _card.transform.GetChild(0).gameObject;
        _spriteChild.GetComponent<SpriteRenderer>().sprite = this.getCardSprite(_value, _suit);
        _card.transform.SetParent(this.hand.transform);
        var _rectTransform = _card.GetComponent<RectTransform>();
        _rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
        var _position = _rectTransform.position;
        _position.Set(_position.x, _position.y, -_zIndex);
    }

    private void removeCardFromMyHand(int _value, Suit _suit) {
        GameObject _toDestroy = null;
        
        foreach (Transform _child in this.hand.transform) {
            var _script = _child.GetComponent<CardController>();
            if (_script.Value == _value && _script.Suit == _suit) {
                _toDestroy = _child.gameObject;
                break;
            }
        }
        
        Destroy(_toDestroy);
        GlobalInfo.playerCards.Remove( GlobalInfo.playerCards.Single( _card => _card.SelectedToPlay ) );
        this.hand.GetComponent<HorizontalLayoutGroup>().spacing = spacing[GlobalInfo.playerCards.Count];
    }

    private void removeCardFromEnemyHand(int _enemyId, GameObject _enemyHand) {
        if (_enemyHand.transform.childCount > 0) {
            Destroy(_enemyHand.transform.GetChild(0).gameObject);
            _enemyHand.GetComponent<HorizontalLayoutGroup>().spacing = spacing[GlobalInfo.otherPlayersCardCount[_enemyId]];
        }
    }

    private void restartWrongMovementText(string _error) {
        this.wrongMovementText.gameObject.SetActive(true);
        this.wrongMovementText.text = _error;
        playWrongMoveAnimation = true;
        this.wrongMovementText.transform.localPosition = originalWrongMovementTextPosition;
        var _color = this.wrongMovementText.color;
        this.wrongMovementText.color = new Color(_color.r, _color.g, _color.b, 255);
    }
}
