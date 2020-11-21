using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    // Start is called before the first frame update

    public void Awake()
    {

        if (GameObject.FindGameObjectsWithTag("Manager").Length > 1)
            Destroy(gameObject);

        if (GameObject.FindGameObjectsWithTag("Canvas").Length > 1)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

}
