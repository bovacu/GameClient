using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    private Slider progressBar;

    // Start is called before the first frame update
    void Start() {
        progressBar = gameObject.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update() {
        progressBar.value = (float)GlobalInfo.getLoadingProgress() / 100.0f;
    }
}
