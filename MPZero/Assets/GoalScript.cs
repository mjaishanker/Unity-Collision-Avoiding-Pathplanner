using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GoalScript : MonoBehaviour
{
    public int agentAtGoal = 0;
    public float globalScore = 100f;
    [SerializeField] Text scoreText;
    void start()
    {
        globalScore = 100f;
        scoreText.text = globalScore.ToString("0");
    }
    void update()
    {
        if(globalScore == 0)
            Application.LoadLevel("End Scene");
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Agent")
        {
            agentAtGoal++;
            globalScore -= 10;
            scoreText.text = globalScore.ToString("0");
            Debug.Log("Agent Reached Goal: " + agentAtGoal);
        }
    }
}
