namespace Games {
    
    public class TestGame : IGame {
        private CardInfo cardOnTable;
        public bool DrawOnceAlready;

        public TestGame(int _value, Suit _suit) {
            this.cardOnTable = new CardInfo(_value, _suit);
            this.DrawOnceAlready = false;
        }
        
        public bool isMovementValid(int _value, Suit _suit) {
            var _selectedCard = GlobalInfo.playerCards.Find(_card => _card.SelectedToPlay);
            return cardOnTable.Suit == _suit || cardOnTable.Value == _value;
        }

        public void init() {
            
        }

        public void end() {
            
        }

        public CardInfo getCardOnTable() {
            return this.cardOnTable;
        }
    }
}