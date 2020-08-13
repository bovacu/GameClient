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
    private bool justJoinedPlayerToMatch = false;
    
    
    private void Start() {
        
    }

    private void Update() {
        if (GlobalInfo.playerInfo.inMatch && !this.panelOfPlayersGameObject.activeInHierarchy) {
            this.loading_Text.gameObject.SetActive(false);
            this.panelOfPlayersGameObject.SetActive(true);
            this.panelOfPlayersGameObject.transform.GetChild(1).gameObject.SetActive(true);
        }

        if (this.justJoinedPlayerToMatch) {
            this.justJoinedPlayerToMatch = false;
            for (var _i = 0; _i < GlobalInfo.otherPlayers.Count; _i++)
                this.panelOfPlayersGameObject.transform.GetChild(2 + _i).gameObject.SetActive(true);
        }
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
            var _response = ClientTCP.getResponseFromServer(false, "Search match");
            
            if (_response != Response.ADDED_TO_MATCH_QUEUE) {
                // TODO Show modal window with error.
                Debug.LogError($"Couldn't add to the queue. Error: {_response}");
            }

            // TODO This thread waits for all players to join the match and starts the match once it is filled.
            Thread _t = new Thread(() => {
                // TODO 1. Get response from server after joining a match.
                _response = ClientTCP.getResponseFromServer(false, "Joined match");
                if (_response != Response.OK && _response != Response.ADDED_TO_MATCH_QUEUE) {
                    // TODO Show modal window with error.
                    Debug.LogError($"Couldn't add to a match. Error: {_response}");
                    return;
                }

                // TODO 2. if currentNumOfPlayers == TypeOfGame NumOfPlayers => start game
                if (GlobalInfo.otherPlayers.Count + 1 == typeOfGameToMaxPlayers(TypeOfGame.TEST)) {
                    // TODO start game
                    this.justJoinedPlayerToMatch = true;
                    Debug.Log("The match starts!");
                }
                
                // TODO 3. while currentNumOfPlayers < TypeOfGame NumOfPlayers
                while (GlobalInfo.otherPlayers.Count + 1 < typeOfGameToMaxPlayers(TypeOfGame.TEST)) {
                    //        TODO 4. Get response from server with currentNumOfPlayers.
                    _response = ClientTCP.getResponseFromServer(false, "Player has joined the match");
                    if (_response != Response.OK) {
                        // TODO Show modal window with error.
                        Debug.LogError($"There was a problem while adding the new player, answer: {_response}");
                        return;
                    }
                    
                    //        TODO 5. Update locally the icons of joined players. Done in main thread, must be done there.
                    this.justJoinedPlayerToMatch = true;
                    
                    //        TODO 6. if currentNumOfPlayers == TypeOfGame NumOfPlayers => start game
                    if (GlobalInfo.otherPlayers.Count + 1 == typeOfGameToMaxPlayers(TypeOfGame.TEST)) {
                        // TODO start game
                        Debug.Log("The match starts!");
                        break;
                    }
                }
            });
            _t.Start();
            
        } else {
            // TODO Show modal window with error.
        }
        
    }
    
    private int typeOfGameToMaxPlayers(TypeOfGame _typeOfGame) {
        switch (_typeOfGame) {
            case TypeOfGame.TEST:
                return 2;
            case TypeOfGame.MUS:
                return 4;
            case TypeOfGame.COME_MIERDA:
                return 6;
            default:
                return 0;
        }
    }
    
}
