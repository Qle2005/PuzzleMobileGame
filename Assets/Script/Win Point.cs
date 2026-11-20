using UnityEngine;

public class WinPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Move Object")
        {
            Debug.Log("bạn đã giải đúng puzzle");
        }
    }
}
