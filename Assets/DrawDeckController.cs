using System.Linq;
using Games;
using UnityEngine;

public class DrawDeckController : MonoBehaviour {
    public void OnMouseDown() {
        TestGame _testGame = (TestGame) GlobalInfo.game;

        if (GlobalInfo.playerCards.Any(_card => _testGame.isMovementValid(_card.Value, _card.Suit))) {
            Debug.Log("You still have cards to play!");
            return;
        }

        if (_testGame.DrawOnceAlready) {
            Debug.LogError("You already draw a card, if you can't play pass turn!");
            return;
        }

        ClientTCP.sendHandCardNumberUpdate(1);
        
        _testGame.DrawOnceAlready = true;
        ClientTCP.sendAskForCardsToDraw(1);
        
        while (ClientTCP.getResponseFromServer(true) != Response.RECEIVED_CARD_LIST) { }
        
        Debug.LogError("You successfully draw a card.");

        GameManager.updateMyHand = true;
    }
}
