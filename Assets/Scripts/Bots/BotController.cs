using UnityEngine;
using UnityEngine.AI;

public class BotController : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 20f;
    public LayerMask EnemyLayer;
    private Transform currentTarget;

    [Header("Combat")]
    public float attackRange = 10f;
    public float attackCooldown = 2f; // Cooldown between attacks
    private float lastAttackTime;

    [Header("Navigation")]
    public Transform[] waypoints; // Array of waypoints for patrolling
    private int currentWaypointIndex = 0;
    public float stoppingDistance = 1f;

    [Header("Vehicle")]
    public GameObject currentVehicle;
    public Transform vehicleSeat;
    public float vehicleEnterDistance = 2f;

    //Behaviour Definitions
    public enum InfantryType { Sniper, Assault, Defenders, AntiArmor }
    public enum VehicleType { LandTransport, HeavyArmor, AirTransport, AirAttack, NavalTransport, NavalAttack }

    //State Definitions
    public enum InfantryState { Navigation, Combat, Idle, EnterVehicle }
    public InfantryState infantryState;

    public enum VehicleState { Driving, Firing, Passenger, ExitVehicle }
    public VehicleState vehicleState;

    public bool IsInfantry;

    //AI Pathfinding Requirements
    private NavMeshAgent NavAgent;

    //Firing Agent Requirements
    private RaycastFiring[] allFiringAgents;

    void Awake()
    {
        allFiringAgents = GetComponentsInChildren<RaycastFiring>();
        NavAgent = GetComponentInParent<NavMeshAgent>();
        NavAgent.stoppingDistance = stoppingDistance;
        NavAgent.updateRotation = false;
    }

    void Update()
    {
        if (IsInfantry)
        {
            switch (infantryState)
            {
                case InfantryState.Navigation:
                    Navigation();
                    break;
                case InfantryState.Combat:
                    Combat();
                    break;
                case InfantryState.Idle:
                    Idle();
                    break;
                case InfantryState.EnterVehicle:
                    EnterVehicle();
                    break;
            }
        }
        else
        {
            switch (vehicleState)
            {
                case VehicleState.Driving:
                    VehiclePathfinding();
                    break;
                case VehicleState.Firing:
                    VehicleFiring();
                    break;
                case VehicleState.Passenger:
                    RideVehicle();
                    break;
                case VehicleState.ExitVehicle:
                    ExitVehicle();
                    break;
            }
        }

    }

    public void Idle()
    {
        // If there are waypoints, start patrolling after a short idle period
        if (waypoints != null && waypoints.Length > 0)
        {
            // Could add a timer here before transitioning to Navigation
            infantryState = InfantryState.Navigation;
        }
        // Otherwise, just stand still
        NavAgent.isStopped = true;

        bool EnemyFound = ScanEnemies();
        if (EnemyFound)
        {
            infantryState = InfantryState.Combat;
        }
    }

    public void RideVehicle()
    {
        // Logic for being a passenger in a vehicle
        // The bot should follow the vehicle's movement
        if (currentVehicle != null)
        {
            transform.SetPositionAndRotation(vehicleSeat.position, vehicleSeat.rotation);
            NavAgent.enabled = false; // Disable NavMeshAgent when riding
        }
        else
        {
            // If no vehicle, something went wrong, revert to infantry navigation
            IsInfantry = true;
            infantryState = InfantryState.Navigation;
            NavAgent.enabled = true;
        }
    }

    public void VehicleFiring()
    {
        // Logic for firing from a vehicle
        // This would involve vehicle-specific weapon systems
        // For now, let's assume it's similar to infantry combat but from a vehicle
        bool EnemyFound = ScanEnemies();
        if (EnemyFound)
        {
            // Aim at target (vehicle's turret rotation)
            // Fire weapon (vehicle's weapon system)
            Debug.Log("Vehicle Firing at " + currentTarget.name);
        }
    }

    public void VehiclePathfinding()
    {
        // Logic for vehicle movement
        // This would typically involve a separate vehicle movement component
        // For simplicity, we'll use NavMeshAgent for basic vehicle movement if it's a land vehicle
        if (currentVehicle != null && NavAgent.enabled)
        {
            NavAgent.SetDestination(waypoints[currentWaypointIndex].position);
            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < stoppingDistance)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            else
            {
                // No waypoints, just idle or wait for commands
                NavAgent.isStopped = true;
            }
        }
    }

    public void Navigation()
    {
        bool EnemyFound = ScanEnemies();
        if (EnemyFound)
        {
            infantryState = InfantryState.Combat;
        }
        else
        {
            NavAgent.isStopped = false;
            if (waypoints != null && waypoints.Length > 0)
            {
                // Patrolling behavior
                NavAgent.SetDestination(waypoints[currentWaypointIndex].position);
                if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < stoppingDistance)
                {
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                }
            }
            else
            {
                // No waypoints, just idle
                infantryState = InfantryState.Idle;
            }
        }
    }

    public bool ScanEnemies()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, EnemyLayer);
        if (hits.Length > 0)
        {
            // Prioritize closest enemy
            float closestDistance = Mathf.Infinity;
            Transform closestEnemy = null;
            foreach (Collider hit in hits)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
            currentTarget = closestEnemy;
            return true;
        }
        else
        {
            currentTarget = null;
            return false;
        }

    }

    public void Combat()
    {
        ScanEnemies();

        if (currentTarget == null)
        {
            infantryState = InfantryState.Navigation;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;

        if (distanceToTarget > attackRange)
        {
            // Move towards the target
            NavAgent.isStopped = false;
            NavAgent.SetDestination(currentTarget.position);

            // Let the NavMeshAgent handle rotation while moving (smooth path following)
            // Since we disabled auto-rotation globally, we can optionally re-enable it here.
            // But for simplicity, we still manually rotate with a higher speed.
            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
            }
        }
        else
        {
            // Within attack range: stop moving, only rotate and shoot
            NavAgent.isStopped = true;

            // Rotate to face the target (fast enough to track moving enemies)
            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
            }

            // Attack logic
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Debug.Log("Attacking " + currentTarget.name);
                foreach (RaycastFiring FiringAgent in allFiringAgents)
                {
                    FiringAgent.isFiring = true;
                }
                lastAttackTime = Time.time;
            }
        }

        // If the enemy leaves the extended detection range, go back to navigation
        if (distanceToTarget > detectionRange + 5f)
        {
            infantryState = InfantryState.Navigation;
            currentTarget = null;
            NavAgent.isStopped = false;   // resume movement
        }
    }

    public void EnterVehicle()
    {
        if (currentVehicle != null && vehicleSeat != null)
        {
            // Move towards the vehicle seat
            NavAgent.isStopped = false;
            NavAgent.SetDestination(vehicleSeat.position);

            if (Vector3.Distance(transform.position, vehicleSeat.position) < vehicleEnterDistance)
            {
                // Once close enough, enter the vehicle
                IsInfantry = false;
                vehicleState = VehicleState.Driving; // Or Passenger, depending on role
                NavAgent.enabled = false; // Disable NavMeshAgent when in vehicle
                transform.SetParent(vehicleSeat); // Parent bot to vehicle seat
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }
        else
        {
            // No vehicle or seat defined, revert to navigation
            infantryState = InfantryState.Navigation;
        }
    }

    public void ExitVehicle()
    {
        if (currentVehicle != null)
        {
            // Detach from vehicle
            transform.SetParent(null);
            IsInfantry = true;
            NavAgent.enabled = true;
            // Find a safe spot to exit, for now just exit at current position
            infantryState = InfantryState.Idle; // Or Navigation
            currentVehicle = null;
            vehicleSeat = null;
        }
    }
}
