using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour {

    public Text componentLoading_Text;
    public GameObject modalWindow;
    public Text errorTitle_Text, errorDescription_Text;
    
    // Start is called before the first frame update
    void Start() {
        GlobalInfo.loadApplicationFromServer();
    }

    // Update is called once per frame
    void Update() {
        if (GlobalInfo.getPossibleError() != GlobalInfo.ERROR.NONE && !this.modalWindow.activeInHierarchy) {
            GlobalInfo.stopLoadingForError();
            this.modalWindow.SetActive(true);

            if(GlobalInfo.getPossibleError() == GlobalInfo.ERROR.TIME_OUT) {
                this.errorTitle_Text.text = "Timeout Error";
                this.errorDescription_Text.text = "Couldn't connect to server, check your internet or try later.";
            } else if (GlobalInfo.getPossibleError() == GlobalInfo.ERROR.DIFFERENT_VERSION) {
                this.errorTitle_Text.text = "Version Error";
                this.errorDescription_Text.text = "Your app version is not the current one, update application.";
            } else if (GlobalInfo.getPossibleError() == GlobalInfo.ERROR.ALREADY_ONLINE) {
                this.errorTitle_Text.text = "Already online";
                this.errorDescription_Text.text = "We have detected you are already \n logged in, can't log.";
            }
        } else {
            GlobalInfo.getTimer().update();

            if (GlobalInfo.getLoadingProgress() < 33)
                componentLoading_Text.text = "Connecting to the server...";
            else if (GlobalInfo.getLoadingProgress() >= 33 && GlobalInfo.getLoadingProgress() < 66)
                componentLoading_Text.text = "Getting app version...";
            else if(GlobalInfo.getLoadingProgress() >= 66)
                componentLoading_Text.text = "Logging in...";

            if (GlobalInfo.getLoadingProgress() == 100 && GlobalInfo.getPossibleError() == GlobalInfo.ERROR.NONE)
                SceneManager.LoadScene("MainMenu");
        }
    }

    private void OnApplicationQuit() {
        if (ClientTCP.isConnectionActive())
            ClientTCP.sendPacketQuitApp();
    }

    public void exit() {
        Application.Quit();
    }
}
