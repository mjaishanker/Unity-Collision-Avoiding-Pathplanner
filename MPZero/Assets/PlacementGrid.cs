using UnityEngine;

//Minor helper class that offers snap to grid functionality
public class PlacementGrid : MonoBehaviour
{
    public Vector3 snapTogrid(Vector3 pos)
    {
        Vector3 snapPos;
        snapPos.x = Mathf.Round(pos.x);
        snapPos.z = Mathf.Round(pos.z);
        snapPos.y = pos.y;

        return snapPos;
    }
}
