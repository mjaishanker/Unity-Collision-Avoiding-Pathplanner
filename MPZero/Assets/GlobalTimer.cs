using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GlobalTimer : MonoBehaviour
{
    public float currentTime = 0f;
    public float startingTime = 30f;

    [SerializeField] Text countdownText;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        currentTime = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");
        if (currentTime < 0)
        {
            if (Time.timeScale == 1f)
            {
                Debug.Log("Time is STILL ZERO");
                Application.LoadLevel("End Scene");
                //SceneManager.LoadScene("End Scene", LoadSceneMode.Additive);
            }
     
        }
        
    }
}
