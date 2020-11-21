using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Reflection;
using System;
using System.Diagnostics;



public class PathManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GlobalPlanner myGlobalPlanner;
    public GameObject pathIndicator;
    //public Material defaultMat;



    
    
    public Vector2 start;
    public Vector2 goal;
    
    public int lookAhead = 4;
    public float distTolerance;
    public float waitTime;
    public int maxRetries;

    public bool visualize = false;

    public bool randomizeLoc = false;

    public List<Vector2> path = new List<Vector2>();
    public int pathLength;


    private int index;    
    private Vector3 prePos;    
    private float timer;
    private int retryCounter;


    
    private Agent self;
    private PowerLaw myMotionScript;
    private int collisionPoint;
    private List<Vector2> skipPath = new List<Vector2>();
    private List<GameObject> spawnedIndicators = new List<GameObject>();
    public bool pathFind = true;
    private Renderer myRenderer;
    private Color myColor;
    private bool astarFixCheck = false;
    // public Renderer pathRenderer;
    public float astarCooldown = 0.05f;
    private GameObject sparks;
    public float astarTimer;
    private DemoPresentation myDemo;  // FOR PRESENTATION ONLY
    void Awake()
    {
        self = gameObject.GetComponent<Agent>();
    }
    void Start()
    {
      
        //if(!randomizeLoc)
        //    validateUpdate();
        myDemo = GameObject.FindGameObjectWithTag("Manager").GetComponent<DemoPresentation>(); // FOR PRESENTATION ONLY
        if (myDemo.customAgentloc)
        {
            goal = myDemo.customGoal;
        }
        myGlobalPlanner = GameObject.FindGameObjectWithTag("Manager").GetComponent<GlobalPlanner>();
        pathIndicator = GameObject.FindGameObjectWithTag("PathIndicator");

        sparks = transform.GetChild(0).gameObject;

        myRenderer = gameObject.GetComponent<Renderer>();
        myColor = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
        myRenderer.material.SetColor("_Color", myColor);
 

        Renderer rend = pathIndicator.GetComponent<Renderer>();
        rend.material.SetColor("_Color", new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)));

        
        myMotionScript = gameObject.GetComponent<PowerLaw>();

        prePos = transform.position;
        timer = Time.time;
        astarTimer = Time.time;
       initializePath(start,goal);
        
       
        //path = updateSkipPath(); 

    }

    // Update is called once per frame


   public void initializePath(Vector2 start, Vector2 goal)
    {
        self.hasReachedDestination = false;
        self.hasReachedWaypoint = false;
        destroyAllIndicators();

        if (randomizeLoc)
        {
            start = myGlobalPlanner.findClosest(new Vector2(transform.position.x,transform.position.z));
            goal = getValidLoc();
        }

        UnityEngine.Debug.Log("START " + start + " " + goal);
        path = myGlobalPlanner.astar(start, goal);

        if (path == null)
        {
            UnityEngine.Debug.Log("Shutting Down");
            shutDownAgent();
        }
        else
        {
            path.Reverse();
            pathLength = path.Count;
            index = 0;
            self.goal = path[index];

            if (visualize)
                visualizePath(path);
        }
        this.start = start;
        this.goal = goal;
    }

   
    void Update()
    {



        //if (retryCounter >= maxRetries && astarFixCheck == true)
        //{
        //    UnityEngine.Debug.Log("SHUTDOWN");
        //    shutDownAgent();
        //}

        // FOR PRESENTATION ONLY
        if (Input.GetKeyDown(KeyCode.D))////
        {
            resetAgent();////
        }

       

        if (pathFind && !self.paused)
        {
            if (path == null)
            {
                UnityEngine.Debug.Log("Shutting Down");
                shutDownAgent();

            }

            //if (retryCounter >= maxRetries)
            //{
            //    shutDownAgent();
            //}


            // UnityEngine.Debug.Log("DETAILS " + self.hasReachedDestination + " " + self.hasReachedWaypoint);
           else if (self.hasReachedWaypoint == true && !self.hasReachedDestination)
            {
                astarFixCheck = false;
                retryCounter = 0;
                index = index + 1;
                //UnityEngine.Debug.Log("DONE WITH " + path[index - 1] + " MOVING TO " + path[index]);
                if (index >= pathLength)
                {
                    self.hasReachedDestination = true;
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                    myDemo.finishedAgents.Add(gameObject);

                   // destroyAllIndicators();
                }
                else if (index < pathLength)
                {
                    self.goal = path[index];
                    self.hasReachedWaypoint = false;
                    self.enabled = true;
                }

            }


            if (Time.time > timer + waitTime && retryCounter < maxRetries && !self.hasReachedWaypoint && !self.hasReachedDestination)
            {

                if (distance(new Vector2(prePos.x, prePos.z), new Vector2(transform.position.x, transform.position.z)) < distTolerance)
                {
                    UnityEngine.Debug.Log("Re trying " + retryCounter + " " + index);
                    retryCounter = retryCounter + 1;
                    if (index + 1 < path.Count - 1)
                    {
                        index = index + 1;
                        UnityEngine.Debug.Log("Goal Set from " + self.goal + " " + path[index]);
                        self.goal = path[index];
                    }
                }
                timer = Time.time;
                prePos = transform.position;
            }

            else if (retryCounter >= maxRetries)
                shutDownAgent();
                //{

                //    UnityEngine.Debug.Log("ATTEMPTING ATSAR FIX AFTER RETRIES "+ index+ " "+path[index]);
                //    index = index - maxRetries + 3;
                //    self.goal = path[index];
                //    UnityEngine.Debug.Log("RESCALED INDEX " + index + " " + path[index]);
                //    astarFixCheck = true;
                //    if (index < 0)
                //        shutDownAgent();
                //    else
                //    {
                //        UnityEngine.Debug.Log(index);
                //        if (path.Count > 0)
                //        {
                //            UnityEngine.Debug.Log("TRYING TO FIND PATH BETWEEN " + path[index]+" "+ goal);
                //            List<Vector2> tempPath = myGlobalPlanner.astar(path[index], goal);
                //            if (tempPath != null)
                //            {
                //                tempPath.Reverse();
                //                path.GetRange(0, index + 1);
                //                path.InsertRange(index, tempPath);
                //                retryCounter = 0;

                //            }
                //        }
                //    }


                //}



                collisionPoint = checkCollision(index);
            if (collisionPoint > 0 && !astarFixCheck && Time.time > timer + astarCooldown)
            {
                UnityEngine.Debug.Log("Goal BLOCKED");
                if (collisionPoint == path.Count - 1)
                {
                    goal = path[collisionPoint];
                  //  index = index - 1;
                    path.RemoveAt(collisionPoint);
                    pathLength = pathLength - 1;
                    //self.goal = path[index];
                }
                //else if (collisionPoint == 0)
                // {
                //     index = index + 1;
                //     self.goal = path[index];
                // }
                // else if(collisionPoint == index)
                // {
                //     if(canSpawn(new Vector3(path[index-1].x,0.5f,path[index-1].y),myGlobalPlanner.radius))
                //     {
                //         index = index - 1;
                //         self.goal = path[index];
                //         List<Vector2> tempPath = myGlobalPlanner.astar(path[index], goal);
                //     }

                // }
                else
                {
                    astarFix(collisionPoint);
                    if (path == null)
                        astarFixCheck = true;
                }

                astarTimer = Time.time;
                prePos = transform.position;
            }
        }

    }

  
    public Vector2 getValidLoc()
    {



        //Vector2 temp = new Vector2((int)UnityEngine.Random.Range(-myGlobalPlanner.gridx, myGlobalPlanner.gridx), (int)UnityEngine.Random.Range(-myGlobalPlanner.gridy, myGlobalPlanner.gridy));

        //while (!canSpawn(new Vector3(temp.x, 0.0f, temp.y), myGlobalPlanner.radius))
        //    temp = new Vector2(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-15, 15));
        int i;
        i = UnityEngine.Random.Range(0, myGlobalPlanner.gridNodes.Count);
        Vector2 loc = myGlobalPlanner.gridNodes[i];
        if (canSpawn(new Vector3(loc.x, 0.0f, loc.y), myGlobalPlanner.radius))
            return myGlobalPlanner.gridNodes[i];
        else
        {
            myGlobalPlanner.gridNodes.RemoveAt(i);
            return getValidLoc();
        }
    }
    public void shutDownAgent()
    {
        myDemo.finishedAgents.Add(gameObject); // FOR PRESENTATION ONLY

        sparks.SetActive(true);
        sparks.GetComponent<ParticleSystem>().maxParticles = UnityEngine.Random.Range(5, 15);
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
        pathFind = false;
        self.hasReachedDestination = true;
        self.hasReachedWaypoint = true;

    }
    public void resetAgent()
    {
        sparks.SetActive(false);
        gameObject.GetComponent<Agent>().enabled = true;
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", myColor);
        pathFind = true;
        Vector2 temp = getValidLoc();
        
        path = new List<Vector2>();
        initializePath(myGlobalPlanner.findClosest(new Vector2(transform.position.x,transform.position.z)), temp);
       // start = goal;
        goal = temp;
        self.hasReachedDestination = false;
        self.hasReachedWaypoint = false;

        if (path == null)
            resetAgent();

        if (visualize)
            visualizePath(path);

    }


    public float distance(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.Sqrt((pos2.x - pos1.x) * (pos2.x - pos1.x) + (pos2.y - pos1.y) * (pos2.y - pos1.y));
    }




    public void astarFix(int collisionPoint)
    {
       
        int c = collisionPoint;
       // bool found = false;
        UnityEngine.Debug.Log("THIS IS MY GOAL "+goal);


        Vector2 newTempStart;
        newTempStart = myGlobalPlanner.findClosest(new Vector2(transform.position.x, transform.position.z));

        if (newTempStart.x < -999 || newTempStart.y < -999)
            shutDownAgent();
        else
        {
            UnityEngine.Debug.Log("NEW GUY PROBLEM GUY" + newTempStart+" "+path[collisionPoint]);
            self.goal = newTempStart;
            //UnityEngine.Debug.Log(path[index]);
            path.RemoveAt(index);
            if (visualize)
                visualizePath(path);
            // path.Insert(index, newTempStart);
            //UnityEngine.Debug.Log(path[index]);
            //UnityEngine.Debug.Log("PRINTING PATH SO FAR");
            //foreach (Vector2 p in path)
            //    UnityEngine.Debug.Log(p);
        }
        if (path == null || c >=path.Count || c<0||path.Count ==0)
        {
            UnityEngine.Debug.Log("Shutting Down");
            shutDownAgent();
            UnityEngine.Debug.Log("RETURNING");
            return;
        }
        //UnityEngine.Debug.Log("PATH COUNT "+ path.Count);
        ////UnityEngine.Debug.Log("c "+c);
        ////UnityEngine.Debug.Log("pathc " + path[c]);
        ////UnityEngine.Debug.Log("x " + path[c].x);
        ////UnityEngine.Debug.Log("y " + path[c].y);
        ///

        while (c < path.Count)
        {
            UnityEngine.Debug.Log("AT C " + c +" "+path[c]+" WITH TOTAL "+path.Count);
            c = c + 1;
            if (c >= path.Count)
            {
                UnityEngine.Debug.Log("C SHUTDOWN");
                path = path.GetRange(0, collisionPoint);
                pathLength = path.Count;
                self.goal = path[index];



                if (visualize)
                    visualizePath(path);

                // shutDownAgent();
                break;
            }

            if (!canSpawn(new Vector3(path[c].x, 0.0f, path[c].y), myGlobalPlanner.radius))
                continue;

            List<Vector2> pathMod = new List<Vector2>();
            if (index > 0)
            {

                pathMod = myGlobalPlanner.astar(newTempStart, path[c]);

                if (pathMod == null || pathMod.Count == 0)
                {
                    UnityEngine.Debug.Log("Shutting Down");
                    //shutDownAgent();
                    return;
                }
                else
                    pathMod.Reverse();



                for (int i = c + 1; i < path.Count - 1; i++)
                {
                    pathMod.Add(path[i]);
                }
                path = path.GetRange(0, index + 1);
                path.InsertRange(index+1, pathMod);

                //for (int k = 1; k < pathMod.Count - 1; k++)
                //{

                //    if (distance(pathMod[k], goal) < distance(path[c], goal))
                //    {
                //        pathMod = myGlobalPlanner.astar(path[index + k], goal);
                //        if (pathMod == null)
                //            return;
                //        pathMod.Reverse();

                //        path = path.GetRange(0, index + k);
                //        path.InsertRange(index + k, pathMod);
                //        break;
                //    }

                //}

                pathLength = path.Count;
                self.goal = path[index];



                if (visualize)
                    visualizePath(path);

                break;
            }
        }

      

       

    }


    public void visualizePath(List<Vector2> drawPath)
    {
        destroyAllIndicators();

        //UnityEngine.Debug.Log("DRAWING " + drawPath.Count);
        foreach (Vector2 p in drawPath)
        {
            GameObject go = Instantiate(pathIndicator, new Vector3(p.x, 0.5f, p.y), Quaternion.identity);
            go.GetComponent<PathColor>().updateColor(myColor);
            spawnedIndicators.Add(go);
            myDemo.pathIndicators.Add(go);
            if (!myDemo.pathVisualize)
                go.GetComponent<Renderer>().enabled = false;
        }



    }

    public void destroyAllIndicators()
    {
        foreach (GameObject obj in spawnedIndicators)
        {
            Destroy(obj);
        }

        spawnedIndicators = new List<GameObject>();

    }
    public int checkCollision(int index)
    {
        if (path == null)
        {
            UnityEngine.Debug.Log("Shutting Down");
            shutDownAgent();
            return -1;
        }
        for (int i = index; i < index + lookAhead; i++)
        {
           
            if (i < 0 || i >= path.Count)
                break;

            if (!canSpawn(new Vector3(path[i].x, 0.0f, path[i].y), self.radius))
            {
                return i;
            }
        }

        return -1;
    }
    public float manhattanDistance(Vector2 pos, Vector2 goal)
    {
        return Mathf.Abs(goal.y - pos.y) + Mathf.Abs(goal.x - pos.x);
    }
    private bool canSpawn(Vector3 pos, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, radius);

        foreach (Collider collider in colliders)
        {
            GameObject go = collider.gameObject;

            if (go.layer == 8)
            {
                return false;
            }
        }

        return true;
    }
}


