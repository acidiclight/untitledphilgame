using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    private bool _isActive = false;
    private CheckpointManagerScript _manager = null;
    private Animator Animator;

    private void Start()
    {
        Animator = this.GetComponent<Animator>();

        var checkpointManagerGameObject = GameObject.Find("Checkpoint Manager");

        if (checkpointManagerGameObject != null)
        {
            _manager = checkpointManagerGameObject.GetComponent<CheckpointManagerScript>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!_isActive)
            {
                _isActive = true;
                if (_manager != null)
                {
                    _manager.SetRespawnLocation(this.gameObject.transform);

                    var phil = collision.gameObject.GetComponent<PhilipController>();
                    if (phil != null)
                    {
                        phil.AddScore(25);
                    }

                    if (Animator != null)
                    {
                        Animator.SetTrigger("Activated");
                    }
                }
            }
        }
    }
}
