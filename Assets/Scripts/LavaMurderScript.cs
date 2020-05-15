using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaMurderScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            var playerScript = collision.gameObject.GetComponent<PhilipController>();
            if (playerScript != null)
            {
                playerScript.Respawn();
            }
        }
    }
}
