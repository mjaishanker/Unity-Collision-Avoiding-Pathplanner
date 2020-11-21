using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A helper Priority Queue class
public class PriorityQueue 
{

    public List<Node> myList = new List<Node>();

    public Node pop()
    {
        float minValue = 9999999;
        int index = -1;
        for (int i = 0; i < myList.Count; i++)
        {
            if (minValue > myList[i].f)
            {
                minValue = myList[i].f;
                index = i;
            }
        }
        
        Node poppedNode = myList[index];
        myList.RemoveAt(index);
        return poppedNode;

    }

    public bool doesContain(Vector2 node)
    {
        foreach (Node n in myList)
            if (n.node == node)
                return true;
        return false;
    }

    public bool findRemove(Vector2 node)
    {
       
        foreach (Node n in myList)
            if (n.node == node)
            {
                myList.Remove(n);
                return true;
            }

        return false;
    }

    public Node findItem(Vector2 node)
    {
        foreach (Node n in myList)
            if (n.node == node)
            {
                return n;
            }

        return null;
    }
}