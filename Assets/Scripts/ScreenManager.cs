using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour {
    [SerializeField]
    private CanvasGroup gameOverScreen;
    [SerializeField]
    private CanvasGroup victoryScreen;

    private CanvasGroup currentScreen = null;

    public void Update() {
        if (false) { // TODO: CHECK QUEEN ANT HEALTH
            currentScreen = victoryScreen;
        } else if (PlayerController.instance.health <= 0) {
            currentScreen = gameOverScreen;
        } else {
            return;
        }

        Debug.Log("DEBUG: PRE SCREEN");
        if (currentScreen == null) {
            return;
        }
        Debug.Log("DEBUG: POST SCREEN");
        if (currentScreen.alpha >= 1.0f) {
            return;
        }
        Debug.Log("DEBUG: POST ALPHA");

        currentScreen.alpha += Time.deltaTime;
    }
}
