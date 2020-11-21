using System.Collections.Generic;
using UnityEngine;


//This script handles the Powerlaw for each individual agent
//Powerlaw collision avoidance is the technique used by the agents while moving from one way point to another
public class PowerLaw : MonoBehaviour
{
    public GameObject[] agents;

    public float ep;
    public float m;
    public float k;
    public float tau0;
    public float e = 2.718f;
    public float vu;
    private float ep2;
    public Agent self;

    private Vector2 currForce;

    void Start()
    {
        agents = GameObject.FindGameObjectsWithTag("Agent");
        self = gameObject.GetComponent<Agent>();
        ep2 = ep * ep;
    }

    void Update()
    {
        agents = GameObject.FindGameObjectsWithTag("Agent");

        if (!self.hasReachedWaypoint)
        {
            computeForces();
        }

    }

    public Vector2 calcTTCForces(Agent a1, Agent a2)
    {
        float r;
        float angle;
        Vector2 eta;
        Vector2 x, v;
        float a, b, c, d; 
        float tau;
        Vector2 forceLawRep;
        int k = 15, t0 = 3, m = 2;

        //ISO
        r = vu * Mathf.Sqrt(Random.Range(0, 1));
        angle = Random.Range(0, 2) * 3.14f;
        eta = new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle));


        x = a1.position - a2.position;
        r = a1.radius + a2.radius;
        c = Vector2.Dot(x, x) - r * r;


        if (c < 0)
        {
            Debug.Log("COLLISION!");
            return new Vector2(0f, 0f);
        }

        v = a1.velocity - a2.velocity;
       
        b = Vector2.Dot(x, v) - ep * r;
        if (b > 0)
            return new Vector2(0f, 0f);


        a = Vector2.Dot(v, v) - ep * ep;
        d = (b * b) - a * c;
        if (d <= 0)
            return new Vector2(0f, 0f);

        tau = c / (-b + Mathf.Sqrt(d));
      
        if (tau < 0)
            return new Vector2(0f, 0f);

     
        Vector2 tempV = new Vector2(v[0] * tau, v[1] * tau);
        forceLawRep = (k * Mathf.Pow(e, -tau / tau0) /
            (Mathf.Pow(tau, m + 1))) *
            (m + tau / t0) *
            ((x + tempV) /
            Mathf.Sqrt(d));

    
        return forceLawRep;
    }
    public void computeForces()
    {
        List<Agent> valid_neighbors = new List<Agent>();
      
        foreach (GameObject go in agents)
        {
            Agent agent = go.GetComponent<Agent>();
            // Debug.Log("AGENT DETAILS " + self.id + " " + agent.id);
            if (agent.disableStatus && agent.hasReachedDestination)
                continue;
            if (self.id != agent.id)
            {

                    float dist = Vector2.Distance(self.position, agent.position);
                    if (dist <= self.dhor)
                        valid_neighbors.Add(agent);

            }

        }
     
        Vector2 fg = (this.self.goalVelocity - this.self.velocity) / this.self.ksi;

        foreach (Agent n in valid_neighbors)
            fg = fg + calcTTCForces(self, n);

        if (Vector2.Distance(self.position, self.goal) > 0.5f)
        {
            if (fg.magnitude > self.maxForce)
                fg = fg.normalized * self.maxForce;
     
            self.force = fg;
        }

    }
}
