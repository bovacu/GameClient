using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class GlobalInfo {

    public struct PlayerInfo {
        public int id;
        
        public string userName;
        public string password;
        public int currencyAmount;
        public int reports;
        public bool remember;

        public bool online;
        public bool inMatch;
        public int matchId;
    }

    public struct OtherPlayer {
        public string UserName { get; }
        public int Id  { get; }
        
        public OtherPlayer(int _id, string _name) {
            this.Id = _id;
            this.UserName = _name;
        }
    }

    public enum ERROR { NONE, TIME_OUT, DIFFERENT_VERSION, ALREADY_ONLINE };

    public static PlayerInfo playerInfo;
    private static string appVersion;
    private static int loadingProgress;
    private static ERROR error;
    private static SimpleTimer timer;

    private static string ipAddress = "192.168.5.136";
    private static int port = 5555;
    private static Thread loadingThread;

    public static List<OtherPlayer> otherPlayers;
    public static Dictionary<int, int> otherPlayersCardCount;
    public static List<CardInfo> playerCards;

    public static bool isMyTurn = false;

    // Tasks to load.
    // 1. Connect to the server. (50%)
    // 2. Retreive app version.  (50%)

    private static void initClientTCPSide() {
        ClientHandlerData.initPacketListener();
        ClientTCP.initClientSocket(GlobalInfo.ipAddress, GlobalInfo.port);

        bool _coudlConnect = true;
        while (!ClientTCP.isConnectionActive()) {
            if (timer.timerEnded()) {
                _coudlConnect = false;
                break;
            }
        }

        if(!_coudlConnect) {
            GlobalInfo.error = ERROR.TIME_OUT;
            return;
        }

        // The connection established answer. DO NOT MOVE, REALLY REALLY IMPORTANT.
        ClientTCP.getResponseFromServer(true, "Welcome response");
        GlobalInfo.loadingProgress += 33;
        GlobalInfo.otherPlayers = new List<OtherPlayer>();
        GlobalInfo.playerCards = new List<CardInfo>();
        GlobalInfo.otherPlayersCardCount = new Dictionary<int, int>();

        ClientTCP.sendPacketAppVersion();
        Response _response = ClientTCP.getResponseFromServer(true, "App version");

        if(_response != Response.OK) {
            GlobalInfo.error = ERROR.DIFFERENT_VERSION;
        } else {
            GlobalInfo.appVersion = InnerFileReader.getProperty("appVersion");
            GlobalInfo.loadingProgress += 33;
        }

        if (!InnerFileReader.getProperty("remember").Equals("|") && Int32.Parse(InnerFileReader.getProperty("remember")) == 1) {

            string _user = InnerFileReader.getProperty("userName");
            string _password = InnerFileReader.getProperty("password");

            ClientTCP.sendPacketLogin(_user, _password);
            Response _couldLogin = ClientTCP.getResponseFromServer(true, "Automatic Login");

            if (_couldLogin == Response.OK) {
                GlobalInfo.playerInfo.userName = _user;
                GlobalInfo.playerInfo.password = _password;

                ClientTCP.sendPacketPlayerInfo();
                // This is just an okay from the server, because the player is already confirmed to be legit,
                // but we still need to take the answer from the server.
                ClientTCP.getResponseFromServer(true, "Automatic Player info ");

                // GlobalInfo.playerInfo.currencyAmount and GlobalInfo.playerInfo.reports are set in the corresponding handler.
                GlobalInfo.playerInfo.remember = true;
                GlobalInfo.loadingProgress = 100;
                GlobalInfo.playerInfo.online = true;
            } else {
                GlobalInfo.playerInfo.remember = false;
                GlobalInfo.error = ERROR.ALREADY_ONLINE;
            }

        } else {
            GlobalInfo.playerInfo.remember = false;
            GlobalInfo.loadingProgress = 100;
        }
    }

    public static void loadApplicationFromServer() {

        InnerFileReader.initProperties();

        GlobalInfo.playerInfo = new PlayerInfo();
        GlobalInfo.appVersion = "xx.xx";
        GlobalInfo.loadingProgress = 0;
        GlobalInfo.error = ERROR.NONE;
        GlobalInfo.timer = new SimpleTimer();
        timer.targetTime = 10;
        GlobalInfo.loadingThread = new Thread(initClientTCPSide);
        GlobalInfo.loadingThread.Start();
    }

    public static PlayerInfo getPlayerInfo() {
        return GlobalInfo.playerInfo;
    }

    public static string getAppVersion() {
        return GlobalInfo.appVersion;
    }

    public static int getLoadingProgress() {
        return GlobalInfo.loadingProgress;
    }

    public static ERROR getPossibleError() {
        return GlobalInfo.error;
    }

    public static SimpleTimer getTimer() {
        return GlobalInfo.timer;
    }

    public static void stopLoadingForError() {
        if(GlobalInfo.loadingThread.IsAlive)
            GlobalInfo.loadingThread.Interrupt();
    }

    public static CardInfo getCard(int _value, Suit _suit) {
        for(var _i = 0; _i < playerCards.Count; _i++)
            if (playerCards[_i].Value == _value && playerCards[_i].Suit == _suit)
                return playerCards[_i];

        return new CardInfo(-1, (Suit)10);
    }
}
