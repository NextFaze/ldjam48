using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public float scaleFactor = 3f;
    public GameObject scene;

    void Awake()
    {
        if (scene == null) {
            this.enabled = false;
            return;
        }

        var level = Instantiate(scene, Vector3.zero, Quaternion.identity, transform);
        level.transform.localPosition = Vector3.zero;
        level.transform.localScale /= scaleFactor;

        var sr = GetComponent<SpriteRenderer>();
        if(sr != null)
            sr.enabled = false;
    }
}
