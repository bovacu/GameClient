using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour {

    public GameObject   modalWindow;
    public Text         errorTitle_txt, errorDescription_txt;
    public GameObject   backgroundPanel, canvas;
    public GameObject   mainMenuGameObject;
    public GameObject   gameTypeGameObject;

    // Start is called before the first frame update
    void Start() {  }

    public void onClickPlayOnline() {
        if (!this.isOnline("First log in", "You need to log in to \n play an online match.")) 
            return;

        this.mainMenuGameObject.SetActive(false);
        this.gameTypeGameObject.SetActive(true);
    }

    public void onClickInviteFriends() {
        if (!this.isOnline("First log in", "You need to log in to \n play an online match."))
            return;

        Debug.Log("Inviting friends not implemented yet.");
    }

    public void onClickPlayPrivate() {
        if (!this.isOnline("First log in", "You need to log in to \n play an online match."))
            return;

        Debug.Log("Private playing not implemented yet.");
    }

    bool isOnline(string _errorTitle, string _errorMsg) {
        if (GlobalInfo.playerInfo.online) 
            return true;

        this.modalWindow.SetActive(true);
        this.errorTitle_txt.text = _errorTitle;
        this.errorDescription_txt.text = _errorMsg;
        this.backgroundPanel.GetComponent<RectTransform>().sizeDelta = this.canvas.GetComponent<RectTransform>().sizeDelta;
        return false;
    }

    public void closeModalWindow() {
        this.modalWindow.SetActive(false);
    }
}
