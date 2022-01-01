using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    Animator _animator;

    [SerializeField]
    float _animSpeed = 1.0f;

    [SerializeField]
    float _moveSpeed = 15.0f;

    [SerializeField]
    float _jumpImpulse = 10.0f;

    [SerializeField]
    AudioSource _jumpSound; 

    Rigidbody2D _rigidBody;
    CapsuleCollider2D _collider;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator.speed = 0.0f;
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PhotonController.NetworkInputData data))
        {
            UpdateInput(data);
        }

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        float threthold_velocity = 0.1f; 
        if(_rigidBody.velocity.x > threthold_velocity)
        {
            _animator.speed = _animSpeed;
            _animator.SetFloat("dir", 1.0f);
        }
        else if (_rigidBody.velocity.x < -threthold_velocity)
        {
            _animator.speed = _animSpeed;
            _animator.SetFloat("dir", -1.0f);
        }
        else
        {
            _animator.speed = 0.0f;
        }

    }

    void UpdateInput(PhotonController.NetworkInputData data)
    {
        _rigidBody.AddForce(data.Force, ForceMode2D.Force);
        _rigidBody.AddForce(data.Impulse, ForceMode2D.Impulse);
        if(data.TriggerJump)
        {
            _jumpSound.Play();
        }
    }

    public float MoveSpeed()
    {
        return _moveSpeed; 
    }

    public float JumpImpulse()
    {
        return _jumpImpulse; 
    }

    public bool IsOnFloor()
    {
        int block_layer = 6;
        int mask = 1 << block_layer; 
        float margin = 0.01f;
        float threshold_speed = 1.0f;
        Debug.DrawLine(transform.position, transform.position + Vector3.down * (_collider.size.y * 0.5f + margin));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _collider.size.y * 0.5f + margin, mask);
        return (hit && Mathf.Abs(_rigidBody.velocity.y) < threshold_speed); 
    }
}
