using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class BotAgent : MonoBehaviour
{
    public int botID;
    public GroupName currentGroup;
    public int squadID; // Sub-group identifier (2-4 bots)
    private NavMeshAgent agent;

    void Awake() => agent = GetComponent<NavMeshAgent>();

    public void MoveTo(Vector3 target)
    {
        if (agent != null && agent.isOnNavMesh) agent.SetDestination(target);
    }
}
