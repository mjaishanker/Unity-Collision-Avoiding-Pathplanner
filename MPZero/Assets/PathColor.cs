using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathColor : MonoBehaviour
{

    public Renderer myRenderer;
    public Color myColor;

    public bool main = false;
    void Start()
    {
        myRenderer = gameObject.GetComponent<Renderer>();

    }

    // Update is called once per frame

    public void updateColor(Color c)
    {
        myRenderer = gameObject.GetComponent<Renderer>();
        myColor = c;
        myRenderer.material.SetColor("_Color", myColor);
    }
}
