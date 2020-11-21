using UnityEngine;


//Agent class that holds all the relevant information about the agent.
//This class is also reponsible for moving the agent based of it's current velocity.
public class Agent : MonoBehaviour
{

    public bool isObstacle;
    public float fixedTimeUpdate = 0.05f;
    public int id;
    public float radius = 0.5f;
    
    public Vector2 position;
    public Vector2 goal;

    public float maxForce;
    public float prefSpeed;
    public Vector2 goalVelocity;
    public float goalTolerance = 0.1f;

    public float timeHorizon;
    public float dhor;

    public float ksi; // relaxation time

    public Vector2 velocity;
    public Vector2 force;

    public bool paused = false;

    public bool hasReachedWaypoint = false;
    public bool hasReachedDestination = false;

    private CustomFixedUpdate m_FixedUpdate;

    public bool disableStatus = false;
    
    
    private void Start()
    {
        m_FixedUpdate = new CustomFixedUpdate(0.05f, MyFixedUpdate);
        position = new Vector2(transform.position.x, transform.position.z);
      
    }


    private void Update()
    {
       if(!isObstacle && !hasReachedDestination && !hasReachedWaypoint &&!paused)
            m_FixedUpdate.Update(fixedTimeUpdate);
    }
    void MyFixedUpdate()
    {

        float mag = Mathf.Sqrt(force.x * force.x + force.y * force.y);
        if (mag > maxForce)
        {
            Vector2 unitForce = force / mag;
            force = force * unitForce;
        }
        position = new Vector2(transform.position.x, transform.position.z);
        velocity = force * 0.01f;
        transform.position = transform.position + new Vector3(velocity.x, 0f, velocity.y);
        if (Vector2.Distance(position, goal) <= goalTolerance)
        {
          
            hasReachedWaypoint = true;
           // gameObject.GetComponent<Agent>().enabled = false;

        }
        goalVelocity = goal - position;
        goalVelocity = goalVelocity.normalized * prefSpeed;

    }
   
}
