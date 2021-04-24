using UnityEngine;

public class Socket : MonoBehaviour
{
    public bool Appear { get; internal set; }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<RopeEnd>() is RopeEnd ropeEnd)
        {
            Appear = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Appear && collision.GetComponent<RopeEnd>() is RopeEnd ropeEnd)
        {
            ropeEnd.rope.Reconnect(this);
            Destroy(gameObject);
        }
    }
}
