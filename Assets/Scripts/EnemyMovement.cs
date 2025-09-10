using UnityEngine;
using UnityEngine.AI;
public class EnemyMovement : MonoBehaviour
{

    Transform target;
    public GameObject[] waypoints;
    public float speed = 2f;
    public float reachDistance = 1f;
    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = waypoints[0].transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            // Move towards the target waypoint
            //transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            agent.SetDestination(target.position);
            
        }
    }
        
    
}
