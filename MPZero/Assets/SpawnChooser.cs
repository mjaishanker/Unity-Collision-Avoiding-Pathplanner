using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnChooser : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject obstacle;
    public GameObject agent;

    public Text text;
    private ObstaclePlacer myObstaclePlacer;
    public int mode = 0;
    void Start()
    {
        myObstaclePlacer = GameObject.FindGameObjectWithTag("Manager").GetComponent<ObstaclePlacer>();
        //text = GameObject.FindGameObjectWithTag("UIText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
          //  mode = 0;
            myObstaclePlacer.obstacle = obstacle;
            //text.text = "Obstacle";
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
          //  mode = 1;
            myObstaclePlacer.obstacle = agent;
            //text.text = "Agent";
        }
        
    }
    void OnLevelWasLoaded()
    {
        this.Start();
    }
}
