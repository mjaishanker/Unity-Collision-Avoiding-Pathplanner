using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTCISO : MonoBehaviour
{
    public GameObject[] agents;

    public float ep;
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

        if (Vector2.Distance(self.position, self.goal) > 0.5f)
        {
            computeForces();
        }

    }

    Vector2 calcTTCForces(Agent a1, Agent a2)
    {
        float r;
        float angle;
        Vector2 eta;
        Vector2 x, v;
        float a, b, c, d;
        float tau;
        Vector2 colDirection;

        // ISO
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

        //ISO
        v = a1.velocity - a2.velocity;
        Debug.Log(self.id + " " + v+ " " + x.normalized);
        v = v + eta;
        v = v - ep * x.normalized;

        v = a1.velocity - a2.velocity;
        Debug.Log(self.id + " " + v + " " + x.normalized);
        v = v - ep * x.normalized;
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

        colDirection = (x + v * tau).normalized;
        float f = Mathf.Max(self.timeHorizon - tau, 0) / tau;
        if (self.id == 1)
            Debug.Log("Will collide in " + tau);
        return colDirection * f;
    }
    void computeForces()
    {
        List<Agent> valid_neighbors = new List<Agent>();
        //GameObject[] valid_neighbor = new GameObject[agents.Length];
        foreach (GameObject go in agents)
        {
            Agent agent = go.GetComponent<Agent>();
            if (self.id != agent.id)
            {
                float dist = Vector2.Distance(self.position, agent.position);
                if (dist <= self.dhor)
                    valid_neighbors.Add(agent);

            }

        }
        //   Debug.Log(this.self.goalVelocity + " " + this.self.velocity);

        Vector2 fg = (this.self.goalVelocity - this.self.velocity) / this.self.ksi;
        // Debug.Log("Calculating fg " + fg);

        foreach (Agent n in valid_neighbors)
            fg = fg + calcTTCForces(self, n);



        if (Vector2.Distance(self.position, self.goal) > 0.5f)
        {
            if (fg.magnitude > self.maxForce)
                fg = fg.normalized * self.maxForce;
            //  Debug.Log(self.id + " " + fg);
            self.force = fg;
        }

    }
}