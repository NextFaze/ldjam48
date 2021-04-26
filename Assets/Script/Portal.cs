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

    [SerializeField]
    [Tooltip("Offset from the portal's position to move to")]
    Vector3 portalOutPosition = new Vector3(-1.0f, 0.0f, 0.0f);

    public float ScaleFactor => levelTransform.localScale.x;
    public Vector3 PortalPositionOffset => Vector3.Scale(portalPosition, portalInDirection.normalized);
    public Vector3 PortalOutPositionOffset => Vector3.Scale(portalOutPosition, portalInDirection.normalized);

    private void OnDrawGizmos() {
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + portalInDirection.normalized * transform.lossyScale.z);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position + Vector3.Scale(PortalPositionOffset, transform.lossyScale), 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position + PortalOutPositionOffset, 0.2f);
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log($"{name} triggered by {other.name}");

        if (other.tag == "Player")
        {
            var rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                var directionIn = Vector3.Dot(Vector3.right * other.transform.localScale.x, portalInDirection.normalized) > 0;

                GameManager.Instance.TeleportPlayer(this, directionIn: directionIn);
            }
        }
    }
}
