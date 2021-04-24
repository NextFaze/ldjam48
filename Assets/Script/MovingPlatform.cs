using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    Vector3 startPositionOffset = new Vector3(3.0f, 0.0f, 0.0f);
    [SerializeField]
    Vector3 endPositionOffset = new Vector3(-3.0f, 0.0f, 0.0f);
    [SerializeField]
    float speed = 5f;

    bool movingToEnd = true;
    Vector3 startPosition;
    Vector3 centerPosition;
    Vector3 endPosition;

    Vector3 gizmoPosition;

    // Start is called before the first frame update
    void Start()
    {
        gizmoPosition = transform.position;
        centerPosition = transform.localPosition;
        startPosition = transform.localPosition + startPositionOffset; 
        endPosition = transform.localPosition + endPositionOffset; 
        transform.localPosition = startPosition;
    }

    private void OnDrawGizmos() {
        var pos = transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + (startPositionOffset * transform.lossyScale.z));
        Gizmos.DrawLine(pos, pos + (endPositionOffset * transform.lossyScale.z));
    }

    void Update()
    {
        // Move our position a step closer to the target.
        float step = speed * Time.deltaTime; // calculate distance to move
        var targetPosition = movingToEnd ? centerPosition + endPositionOffset : centerPosition + startPositionOffset;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, step);

        // Check if the position of the cube and sphere are approximately equal.
        if (Vector3.Distance(transform.localPosition, targetPosition) == 0.0f)
        {
            Debug.Log($"{transform.localPosition}, {targetPosition}");
            movingToEnd = !movingToEnd;
        }
    }
}
