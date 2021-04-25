using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public GameObject scene;

    void Awake()
    {
        var level = Instantiate(scene, transform);
        level.transform.localScale = this.transform.localScale;
    }
}
