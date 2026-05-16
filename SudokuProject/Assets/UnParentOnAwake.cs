using UnityEngine;

public class UnParentOnAwake : MonoBehaviour
{
    void Awake()
    {
        transform.parent = null;
    }
}
