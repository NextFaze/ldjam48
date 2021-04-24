using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The root transform for the level, used to scale the world")]
    Transform levelTransform;

    [SerializeField]
    [Tooltip("Which way is 'in', and then scaled down")]
    Vector3 portalInDirection = new Vector3(1.0f, 0.0f, 0.0f);
    [SerializeField]
    [Tooltip("Offset from the portal's position to move to")]
    Vector3 portalPosition = new Vector3(3.0f, 0.0f, 0.0f);

    bool beenTriggered = false;

    private void OnDrawGizmos() {
        
        Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(transform.position, .2f);
        Gizmos.DrawLine(transform.position, transform.position + portalInDirection.normalized * transform.lossyScale.z);
    }

    private void OnTriggerExit2D(Collider2D other) {
        beenTriggered = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log($"{name} triggered by {other.name}");

        // TODO: This should check for the player layer ;)
        var rb = other.GetComponent<Rigidbody2D>();
        if (!beenTriggered && rb != null) {
            beenTriggered = true;
            var scaleFactor = levelTransform.localScale.x;
            var portalPositionOffset = Vector3.Scale(portalPosition,  portalInDirection.normalized);
            if (Vector3.Dot(rb.velocity.normalized, portalInDirection.normalized) < 0) {
                scaleFactor = 1/scaleFactor;
                portalPositionOffset *= -1;
            }
            Debug.Log($"Scaling world by {scaleFactor}");
            
            // Move this to either a DI thing, or game message
            var gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
            gameManager.gameObject.transform.localScale /= scaleFactor;

            // Replace with animated to new position
            rb.isKinematic = true;
            rb.transform.position = transform.position + portalPositionOffset;
            rb.isKinematic = false;
        }
    }
}
