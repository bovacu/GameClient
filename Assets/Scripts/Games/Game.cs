using System.Collections.Generic;

namespace Games {
    
    public struct OtherPlayer {
        public string UserName { get; }
        public int Id  { get; }
        
        public OtherPlayer(int _id, string _name) {
            this.Id = _id;
            this.UserName = _name;
        }
    }
    
    public interface IGame {
        
        bool isMovementValid(int _value, Suit _suit);

        List<CardInfo> getPlayerCards();

        List<OtherPlayer> getOtherPlayers();

        Dictionary<int, int> getCardsPerPlayer();

        bool isMyTurn();

        void setIsMyTurn(bool _isMyTurn);
        
        void init();

        void update();
        
        void end();
    }
}