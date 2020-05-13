using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    private bool _isActive = false;
    private CheckpointManagerScript _manager = null;

    public bool IsSpawnPoint = false;

    private void Start()
    {
        var checkpointManagerGameObject = GameObject.Find("Checkpoint Manager");

        if (checkpointManagerGameObject != null)
        {
            _manager = checkpointManagerGameObject.GetComponent<CheckpointManagerScript>();

            if (_manager != null)
            {
                if(IsSpawnPoint)
                {
                    _manager.SetRespawnLocation(this.gameObject.transform);
                }
            }
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
                }
            }
        }
    }
}
