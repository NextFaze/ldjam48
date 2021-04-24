using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    public string levelName = "1ha";
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Submit")) {
            SceneManager.LoadScene(levelName);

            if (GameManager.Instance) {
                DestroyImmediate(GameManager.Instance.gameObject);
            }
        }
    }
}
