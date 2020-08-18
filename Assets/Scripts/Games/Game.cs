namespace Games {
    public interface IGame {
        bool isMovementValid(int _value, Suit _suit);
        void init();
        void end();
    }
}