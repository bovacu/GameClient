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

            // TODO This thread waits for all players to join the match and starts the match once it is filled.
            Thread _t = new Thread(() => {
                // TODO 1. Get response from server with currentNumOfPlayers.
                // TODO 2. if currentNumOfPlayers == TypeOfGame NumOfPlayers => start game
                // TODO 3. while currentNumOfPlayers < TypeOfGame NumOfPlayers
                //        TODO 4. Get response from server with currentNumOfPlayers.
                //        TODO 5. Update locally the icons of joined players.
                //        TODO 6. if currentNumOfPlayers == TypeOfGame NumOfPlayers => start game
                return;
            });
            _t.Start();
            
        } else {
            // TODO Show modal window with error.
        }
        
    }
}
