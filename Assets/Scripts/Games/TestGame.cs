using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Games {
    
    public class TestGame : MonoBehaviour, IGame {
        private CardInfo cardOnTable;
        public bool alreadyDrawnCard;
        
        private List<OtherPlayer> otherPlayers;
        private Dictionary<int, int> cardsPerPlayer;
        private List<CardInfo> playerCards;
        
        private bool myTurn;

        public int playerWhoMadeUpdateId = - 1;
        public int winnerId = -1;
        
        public bool updateEnemyHand = false;
        public bool updateMyHand = false;
        
        public bool fullGameUpdate = false;
        public bool turnGameUpdate = false;

        public bool playWrongMovementAnimation = false;

        public bool canStillDrawCards = true;
        public bool isDeckFinished = false;
        
        private readonly Dictionary<int, int> spacing = new Dictionary<int, int>() {
            {20, -1510},
            {19, -1510},
            {18, -1500},
            {17, -1500},
            {16, -1500},
            {15, -1450},
            {14, -1450},
            {13, -1450},
            {12, -1390},
            {11, -1390},
            {10, -1340},
            {9, -1340},
            {8, -1340},
            {7, -1290},
            {6, -1200},
            {5, -1165},
            {4, -1135},
            {3, -1090},
            {2, -1075},
            {1, -1075},
            {0, -1075}
        };

        private Button returnToMainMenuBtn, playCardBtn, passTurnBtn;
        private Text winTxt, myTurnTxt, wrongMovementTxt;
        private ParticleSystem confettiPS;
        private GameObject canvasGObj, handGObj, cardPrefabGObj, enemyCardPrefabGObj;

        private Sprite[] sprites;
        private Texture2D cardDeckT2D;

        public TestGame() {
            this.cardOnTable = new CardInfo(-1, (Suit)10);
            this.alreadyDrawnCard = false;
            
            this.otherPlayers = new List<OtherPlayer>();
            this.cardsPerPlayer = new Dictionary<int, int>();
            this.playerCards = new List<CardInfo>();

            this.myTurn = false;
        }
        
        public void init() {
            
        }

        public void update() {
            if (this.winnerId >= 0) {
                returnToMainMenuBtn.gameObject.SetActive(true);
                
                winTxt.gameObject.SetActive(true);
                winTxt.text = this.winnerId == GlobalInfo.playerInfo.id ? "You win!!" : "You loose...";

                if (this.winnerId == GlobalInfo.playerInfo.id) {
                    this.confettiPS.gameObject.SetActive(true);
                    this.confettiPS.Play();
                }

                playCardBtn.enabled = false;
                passTurnBtn.enabled = false;
                myTurnTxt.gameObject.SetActive(false);
                this.winnerId = -1;
                GameManager.newUpdateInGame = false;
            }
        
            if (this.playWrongMovementAnimation) {
                var _transform = wrongMovementTxt.transform.localPosition;
                this.wrongMovementTxt.transform.localPosition = new Vector3(_transform.x, _transform.y + Time.deltaTime * 50, _transform.z);
                
                var _color = this.wrongMovementTxt.color;
                _color = Color.Lerp(_color, new Color(_color.r, _color.g, _color.b, 0), 5 * Time.deltaTime);
                this.wrongMovementTxt.color = _color;

                var _deckGameObject = GameObject.Find("Deck");
                var _cardSon = _deckGameObject.transform.GetChild(0).GetComponent<RectTransform>();
                if (_color.a <= 0 || _transform.y > _deckGameObject.GetComponent<RectTransform>().localPosition.y - _cardSon.sizeDelta.y / 2f) {
                    this.playWrongMovementAnimation = false;
                    this.wrongMovementTxt.gameObject.SetActive(false);
                }
            }
            
            if (this.fullGameUpdate) {
                this.fullGameUpdate = false;
                var _random = new System.Random();
                var _rotation = (_random.Next(10, 45)) * (_random.Next(0, 2) == 0 ? 1f : -1f);
                this.addCardToTheTable(this.getCardOnTable().Value, this.getCardOnTable().Suit, _rotation);
                
                this.removeCardFromEnemyHand(GameManager.whoMadeTheUpdate, GameObject.Find("EnemyHand1"));
                
                this.myTurnTxt.gameObject.SetActive(this.isMyTurn());
                this.playCardBtn.enabled = this.isMyTurn();
                this.passTurnBtn.enabled = this.isMyTurn();
            }

            if (this.turnGameUpdate) {
                this.turnGameUpdate = false;
                this.myTurnTxt.gameObject.SetActive(this.isMyTurn());
                this.playCardBtn.enabled = this.isMyTurn();
                this.passTurnBtn.enabled = this.isMyTurn();
            }

            if (updateMyHand) {
                updateMyHand = false;

                var _zIndices = new List<float>();
                foreach (Transform _card in this.handGObj.transform)
                    _zIndices.Add(_card.GetComponent<RectTransform>().localPosition.z);

                this.addCardToHand(this.getPlayerCards().Last().Value, this.getPlayerCards().Last().Suit, (int)_zIndices.Max() + 1);
            }

            if (this.updateEnemyHand) {
                this.updateEnemyHand = false;
                var _enemyHand = GameObject.Find("EnemyHand1");
                _enemyHand.GetComponent<HorizontalLayoutGroup>().spacing = spacing[this.getCardsPerPlayer()[this.getOtherPlayers()[0].Id]];
                
                var _card = Instantiate(this.enemyCardPrefabGObj, new Vector3(), Quaternion.identity);
                _card.transform.SetParent(_enemyHand.transform);
                var _rectTransform = _card.GetComponent<RectTransform>();
                _rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
                var _position = _rectTransform.position;
                _position.Set(_position.x, _position.y, -1);
            }

            if (this.isDeckFinished) {
                this.isDeckFinished = false;
                Destroy(GameObject.Find("Deck"));
                this.canStillDrawCards = false;
            }
        }

        public void end() {
            
        }
        
        public bool isMovementValid(int _value, Suit _suit) {
            var _selectedCard = this.playerCards.Find(_card => _card.SelectedToPlay);
            return cardOnTable.Suit == _suit || cardOnTable.Value == _value;
        }

        public List<CardInfo> getPlayerCards() {
            return this.playerCards;
        }

        public List<OtherPlayer> getOtherPlayers() {
            return this.otherPlayers;
        }

        public Dictionary<int, int> getCardsPerPlayer() {
            return this.cardsPerPlayer;
        }

        public bool isMyTurn() {
            return this.myTurn;
        }

        public void setIsMyTurn(bool _isMyTurn) {
            this.myTurn = _isMyTurn;
        }

        public CardInfo getCardOnTable() {
            return this.cardOnTable;
        }

        private void addCardToTheTable(int _value, Suit _suit, float _rotation) { }

        private void addCardToHand(int _value, Suit _suit, int _zIndex) { }

        private void removeCardFromMyHand(int _value, Suit _suit) { }

        private void removeCardFromEnemyHand(int _enemyId, GameObject _enemyHand) { }

        private void restartWrongMovementText(string _error) { }
    }
}