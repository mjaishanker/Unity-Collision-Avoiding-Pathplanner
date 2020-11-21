using System.Collections.Generic;
using UnityEngine;




//This script provides the functionality for a global planner. It is implemented using A-star.


public class Node
{
    public Vector2 node;
    public float f;
    public float g;
    public float h;
}
public class Parent 
{

    
    public Vector2 parentNode;
    public Vector2 self;

    public Parent(Vector2 parent,Vector2 child)
    {
        parentNode = parent;
        self = child;
    }

}

public class GlobalPlanner : MonoBehaviour
{
    public Vector2 start1;
    public Vector2 end1;
    public float sampleScaleFactor = 1.0f;
    public float gridx; //Unity Points
    public float gridy;

    public int numpointsx; // number of points to sample
    public int numpointsy;

    public float radius;
    public bool visualize = false;
    public List<Vector2> gridNodes = new List<Vector2>();
    private PriorityQueue openList = new PriorityQueue();
    private List<Vector2> closedList = new List<Vector2>();
    private List<Parent> tracer = new List<Parent>();
    private DemoPresentation myDemo;

    private float xdist;
    private float ydist;
    private float maxDist;
    private float myRadius;
    public GameObject indicator;
    
    private void Awake()
    {
        myDemo = gameObject.GetComponent<DemoPresentation>();
        distCalc();
        computeGrid();
    }
 public void Start()
    {
        myRadius = radius;

        radius = radius * sampleScaleFactor;
        myDemo = gameObject.GetComponent<DemoPresentation>();
    }

    public void OnLevelWasLoaded()
    {
        this.Awake();
        this.Start();
        
    }
    public void computeGrid()
    {
        gridNodes = new List<Vector2>();
        for (int i = 0; i < numpointsx; i = i + 1)
        {
            for (int j = 0; j < numpointsy; j = j + 1)
            {

                float x = -gridx + i * xdist;
                float y = -gridy + j * ydist;
                if (canSpawn(new Vector3(x, 0.5f, y), radius))
                {
                    if (visualize)
                    {
                        GameObject go = Instantiate(indicator, new Vector3(x, 0.5f, y), Quaternion.identity);
                        myDemo.nodeIndicator.Add(go);
                        if (!myDemo.gridVisualize)
                            go.GetComponent<Renderer>().enabled = false;
                    }
                    gridNodes.Add(new Vector2(x, y));
                }
            }
        }
    }

    public void setRadius(string s)
    {
        radius = myRadius;
        float f = float.Parse(s);

        if (f > 0)
            radius = radius * f;
    }
    public Vector2 findClosest(Vector2 pos)
    {
        float dist;
        float minDist = 9999;
        Vector2 loc = new Vector2(-999, -999);

        foreach (Vector2 v in gridNodes)
        {

            if (!canSpawn(v, radius))
                continue;
            
            dist = distance(pos, v);
            if (dist < minDist)
            {
                loc = v;
                minDist = dist;
            }
        }
        return loc;
    }
    public void distCalc()
    {
        xdist = 2 * gridx / numpointsx;
        ydist = 2 * gridy / numpointsy;

        maxDist = Mathf.Sqrt(xdist * xdist + ydist * ydist);
    }

    public float distance(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.Sqrt((pos2.x-pos1.x)*(pos2.x-pos1.x) + (pos2.y-pos1.y)*(pos2.y-pos1.y));
    }

    public float manhattanDistance(Vector2 pos, Vector2 goal)
    {
        return Mathf.Abs(goal.y - pos.y) + Mathf.Abs(goal.x - pos.x);
    }

    public bool canSpawn(Vector3 pos, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, radius);
        foreach (Collider collider in colliders)
        {
            GameObject go = collider.gameObject;
            if (go.layer == 8) //if colliding object is an obstacle
                return false;
        }

