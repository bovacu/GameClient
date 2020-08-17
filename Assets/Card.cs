
using UnityEngine;

public enum Suit { OROS = 0, COPAS = 1, ESPADAS = 2, BASTOS = 3 }

public class CardInfo {
    public int Value;
    public Suit Suit;
    public bool SelectedToPlay;

    public CardInfo(int _value, Suit _suit) {
        this.Value = _value;
        this.Suit = _suit;
        this.SelectedToPlay = false;
    }
}