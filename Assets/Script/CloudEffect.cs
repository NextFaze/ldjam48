using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudEffect : MonoBehaviour
{
    public void OnAnimEnd()
    {
        Destroy(gameObject);
    }
}
