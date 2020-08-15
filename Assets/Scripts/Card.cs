
public enum Suit { OROS = 0, COPAS = 1, ESPADAS = 2, BASTOS = 3 }
public class Card {
    public int Value;
    public Suit Suit;

    public Card(int _value, Suit _suit) {
        this.Value = _value;
        this.Suit = _suit;
    }
}