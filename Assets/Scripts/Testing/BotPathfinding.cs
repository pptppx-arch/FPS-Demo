using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class BotPathfinding : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent navAgent;

    [Header("Targeting Settings")]
    public string targetTag = "Enemy";
    public float searchRange = 10f;
    public float searchAngle = 360f;
    public int rayCount = 18;
    public float rotationSpeed;

    [Header("Offset Settings")]
    public float eyeHeight = 1.0f;
    public float enemyHeight = 1.0f;
    public float rotationOffset;

    [Header("Obstacle Settings")]
    public LayerMask obstacleMask;

    // Internal state
    private GameObject currentTarget;
    public static bool hasObstacle;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        navAgent.updateRotation = false;
    }

    void Update()
    {
        // Automatically find the best target based on tag and LOS
        SearchForTarget(targetTag);

        //Rotate in the direction of the target
        RotateForTarget();

        MoveForTarget();

        // Animation control
        if (navAgent.velocity.magnitude > 0.01f)
        {
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }

    private void MoveForTarget()
    {
        navAgent.SetDestination(currentTarget.transform.position);
    }

    private void RotateForTarget()
    {
        if (currentTarget != null)
        {
            // Calculate direction and distance to target
            Vector3 targetPos = currentTarget.transform.position - new Vector3(0,enemyHeight,0);
            Vector3 direction = targetPos - transform.position;
            float distance = direction.magnitude;

            // Line of Sight Check
            Ray ray = new Ray(transform.position + Vector3.up * eyeHeight, (targetPos - transform.position).normalized);

            hasObstacle = Physics.Raycast(ray, out RaycastHit hit, distance, obstacleMask);

            // Rotate only if visible
            if (!hasObstacle && direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(Quaternion.Euler(0, rotationOffset, 0) * direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    navAgent.angularSpeed * Time.deltaTime * rotationSpeed
                );
            }
        }
    }

    private void SearchForTarget(string tag)
    {
        GameObject closestVisibleTarget = null;
        float minDistance = float.MaxValue;

        // Find all potential targets in range
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRange);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(tag))
            {
                Vector3 targetDir = hit.transform.position - transform.position;
                float distToTarget = targetDir.magnitude;

                // Check if within field of view
                float angleToTarget = Vector3.Angle(transform.forward, targetDir.normalized);

                if (angleToTarget <= searchAngle / 2f)
                {
                    // LOS Check against obstacles
                    bool blocked = Physics.Raycast(transform.position + Vector3.up * eyeHeight,
                        targetDir.normalized, distToTarget, obstacleMask);

                    if (!blocked && distToTarget < minDistance)
                    {
                        minDistance = distToTarget;
                        closestVisibleTarget = hit.gameObject;
                    }
                }
            }
        }

        currentTarget = closestVisibleTarget;
    }
}
