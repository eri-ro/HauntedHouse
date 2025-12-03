using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPatrolSpider : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public Transform[] waypoints;
    public float minWait = 3f;
    public float maxWait = 5f;

    private Rigidbody m_RigidBody;
    private Animator m_Animator; 
    
    int m_CurrentWaypointIndex;
    bool isWaiting = false;

    void Start ()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_Animator = GetComponentInChildren<Animator>();

        if (m_RigidBody == null)
        {
            Debug.LogWarning($"{name}: No Rigidbody found — disabling WaypointPatrol.");
            enabled = false;
            return;
        }
        if (m_Animator == null)
        {
             Debug.LogWarning($"{name}: No Animator found in children.");
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning($"{name}: No waypoints assigned — disabling WaypointPatrol.");
            enabled = false;
            return;
        }
        m_CurrentWaypointIndex = 0;
    }

    void FixedUpdate ()
    {
        if (m_Animator != null)
        {
            m_Animator.SetBool("isWalking", !isWaiting);
        }

        if (isWaiting) return;

        Transform currentWaypoint = waypoints[m_CurrentWaypointIndex];
        Vector3 currentToTarget = currentWaypoint.position - m_RigidBody.position;

        if (currentToTarget.magnitude < 0.1f)
        {
            m_RigidBody.MovePosition(currentWaypoint.position);
            StartCoroutine(WaitAndAdvance());
            return;
        }
        Quaternion forwardRotation = Quaternion.LookRotation(currentToTarget);
        m_RigidBody.MoveRotation(forwardRotation);
        m_RigidBody.MovePosition(m_RigidBody.position + currentToTarget.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    IEnumerator WaitAndAdvance()
    {
        isWaiting = true;
        float wait = Random.Range(minWait, maxWait);
        yield return new WaitForSeconds(wait);
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        isWaiting = false;
    }
}