using UnityEngine;


//This script handles the placement of Obstacles.
public class ObstaclePlacer : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera cam;
    public GameObject obstacle;
    public GameObject highlight;
    private Ray highlightRay;
    public PlacementGrid grid;
    public SpawnChooser mySpawnChooser;
    public float spawnCheckRadius;

    public int agentCounter = 1;
   // public float respawnTime = 3.0f;

    private DemoPresentation myDemo;  // FOR PRESENTATION ONLY
    private void Start()
    {
        myDemo = GameObject.FindGameObjectWithTag("Manager").GetComponent<DemoPresentation>(); // FOR PRESENTATION ONLY
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mySpawnChooser = GameObject.FindGameObjectWithTag("Manager").GetComponent<SpawnChooser>();
        highlight = GameObject.FindGameObjectWithTag("Highlight");
        grid = gameObject.GetComponent<PlacementGrid>();

    }
    void OnLevelWasLoaded()
    {
        this.Start();
    }
    void Update()
    {

        highlightRay = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit highlightHit;

        if (Physics.Raycast(highlightRay, out highlightHit))
        {
            Vector3 loc = highlightHit.point;
            if (highlightHit.transform.gameObject.tag != "Ground")
            {
                highlight.GetComponent<Renderer>().material.color = Color.red;
                loc.y = 1.0f;
            }

            else if (highlightHit.transform.gameObject.tag == "Ground" && canSpawn(new Vector3(loc.x,loc.y+0.5f,loc.z), spawnCheckRadius))
                {              
                loc.y += 0.5f;
                highlight.GetComponent<Renderer>().material.color = Color.green;          

            }

            loc = grid.snapTogrid(loc);
            highlight.transform.position = loc;
            
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                
                Vector3 spawnpos = hit.point;
                spawnpos.y = spawnpos.y + 0.5f;
                spawnpos = grid.snapTogrid(spawnpos);
                if (hit.transform.gameObject.tag == "Ground" && canSpawn(spawnpos, spawnCheckRadius))
                {
                   
                    GameObject go =Instantiate(obstacle, spawnpos, obstacle.transform.rotation);
                    if (go.GetComponent<Agent>().isObstacle == false)
                    {
                        myDemo.agents.Add(go);
                        agentCounter = agentCounter + 1;
                        go.GetComponent<Agent>().id = agentCounter;
                        go.GetComponent<Agent>().paused = myDemo.agentState;
                        go.GetComponent<Agent>().disableStatus = myDemo.agentFinishDisable;

                        if (myDemo.customAgentloc)
                        {
                            go.GetComponent<PathManager>().start = myDemo.findClosest(new Vector2(spawnpos.x, spawnpos.z));
                            go.GetComponent<PathManager>().goal = myDemo.customGoal;
                            go.GetComponent<PathManager>().randomizeLoc = false;


                        }
                    }
                    else
                    {
                       
                        myDemo.obstacles.Add(go);
                      //  myDemo.reComputeGrid();
                    }
                }
            }

        }
    }

    private bool canSpawn(Vector3 pos, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, radius);

        foreach (Collider collider in colliders)
        {
            GameObject go = collider.gameObject;

            if (go.transform.CompareTag("Obstacle") || go.transform.CompareTag("Agent"))
            {
                return false;
            }
        }

        return true;
    }
}
