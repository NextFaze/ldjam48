using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The root transform for the level, used to scale the world")]
    Transform levelTransform;
    public Transform LevelTransform => levelTransform;

    [SerializeField]
    [Tooltip("Which way is 'in', and then scaled down")]
    Vector3 portalInDirection = new Vector3(1.0f, 0.0f, 0.0f);
    [SerializeField]
    [Tooltip("Offset from the portal's position to move to")]
    Vector3 portalPosition = new Vector3(3.0f, 0.0f, 0.0f);

    bool beenTriggered = false;

    public float ScaleFactor => levelTransform.localScale.x;
    public Vector3 PortalPositionOffset => Vector3.Scale(portalPosition, portalInDirection.normalized);

    private void OnDrawGizmos() {
        
        Gizmos.color = Color.magenta;
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
            var directionIn = Vector3.Dot(rb.velocity.normalized, portalInDirection.normalized) > 0;

            GameManager.Instance.TeleportPlayer(this, directionIn: directionIn);
        }
    }
}
