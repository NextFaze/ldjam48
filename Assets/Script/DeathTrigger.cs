using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{

    void Awake()
    {
        if (GameManager.Instance == null)
        {
            this.enabled = false;
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        Debug.Log($"{this.name} killed player");
        GameManager.Instance.KillPlayer();
    }
}
