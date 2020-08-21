using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Games;
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

    public GameObject modalWindow;
    
    
    private void Start() {
        
    }

    private void Update() {
        if (GameManager.Game != null) {
            if (GameManager.Game.getOtherPlayers().Count + 1 == typeOfGameToMaxPlayers(TypeOfGame.TEST))
                this.loadGameScene = true;
            
            for (var _i = 0; _i < GameManager.Game.getOtherPlayers().Count; _i++)
                this.panelOfPlayersGameObject.transform.GetChild(2 + _i).gameObject.SetActive(true);
        }
        
        if (GlobalInfo.playerInfo.inMatch && !this.panelOfPlayersGameObject.activeInHierarchy) {
            this.loading_Text.gameObject.SetActive(false);
            this.panelOfPlayersGameObject.SetActive(true);
            this.panelOfPlayersGameObject.transform.GetChild(1).gameObject.SetActive(true);
        }

        if (this.loadGameScene) {
            Debug.Log("Loading scene");
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
            Debug.LogError("Send packet to search a match.");
            ClientTCP.sendPacketSearchMatch(TypeOfGame.TEST);
            this.gamesGameObject.SetActive(false);
            this.loading_Text.gameObject.SetActive(true);

            GameManager.Game = this.constructGame(TypeOfGame.TEST);

        } else {
            // TODO Show modal window with error.
        }
        
    }

    public void onClickMus() {
        this.modalWindow.SetActive(true);
        this.modalWindow.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = this.modalWindow.transform.parent.GetComponent<RectTransform>().sizeDelta;
        this.modalWindow.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = "For now only test is playable.";
        this.modalWindow.transform.GetChild(1).transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text =
            "Non implemented game.";
    }

    public void onClickComeMierda() {
        this.modalWindow.SetActive(true);
        this.modalWindow.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = "For now only test is playable.";
        this.modalWindow.transform.GetChild(1).transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text =
            "Non implemented game.";
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
    
    private IGame constructGame(TypeOfGame _typeOfGame) {
        switch (_typeOfGame) {
            case TypeOfGame.TEST:
                return new TestGame();
            case TypeOfGame.MUS:
                return null;
            case TypeOfGame.COME_MIERDA:
                return null;
            default:
                return null;
        }
    }
    
}
