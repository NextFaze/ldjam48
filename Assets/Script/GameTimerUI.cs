using System;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class GameTimerUI : MonoBehaviour {
    
    public bool showFractions;
    TMPro.TextMeshProUGUI timerText;

    private void Start() {
        timerText = GetComponent<TMPro.TextMeshProUGUI>();

        if (GameManager.Instance == null) this.enabled = false;
    }

    private void Update() {
        var fmt = showFractions ? @"mm\:ss\:ff" : @"mm\:ss";
        timerText.text = GameManager.Instance.gameRunTime.ToString(fmt);
    }
}