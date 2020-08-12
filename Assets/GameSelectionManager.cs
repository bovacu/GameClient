using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectionManager : MonoBehaviour {

    public GameObject mainMenuGameObject;
    private bool alreadySelectedGame = false;
    public Text loading_Text;
    public GameObject gamesGameObject;
    public GameObject panelOfPlayersGameObject;
    private Thread waiterForPlayer;

    private void Start() {
        this.waiterForPlayer = new Thread(waitForOtherPlayers);
    }

    private void waitForOtherPlayers() {
        
    }

    public void onClickBack() {
        this.mainMenuGameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void onClickPlayTest() {
        if (!this.alreadySelectedGame) {
            this.alreadySelectedGame = true;
            ClientTCP.sendPacketSearchMatch(TypeOfGame.TEST);
            this.gamesGameObject.SetActive(false);
            this.loading_Text.gameObject.SetActive(true);
            // Waiting for response.
            var _response = ClientTCP.getResponseFromServer();
            this.loading_Text.gameObject.SetActive(false);
            this.panelOfPlayersGameObject.SetActive(true);
        } else {
            // Show modal window with error.
        }
        
    }
}
