using UnityEngine;
using UnityEngine.AI;
[RequireComponent (typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]

public class BotWaypoints : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    public GameObject waypointReference;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
         
    }
}
