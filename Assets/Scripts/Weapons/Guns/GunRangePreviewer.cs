using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRangePreviewer : MonoBehaviour
{
    public float range;       // how far the gun reaches
    public float arcAngle;   // cone angle in degrees
    public Color previewColor = new Color(1f, 0f, 0f, 0.25f);

    void OnDrawGizmosSelected()
    {
        // Draw range sphere
        Gizmos.color = previewColor;
        Gizmos.DrawWireSphere(transform.position, range);

        // Draw arc lines
        Vector3 forward = transform.forward * range;
        Quaternion leftRot = Quaternion.Euler(0, -arcAngle, 0);
        Quaternion rightRot = Quaternion.Euler(0, arcAngle, 0);

        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, forward);
        Gizmos.DrawRay(transform.position, leftDir);
        Gizmos.DrawRay(transform.position, rightDir);
    }
}