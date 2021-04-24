using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class DeathCountUI : MonoBehaviour {
    
    TMPro.TextMeshProUGUI text;

    private void Start() {
        text = GetComponent<TMPro.TextMeshProUGUI>();

        if (GameManager.Instance == null) this.enabled = false;
    }

    private void Update() {
        text.text = GameManager.Instance.DeathCount.ToString();
    }
}
