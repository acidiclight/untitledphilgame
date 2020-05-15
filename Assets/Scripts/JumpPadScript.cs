using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadScript : MonoBehaviour
{
    private Animator Animator = null;

    public float JumpForce = 1250;
    public float ForceMultiplier = 2;

    private void Awake()
    {
        Animator = this.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            var playerScript = other.gameObject.GetComponent<PhilipController>();
            playerScript.Jump(JumpForce * ForceMultiplier, true);

            if (Animator != null)
            {
                Animator.SetTrigger("Jumping");
            }
        }
    }
}
