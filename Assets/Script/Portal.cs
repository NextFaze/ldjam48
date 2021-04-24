using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    GameObject world;

    [SerializeField]
    Transform portalTransform;

    public float baseCameraSize = 6f;
    
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log($"{name} triggered by {other.name}");

        // TODO: This should check for the player layer ;)
        var rb = other.GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.isKinematic = true;
            rb.transform.position = portalTransform.position;
            rb.transform.SetParent(world.transform);
            rb.transform.localScale = Vector3.one;
            rb.isKinematic = false;

            Camera.main.orthographicSize = baseCameraSize * world.transform.lossyScale.x;
        }
    }
}
