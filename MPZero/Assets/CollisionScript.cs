using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    public int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Agent Reached Goal:");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Hello: ");
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Agent")
        {
            counter++;
            Debug.Log("Agent Reached Goal: " + counter);
        }
    }
}
