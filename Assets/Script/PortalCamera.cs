using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public void MoveCamera(Vector3 position) => transform.position = new Vector3(position.x, position.y, -10f);
}
