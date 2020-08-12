using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfo {
    public string userName;
    public string password;
    public int currencyAmount;
    public int reports;
    public bool remember;

    public bool online;
}

public class ClientManger : MonoBehaviour {

    // Start is called before the first frame update
    private void Awake() {
        DontDestroyOnLoad(this);
    }

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnApplicationQuit() {
        if(ClientTCP.isConnectionActive())
            ClientTCP.sendPacketQuitApp();
    }
}
