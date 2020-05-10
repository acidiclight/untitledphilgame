using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSwimTrigger : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            var playerScript = collision.gameObject.GetComponent<PhilipController>();

            if (playerScript != null)
            {
                playerScript.StopSwimming();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            var playerScript = collision.gameObject.GetComponent<PhilipController>();

            if (playerScript != null)
            {
                playerScript.StartSwimming();
            }
        }
    }
}