        return true;
    }

    public List<Vector2> astar(Vector2 start, Vector2 goal)
    {
        Vector2[,] children = new Vector2[3, 3];
      
        float dist;
        float mandist;

        Node n = new Node();
        openList = new PriorityQueue();
        openList.myList = new List<Node>();
        n.node = start;
        n.g = 0;
        n.h = manhattanDistance(start, goal);
        n.f = n.g + n.h;
        openList.myList.Add(n);
        closedList = new List<Vector2>();


        tracer = new List<Parent>();
        while (openList.myList.Count > 0)
        {
            //Utils.ClearLogConsole();
       //     Debug.Log("GLOBAL PLANNER ");
            Node currNode = openList.pop();
            closedList.Add(currNode.node);
            if (currNode.node == goal)
                break;

            
            children = getChildren(currNode.node.x,currNode.node.y);
            
            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == 1 && j == 1 || children[i,j].x == -999.0f || children[i, j].y == -999.0f)
                        continue;
        
                    if (!closedList.Contains(children[i,j]) && !openList.doesContain(children[i, j]))
                    {
                        
                        dist = distance(children[i, j], currNode.node);
                        mandist = manhattanDistance(children[i, j], goal);
                        Node n1 = new Node();
                        n1.node = children[i, j];
                        n1.g = currNode.g + dist;
                        n1.h = mandist;
                        n1.f = n1.g + n1.h;
                        
                        openList.myList.Add(n1);
                        tracer.Add(new Parent(currNode.node, n1.node));
                       
                    }
                    else if(openList.doesContain(children[i, j]) && distance(children[i, j], currNode.node) +currNode.g < openList.findItem(children[i, j]).g)
                    {
                        openList.findRemove(children[i, j]);
                        dist = distance(children[i, j], currNode.node);
                        mandist = manhattanDistance(children[i, j], goal);
                        Node n2 = new Node();
                        n2.node = children[i, j];
                        n2.g = currNode.g + dist;
                        n2.h = mandist;
                        n2.f = n2.g + n2.h;
                        openList.myList.Add(n2);
                        Vector2 temp = getParent(n2.node);
                        removeParent(n2.node);
                        tracer.Add(new Parent(currNode.node, n2.node));

                    }
                }
            }
           
        }



        List<Vector2> path = new List<Vector2>();
        Vector2 path_node = goal;
        path.Add(goal);
        int index;
        index = closedList.Count-1;
        int counter = 0 ;


        while (path_node != start && closedList.Count > 0 && counter <gridNodes.Count )
        {
            if (path_node == new Vector2(-999f, -999f))
            {
                Debug.Log("NO PATH");
                return null;
            }

            counter = counter + 1;
            index = index - 1;
            Vector2 currNode = path_node;
            path_node = getParent(currNode);
            closedList.Remove(currNode);
            path.Add(path_node);
          //  Debug.Log(path_node);
        }
        //Utils.ClearLogConsole();
      //  Debug.Log("RETURNING PATH "+path.Count+" "+path[path.Count-1]);
        return path;

    }


    public Vector2 getParent(Vector2 child)
    {
        foreach(Parent p in tracer)
        {
            if (p.self == child)
                return p.parentNode;
        }

        return new Vector2(-999f, -999f);
    }

    public void removeParent(Vector2 child)
    {
        int i = 0;
        for(i = 0;i<tracer.Count;i++)
        {
            if (tracer[i].self == child)
                tracer.RemoveAt(i);
        }
    }

    public Vector2[,] getChildren(float x, float y)
    { 
        Vector2[,] children = new Vector2[3, 3];

        //Top left
        if (x - xdist >= -gridx && y + ydist < gridy && canSpawn(new Vector3(x - xdist, 0.5f, y + ydist), radius) && canSpawn(new Vector3(x - xdist  * 0.33f, 0.5f, y + ydist*0.33f),radius) && canSpawn(new Vector3(x - xdist * 0.66f, 0.5f, y + ydist * 0.66f), radius)) 
            children[0, 0] = new Vector2(x - xdist, y + ydist);
        else
            children[0, 0] = new Vector2(-999f,-999f);

        //Top middle
        if (x >= -gridx && y + ydist < gridy && canSpawn(new Vector3(x , 0.5f, y + ydist), radius) && canSpawn(new Vector3(x, 0.5f, y + ydist * 0.33f), radius) && canSpawn(new Vector3(x, 0.5f, y + ydist * 0.66f), radius))
            children[0, 1] = new Vector2(x, y + ydist);
        else
            children[0, 1] = new Vector2(-999f, -999f);

        //Top Right
        if (x + xdist < gridx && y + ydist < gridy && canSpawn(new Vector3(x + xdist, 0.5f, y + ydist), radius) && canSpawn(new Vector3(x + xdist * 0.33f, 0.5f, y + ydist * 0.33f), radius) && canSpawn(new Vector3(x + xdist * 0.66f, 0.5f, y + ydist * 0.66f), radius))
            children[0, 2] = new Vector2(x + xdist, y + ydist);
        else
            children[0, 2] = new Vector2(-999f, -999f);

        //Middle left
        if (x - xdist >= -gridx && y < gridy && canSpawn(new Vector3(x - xdist, 0.5f, y), radius) && canSpawn(new Vector3(x - xdist * 0.33f, 0.5f, y), radius) && canSpawn(new Vector3(x - xdist * 0.66f, 0.5f, y), radius))
            children[1, 0] = new Vector2(x - xdist, y);
        else
            children[1, 0] = new Vector2(-999f, -999f);

        //Middle Middle
        if (x < gridx && y < gridy)
            children[1, 1] = new Vector2(-999f,-999f);
        else
            children[1, 1] = new Vector2(-999f, -999f);

        //Middle right
        if (x + xdist < gridx  && y < gridy && canSpawn(new Vector3(x + xdist, 0.5f, y), radius) && canSpawn(new Vector3(x + xdist * 0.33f, 0.5f, y), radius) && canSpawn(new Vector3(x + xdist * 0.66f, 0.5f, y), radius))
            children[1, 2] = new Vector2(x + xdist, y);
        else
            children[1, 2] = new Vector2(-999f, -999f);

        //Bottom Left
        if (x - xdist >= -gridx && y - ydist >= -gridy && canSpawn(new Vector3(x - xdist, 0.5f, y - ydist), radius) && canSpawn(new Vector3(x - xdist * 0.33f, 0.5f, y - ydist * 0.33f), radius) && canSpawn(new Vector3(x - xdist * 0.66f, 0.5f, y - ydist * 0.66f), radius))
            children[2, 0] = new Vector2(x - xdist, y-ydist);
        else
            children[2, 0] = new Vector2(-999f, -999f);

        //Bottom Middle
        if (x >= -gridx && y - ydist >=-gridy && canSpawn(new Vector3(x, 0.5f, y - ydist), radius) && canSpawn(new Vector3(x, 0.5f, y - ydist * 0.33f), radius) && canSpawn(new Vector3(x, 0.5f, y - ydist * 0.66f), radius))
            children[2, 1] = new Vector2(x, y - ydist);
        else
            children[2, 1] = new Vector2(-999f, -999f);

        //Bottom Right
        if (x + xdist < gridx && y - ydist >= -gridy && canSpawn(new Vector3(x + xdist, 0.5f, y - ydist), radius) && canSpawn(new Vector3(x + xdist * 0.33f, 0.5f, y - ydist * 0.33f), radius) && canSpawn(new Vector3(x + xdist * 0.66f, 0.5f, y - ydist * 0.66f), radius))
            children[2, 2] = new Vector2(x + xdist, y - ydist);
        else
            children[2, 2] = new Vector2(-999f, -999f);

        return children;
    }
}
