using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElongatePickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<RopeEnd>() is RopeEnd ropeEnd)
        {
            ropeEnd.rope.Elongate();
            Destroy(gameObject);
        }
    }
}
