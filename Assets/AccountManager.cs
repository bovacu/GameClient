using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountManager : MonoBehaviour {

    public InputField  user_Input;
    public InputField  password_Input;
    public InputField  password2_Input;
    public Button      enter_Btn;
    public Text        errorMsg_Text;
    public Text        appVersion_Text;
    public Toggle      remember_Toggle;
    public GameObject  profile_GameObject, middleSide_GameObject, rightSide_GameObject;
    public Text        welcomeUser_Text, currencyAmount_Text;

    // Registration = 0 | Login = 1.
    private int mode = 0;
    private Vector3 enterBtnOriginalPos, errorMsgOriginalPos;

    void Start() {
        this.profile_GameObject.SetActive(false);
        this.user_Input.gameObject.SetActive(false);
        this.password_Input.gameObject.SetActive(false);
        this.password2_Input.gameObject.SetActive(false);
        this.enter_Btn.gameObject.SetActive(false);
        this.enterBtnOriginalPos = this.enter_Btn.gameObject.transform.position;
        this.errorMsgOriginalPos = this.errorMsg_Text.gameObject.transform.position;
        this.errorMsg_Text.gameObject.SetActive(false);
        this.remember_Toggle.gameObject.SetActive(false);

        this.appVersion_Text.text = "Version: " + GlobalInfo.getAppVersion();

        if(GlobalInfo.playerInfo.remember) {
            this.gameObject.SetActive(false);
            this.profile_GameObject.SetActive(true);

            this.welcomeUser_Text.text = "Welcome " + GlobalInfo.playerInfo.userName + "!";
            this.currencyAmount_Text.text = "Current money: " + GlobalInfo.playerInfo.currencyAmount;
        }
    }

    void Update() {
        
    }

    public void showRegistration() {
        this.mode = 0;
        this.user_Input.gameObject.SetActive(true);
        this.password_Input.gameObject.SetActive(true);
        this.password2_Input.gameObject.SetActive(true);
        this.enter_Btn.gameObject.SetActive(true);
        this.remember_Toggle.gameObject.SetActive(true);
        this.enter_Btn.gameObject.transform.position = this.enterBtnOriginalPos;
        this.errorMsg_Text.gameObject.transform.position = this.errorMsgOriginalPos;
        this.errorMsg_Text.gameObject.SetActive(false);
        this.remember_Toggle.gameObject.SetActive(false);
    }

    public void showLogin() {
        this.mode = 1;
        this.user_Input.gameObject.SetActive(true);
        this.password_Input.gameObject.SetActive(true);
        this.password2_Input.gameObject.SetActive(false);
        this.remember_Toggle.gameObject.SetActive(false);
        this.enter_Btn.gameObject.SetActive(true);
        this.remember_Toggle.gameObject.SetActive(true);

        var _posEnter = this.enter_Btn.gameObject.transform.position;
        _posEnter.y = this.password2_Input.gameObject.transform.position.y;
        this.enter_Btn.gameObject.transform.position = _posEnter;

        this.errorMsg_Text.gameObject.SetActive(false);
    }

    public void executeRegisterOrLogin() {
        if (this.mode == 0)
            this.registerAccount();
        else
            this.login();
    }

    private void login() {
        ClientTCP.sendPacketLogin(this.user_Input.text, this.password_Input.text);
        Response _response = ClientTCP.getResponseFromServer();

        if (this.user_Input.text == string.Empty || this.password_Input.text == string.Empty || _response == Response.LOGIN_WRONG_USER_OR_PASSWORD_ERROR) {
            this.errorMsg_Text.text = "User or/and password incorrect.";
            this.errorMsg_Text.color = Color.red;
            this.errorMsg_Text.gameObject.SetActive(true);
            return;
        }

        if(_response == Response.ERROR) {
            this.errorMsg_Text.text = "Internal app error, contact developers.";
            this.errorMsg_Text.color = Color.red;
            this.errorMsg_Text.gameObject.SetActive(true);
            return;
        }

        ClientTCP.sendPacketPlayerInfo();

        // This is just an okay from the server, because the player is already confirmed to be legit,
        // but we still need to take the answer from the server.
        ClientTCP.getResponseFromServer();

        GlobalInfo.playerInfo.userName = this.user_Input.text;
        GlobalInfo.playerInfo.password = this.password_Input.text;
        GlobalInfo.playerInfo.online = true;

        this.welcomeUser_Text.text = "Welcome " + GlobalInfo.playerInfo.userName + "!";
        this.currencyAmount_Text.text = "Current money: " + GlobalInfo.playerInfo.currencyAmount;

        InnerFileReader.setProperty("remember", this.remember_Toggle.isOn ? "1" : "0");

        if(Int32.Parse(InnerFileReader.getProperty("remember")) == 1) {
            InnerFileReader.setProperty("userName", this.user_Input.text);
            InnerFileReader.setProperty("password", this.password_Input.text);
        }

        this.gameObject.SetActive(false);
        this.profile_GameObject.SetActive(true);
    }

    private void registerAccount() {
        if (this.user_Input.text == string.Empty || this.password_Input.text == string.Empty) {
            this.errorMsg_Text.text = "Both user and password must be filled.";
            this.errorMsg_Text.color = Color.red;
            this.errorMsg_Text.gameObject.SetActive(true);
            return;
        }

        if(!this.password_Input.text.Equals(this.password2_Input.text)) {
            this.errorMsg_Text.text = "Passwords must match.";
            this.errorMsg_Text.color = Color.red;
            this.errorMsg_Text.gameObject.SetActive(true);
            return;
        }

        ClientTCP.sendPacketRegisterAccount(this.user_Input.text, this.password_Input.text); 
        Response _response = ClientTCP.getResponseFromServer();
        
        if(_response == Response.REGISTER_USER_DUPLICATED_ERROR) {
            this.errorMsg_Text.text = "This user name is already in use";
            this.errorMsg_Text.color = Color.red;
            this.errorMsg_Text.gameObject.SetActive(true);
            return;
        }

        if (_response == Response.ERROR) {
            this.errorMsg_Text.text = "Internal app error, contact developers.";
            this.errorMsg_Text.color = Color.red;
            this.errorMsg_Text.gameObject.SetActive(true);
            return;
        }

        this.errorMsg_Text.gameObject.SetActive(true);
        this.errorMsg_Text.color = Color.green;
        this.errorMsg_Text.text = "Registration successful!";
    }

    public void onCloseApplication() {
        Application.Quit();
    }

    private void OnApplicationQuit() {
        Debug.Log("closing");
        if (ClientTCP.isConnectionActive() && GlobalInfo.playerInfo.online)
            ClientTCP.sendPacketQuitApp();
    }
}
