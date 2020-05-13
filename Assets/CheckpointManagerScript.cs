using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManagerScript : MonoBehaviour
{
    private Transform _currentCheckpoint = null;

    public Transform CurrentSpawnPoint => _currentCheckpoint;

    public void SetRespawnLocation(Transform transform)
    {
        if (transform != null)
        {
            _currentCheckpoint = transform;
        }
    }
}
