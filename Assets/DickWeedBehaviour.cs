using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DickWeedBehaviour : MonoBehaviour
{
    private const float CHECK_RADIUS = 0.03f;

    public LayerMask GroundLayers;
    public string PlayerTag = "Player";

    private Transform _groundCheck;
    private Transform _playerCheck;
    private Transform _leftCheck;
    private Transform _rightCheck;
    private float _walkSpeed = 200;
    private float _walkDirection = 1;
    private Rigidbody2D _body;
    private bool _grounded = false;

    // Start is called before the first frame update
    void Start()
    {
        _body = this.GetComponent<Rigidbody2D>();

        _groundCheck = this.gameObject.transform.Find("Ground Check");
        _leftCheck = this.gameObject.transform.Find("Left Wall Check");
        _rightCheck = this.gameObject.transform.Find("Right Wall Check");
        _playerCheck = this.gameObject.transform.Find("Player Check");
    }

    // Update is called once per frame
    void Update()
    {
        // Start walking to the left if we walk into a wall on the right.
        if (_walkDirection > 0)
        {
            if (PerformOverlapCheck(_rightCheck))
            {
                _walkDirection = -1;
            }
        }
        else if (_walkDirection < 0)
        {
            if (PerformOverlapCheck(_leftCheck))
            {
                _walkDirection = 1;
            }
        }

        _grounded = PerformOverlapCheck(_groundCheck);

        // Check for the player hitting our top.
        if (PerformPlayerCheck(_playerCheck))
        {
            // We're fucked.
            this.gameObject.SetActive(false);
            return;
        }

        // Find the player on either of our other sides.
        if (PerformPlayerCheck(_leftCheck) || PerformPlayerCheck(_rightCheck) || PerformPlayerCheck(_groundCheck))
        {
            // TODO: Phil health.
            KillPlayer();
            return;
        }
    }

    private void KillPlayer()
    {
        var phil = GameObject.FindGameObjectWithTag(PlayerTag);
        if (phil != null)
        {
            var philController = phil.GetComponent<PhilipController>();
            if (philController != null)
            {
                philController.Respawn();
            }
        }
    }

    private void FixedUpdate()
    {
        if (_grounded)
        {
            float velX = (_walkSpeed * _walkDirection) * Time.fixedDeltaTime;
            _body.velocity = new Vector2(velX, _body.velocity.y);
        }
    }

    private bool PerformPlayerCheck(Transform transform)
    {
        var collisions = Physics2D.OverlapCircleAll(transform.position, CHECK_RADIUS);

        foreach (var collision in collisions)
        {
            if (collision.gameObject != this.gameObject && collision.gameObject.tag == PlayerTag)
            {
                return true;
            }
        }

        return false;

    }

    private bool PerformOverlapCheck(Transform transform)
    {
        var collisions = Physics2D.OverlapCircleAll(transform.position, CHECK_RADIUS, GroundLayers);

        foreach (var collision in collisions)
        {
            if (collision.gameObject != this.gameObject)
            {

                return true;
            }
        }

        return false;
    }
}
