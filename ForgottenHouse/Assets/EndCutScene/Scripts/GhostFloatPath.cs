using System.Collections;
using UnityEngine;

/// <summary>
/// GhostFloatPath.cs
/// Attach to Ghost. Floats smoothly along defined waypoints.
/// Call StartFloat() from Timeline Signal or CutsceneTrigger.
/// </summary>
public class GhostFloatPath : MonoBehaviour
{
    [Header("── Path ──")]
    public Transform[] waypoints;       // Assign WP_0 to WP_3 in order
    public float       floatSpeed    = 2f;    // Units per second
    public float       floatHeight   = 0.3f;  // How much it bobs up/down
    public float       floatFreq     = 1.5f;  // Speed of bobbing

    [Header("── Rotation ──")]
    public float       rotateSpeed   = 3f;    // How fast ghost turns to face direction

    [Header("── State ──")]
    public bool        startOnAwake  = false; // Auto start or wait for StartFloat()

    // ── Private ──
    private bool      isFloating   = false;
    private float     startY;
    private int       currentWP    = 0;

    void Start()
    {
        startY = transform.position.y;
        if (startOnAwake) StartFloat();
    }

    void Update()
    {
        if (!isFloating) return;

        // Vertical bobbing
        float newY = startY + Mathf.Sin(Time.time * floatFreq) * floatHeight;
        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }

    // ── Call this from Timeline or script ────────────────────
    public void StartFloat()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("[GhostFloat] No waypoints assigned!");
            return;
        }
        isFloating  = true;
        currentWP   = 0;
        StartCoroutine(FloatAlongPath());
    }

    public void StopFloat()
    {
        isFloating = false;
        StopAllCoroutines();
    }

    // ── Main movement coroutine ───────────────────────────────
    IEnumerator FloatAlongPath()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            currentWP = i;
            yield return StartCoroutine(MoveToWaypoint(waypoints[i]));
        }

        // Reached final waypoint — stop and face forward
        isFloating = false;
        Debug.Log("[GhostFloat] Reached gate — stopped.");
    }

    IEnumerator MoveToWaypoint(Transform target)
    {
        while (Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(target.position.x,    0, target.position.z)) > 0.1f)
        {
            // Direction to target (ignore Y)
            Vector3 dir = (target.position - transform.position);
            dir.y = 0f;
            dir.Normalize();

            // Move forward
            transform.position += dir * floatSpeed * Time.deltaTime;

            // Smoothly rotate to face direction
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    rotateSpeed * Time.deltaTime
                );
            }

            yield return null;
        }
    }

    // ── Draw path in Scene view for easy editing ──────────────
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] && waypoints[i + 1])
            {
                Gizmos.DrawLine(
                    waypoints[i].position,
                    waypoints[i + 1].position
                );
                Gizmos.DrawSphere(waypoints[i].position, 0.15f);
            }
        }
        // Draw last waypoint
        if (waypoints[waypoints.Length - 1])
            Gizmos.DrawSphere(
                waypoints[waypoints.Length - 1].position, 0.2f);
    }
}