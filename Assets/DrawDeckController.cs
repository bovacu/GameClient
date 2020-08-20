using System.Linq;
using Games;
using UnityEngine;
using UnityEngine.UI;

public class DrawDeckController : MonoBehaviour {

    public Text wrongMovementText;
    
    public void OnMouseDown() {
        TestGame _testGame = (TestGame) GlobalInfo.game;

        if (GlobalInfo.playerCards.Any(_card => _testGame.isMovementValid(_card.Value, _card.Suit))) {
            this.wrongMovementText.gameObject.SetActive(true);
            this.wrongMovementText.text = "You still have cards to play!";
            GameManager.playWrongMoveAnimation = true;
            this.wrongMovementText.transform.localPosition = GameManager.originalWrongMovementTextPosition;
            var _color = this.wrongMovementText.color;
            this.wrongMovementText.color = new Color(_color.r, _color.g, _color.b, 255);
            
            return;
        }

        if (_testGame.DrawOnceAlready) {
            this.wrongMovementText.gameObject.SetActive(true);
            this.wrongMovementText.text = "You already draw a card, if you can't play pass turn!";
            GameManager.playWrongMoveAnimation = true;
            this.wrongMovementText.transform.localPosition = GameManager.originalWrongMovementTextPosition;
            var _color = this.wrongMovementText.color;
            this.wrongMovementText.color = new Color(_color.r, _color.g, _color.b, 255);
            
            return;
        }

        ClientTCP.sendHandCardNumberUpdate(1);
        
        _testGame.DrawOnceAlready = true;
        ClientTCP.sendAskForCardsToDraw(1);
        
        while (ClientTCP.getResponseFromServer(true) != Response.RECEIVED_CARD_LIST) { }

        GameManager.updateMyHand = true;
    }
}
