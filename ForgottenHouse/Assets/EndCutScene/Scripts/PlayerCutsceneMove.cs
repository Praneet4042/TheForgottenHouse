using System.Collections;
using UnityEngine;

public class PlayerCutsceneMove : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float rotateSpeed = 8f;
    public float stopDistance = 0.25f;

    [Header("Animation")]
    public Animator animator;
    public string runBoolName = "isRunning";

    bool isMoving = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // Called from Timeline Signal
    public void StartRunning()
    {
        if (isMoving) return;
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned!");
            return;
        }

        StartCoroutine(MovePath());
    }

    IEnumerator MovePath()
    {
        isMoving = true;

        SetRunning(true);

        for (int i = 0; i < waypoints.Length; i++)
        {
            Transform target = waypoints[i];

            while (Vector3.Distance(transform.position, target.position) > stopDistance)
            {
                Vector3 dir = (target.position - transform.position).normalized;

                // Move
                transform.position += dir * moveSpeed * Time.deltaTime;

                // Rotate
                if (dir != Vector3.zero)
                {
                    Quaternion look = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
                }

                // Ground snap
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, 10f))
                {
                    transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                }

                yield return null;
            }

            transform.position = target.position;
        }

        // Stop at final waypoint
        SetRunning(false);
        isMoving = false;

        Debug.Log("Player reached final waypoint.");
    }

    void SetRunning(bool running)
    {
        if (animator == null) return;

        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name == runBoolName)
            {
                animator.SetBool(runBoolName, running);
                return;
            }
        }
    }
}