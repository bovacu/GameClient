using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Image background;
    public GameObject canvas;
    public Text[] otherPlayersNames;
    public Text yourName;

    private void Start()  {
        this.background.GetComponent<RectTransform>().sizeDelta = this.canvas.GetComponent<RectTransform>().sizeDelta;
        this.yourName.text = GlobalInfo.playerInfo.userName;
        for (var _i = 0; _i < GlobalInfo.otherPlayers.Count; _i++)
            this.otherPlayersNames[_i].text = GlobalInfo.otherPlayers[_i].UserName;
    }
    
    private void Update() {
        
    }
}
