using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToLevelPickUp : MonoBehaviour
{
    public string sceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<RopeEnd>() is RopeEnd ropeEnd)
        {
            SceneManager.LoadScene(sceneName);
            Destroy(gameObject);
        }
    }
}
