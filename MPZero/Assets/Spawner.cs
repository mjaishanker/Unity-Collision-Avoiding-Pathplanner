using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Agent1, Agent2, Agent3;
    public GameObject agentSpawn1, agentSpawn2, agentSpawn3;
    int i, id;
    private PathManager _pathScript;
    private Agent _agentScript;
    // Start is called before the first frame update
    void Start()
    {
        //for (var i = 0; i < 10; i++) 
        //{
        //    Instantiate(prefabBlock, Vector3(i * 2.0, 0, 0), Quaternion.identity);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > 15)
        {
            if (Time.time > i)
            {
                Agent aM1 = Agent1.GetComponent<Agent>();
                aM1.id = Random.Range(0, 4);
                Agent aM2 = Agent2.GetComponent<Agent>();
                aM2.id = Random.Range(5, 10);
                Agent aM3 = Agent3.GetComponent<Agent>();
                aM3.id = Random.Range(11, 15);

                //PathManager pM2 = Agent2.GetComponent<PathManager>();
                //pM2.goal.x = Random.Range(-3, 3);
                //pM2.goal.y = Random.Range(-11, -15);
                //Agent aM2 = Agent1.GetComponent<Agent>();
                //aM2.id = Random.Range(5, 10);

                //PathManager pM2 = Agent2.GetComponent<PathManager>();
                //pM2.goal.x = Random.Range(-3, 3);
                //pM2.goal.y = Random.Range(-11, -15);

                //PathManager pM3 = Agent3.GetComponent<PathManager>();
                //pM3.goal.x = Random.Range(-3, 3);
                //pM3.goal.y = Random.Range(-11, -15);

                i += 15;


                Instantiate(Agent1, agentSpawn1.transform.position, Quaternion.identity);
                Instantiate(Agent2, agentSpawn2.transform.position, Quaternion.identity);
                Instantiate(Agent3, agentSpawn3.transform.position, Quaternion.identity);
            }
        }
    }
}
