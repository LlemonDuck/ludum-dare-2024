using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
    [SerializeField]
    private Image healthBar = null;

    public void Update() {
        healthBar.fillAmount = PlayerController.instance.health;
    }
}
