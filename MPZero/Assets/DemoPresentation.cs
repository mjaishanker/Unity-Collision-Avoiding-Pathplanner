using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DemoPresentation : MonoBehaviour
{
    // Start is called before the first frame update
    public GlobalPlanner myGlobalPlanner;
    public List<GameObject> agents = new List<GameObject>();
    public List<GameObject> obstacles = new List<GameObject>();

    public List<GameObject> finishedAgents = new List<GameObject>();

    public List<GameObject> pathIndicators = new List<GameObject>();

    public List<GameObject> nodeIndicator = new List<GameObject>();
    public List<GameObject> levelObstacles = new List<GameObject>();
    public bool agentState = true;
    public bool agentFinishDisable = false;

    //public float startx;
    //public float starty;

    public float goalx;
    public float goaly;

   // public Vector2 customStart;
    public Vector2 customGoal;

    public bool customAgentloc = false;
    public bool pathVisualize = true;
    public bool gridVisualize = true;
    public bool obstacleVisualize = true;


    public int x, y;
    void Start()
    {
     //   DestroyAllAgents();
        DestroyAllObstacles();
        DestroyAllFinishedAgents();
        
        x = 40;
        y = 30;
        myGlobalPlanner = GameObject.FindGameObjectWithTag("Manager").GetComponent<GlobalPlanner>();
        GameObject[] arr = GameObject.FindGameObjectsWithTag("Agent");        
        foreach(GameObject go in arr)
        {

            if (!go.GetComponent<Agent>().isObstacle)
                agents.Add(go);
            else
                levelObstacles.Add(go);
        }

    }
    void OnLevelWasLoaded()
    {
        this.Start();
        DestroyAllGridIndicators();
        myGlobalPlanner.computeGrid();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DestroyAllAgents();
        if (Input.GetKeyDown(KeyCode.Return))
            DestroyAllObstacles();
        if (Input.GetKeyDown(KeyCode.RightShift))
            DestroyAllFinishedAgents();

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);

       // pathIndicators = GameObject.FindGameObjectsWithTag("PathIndicator");

    }

    public void DestroyAllAgents()
    {
        foreach (GameObject go in agents)
            Destroy(go);
        agents = new List<GameObject>();

        DestroyAllPathIndicators();

    }

    public void DestroyAllPathIndicators()
    {
        Debug.Log("destroying");
        foreach (GameObject go in pathIndicators)
            Destroy(go);

        pathIndicators = new List<GameObject>();
    }
    public void DestroyAllObstacles()
    {

        foreach (GameObject go in obstacles)
            Destroy(go);
        obstacles = new List<GameObject>();
    }

    public void DestroyAllFinishedAgents()
    {
        foreach (GameObject go in finishedAgents)
            Destroy(go);
        finishedAgents = new List<GameObject>();
    }


    public void updateXSample(string s)
    {
        x = int.Parse(s);
        if (x <= 0)
            x = 40;
    }
    public void updateYSample(string s)
    {
        y = int.Parse(s);
        if (y <= 0)
            y = 40;
    }
    public void DestroyAllGridIndicators()
    {
        foreach (GameObject go in nodeIndicator)
            Destroy(go);

        nodeIndicator = new List<GameObject>();
    }
    public void reComputeGrid()
    {
        Debug.Log("COMPUTING GRID");
        DestroyAllGridIndicators();
        myGlobalPlanner.numpointsx = x;
        myGlobalPlanner.numpointsy = y;
        myGlobalPlanner.distCalc();
        myGlobalPlanner.computeGrid();
    }

    public void setAgentValues()
    {

        //customStart = findClosest(new Vector2(startx, starty));
        customGoal = findClosest(new Vector2(goalx, goaly));

        Debug.Log("CUSTOM LOCATION SET " + customGoal);
    }





    //public void setStartX(string s)
    //{
    //    startx = float.Parse(s);
    //}
    //public void setStartY(string s)
    //{
    //    starty = float.Parse(s);
    //}
    public void setGoalX(string s)
    {
        goalx = float.Parse(s);
    }
    public void setGoalY(string s)
    {
        goaly = float.Parse(s);
    }




    public void SliderValueChanged(float value)
    {
        if (value == 0)
        {
            Debug.Log("FALSE");
            customAgentloc = false;
        }

        else
        {
            Debug.Log("TRUE");
            customAgentloc = true;
        }
    }


    public void agentWaitStateChange(float value)
    {
        if (value == 0)
        {

            agentState = false;

        }

        else
        {
            agentState = true;
        }
        toggleAgents(agentState);
    }

    public void toggleAgents(bool state)
    {
        if (agents.Count == 0)
            findAgents();
        foreach (GameObject go in agents)
        {
            go.GetComponent<Agent>().paused = state;
        }

    }

    public void disableStatusChange(float value)
    {
        if (value == 0)
        {

            agentFinishDisable = false;

        }

        else
        {
            agentFinishDisable = true;
        }
        toggleAgentsFinishState(agentFinishDisable);
    }

    public void toggleAgentsFinishState(bool state)
    {
        if (agents.Count == 0)
            findAgents();
        foreach (GameObject go in agents)
        {
            go.GetComponent<Agent>().disableStatus = state;

        }
    }
    
  
    public void findAgents()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Agent");
        foreach(GameObject g in gos)
        {
            if (!g.GetComponent<Agent>().isObstacle)
                agents.Add(g);
        }
    }


    public float distance(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.Sqrt((pos2.x - pos1.x) * (pos2.x - pos1.x) + (pos2.y - pos1.y) * (pos2.y - pos1.y));
    }
    public Vector2 findClosest(Vector2 pos)
    {
        float dist;
        float minDist = 9999;
        Vector2 loc = new Vector2(-999,-999);

        foreach(Vector2 v in myGlobalPlanner.gridNodes)
        {
            dist = distance(pos, v);
            if (dist < minDist && myGlobalPlanner.canSpawn(new Vector3(v.x,0.0f,v.y),myGlobalPlanner.radius))
            {
                loc = v;
                minDist = dist;
            }
        }
        return loc;
    }
    public void gridVisualizeStatusChange(float value)
    {
        if (value == 0)
        {

            gridVisualize = false;

        }

        else
        {
            gridVisualize = true;
        }

        toggleGridVisualization(gridVisualize);
    }
    public void pathVisualizeStatusChange(float value)
    {
        if (value == 0)
        {

            pathVisualize = false;

        }

        else
        {
            pathVisualize = true;
        }
        togglePathVisualization(pathVisualize);
    }
    public void obstacleVisualizationStatusChange(float value)
    {
        if (value == 0)
        {

            obstacleVisualize = false;

        }

        else
        {
            obstacleVisualize = true;
        }
        toggleObstacleVisualization(obstacleVisualize);
    }
    public void toggleGridVisualization(bool state)
    {
        Debug.Log("SETTING IT TO " + state);
        foreach (GameObject go in nodeIndicator)
            go.GetComponent<Renderer>().enabled = state;
    }

    public void togglePathVisualization(bool state)
    {
        foreach (GameObject go in pathIndicators)
            go.GetComponent<Renderer>().enabled = state;
    }

    public void toggleObstacleVisualization(bool state)
    {
        foreach (GameObject go in levelObstacles)
            go.GetComponent<Renderer>().enabled = state;

        foreach (GameObject go in obstacles)
            go.GetComponent<Renderer>().enabled = state;
    }
}
