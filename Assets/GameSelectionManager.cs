using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSelectionManager : MonoBehaviour {

    public GameObject mainMenuGameObject;
    private bool alreadySelectedGame = false;
    public Text loading_Text;
    public GameObject gamesGameObject;
    public GameObject panelOfPlayersGameObject;
    private bool justJoinedPlayerToMatch = false;
    private bool loadGameScene = false;
    
    
    private void Start() {
        
    }

    private void Update() {
        if (GlobalInfo.otherPlayers.Count + 1 == typeOfGameToMaxPlayers(TypeOfGame.TEST))
            this.loadGameScene = true;
        
        if (GlobalInfo.playerInfo.inMatch && !this.panelOfPlayersGameObject.activeInHierarchy) {
            this.loading_Text.gameObject.SetActive(false);
            this.panelOfPlayersGameObject.SetActive(true);
            this.panelOfPlayersGameObject.transform.GetChild(1).gameObject.SetActive(true);
        }

        for (var _i = 0; _i < GlobalInfo.otherPlayers.Count; _i++)
            this.panelOfPlayersGameObject.transform.GetChild(2 + _i).gameObject.SetActive(true);

        if (this.loadGameScene) {
            SceneManager.LoadScene("TwoPlayersGameScene");
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
            
            _response = ClientTCP.getResponseFromServer(false, "Joined match");
            if (_response != Response.OK && _response != Response.ADDED_TO_MATCH_QUEUE) {
                if (_response == Response.LOAD_MATCH_SCENE) {
                    this.loadGameScene = true;
                    return;
                }
                Debug.LogError($"Couldn't add to a match. Error: {_response}");
                return;
            }

            // TODO 2. if currentNumOfPlayers == TypeOfGame NumOfPlayers => start game
            if (GlobalInfo.otherPlayers.Count + 1 == typeOfGameToMaxPlayers(TypeOfGame.TEST)) {
                this.justJoinedPlayerToMatch = true;
                this.loadGameScene = true;
                Debug.Log("The match starts!");
            }
            
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
