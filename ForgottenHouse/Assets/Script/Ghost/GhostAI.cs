using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    private bool _aiActive = true;

    public void SetActive(bool value)
    {
        _aiActive = value;
        GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = !value;
    }
    public Transform[] waypoints;
    public float chaseSpeed = 8f;
    public float roamSpeed = 3f;

    NavMeshAgent agent;
    Transform player;
    int currentWaypoint = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = roamSpeed;
        GoToNextWaypoint(); // start roaming immediately
    }

    void Update()
    {
        if (!_aiActive) return;
        bool lanternOn = LanternToggle.instance.isOn;

        if (lanternOn)
        {
            // Lantern ON = danger, ghost chases
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            // Lantern OFF = safe, ghost roams
            agent.speed = roamSpeed;
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GoToNextWaypoint();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.KillPlayer("The ghost got you...");
        }
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;
        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }
}