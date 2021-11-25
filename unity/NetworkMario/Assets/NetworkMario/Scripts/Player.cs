using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    Animator _animator;

    [SerializeField]
    float _animSpeed = 1.0f;

    [SerializeField]
    float _moveSpeed = 15.0f;

    [SerializeField]
    float _jumpImpulse = 10.0f;

    Rigidbody2D _rigidBody;
    CapsuleCollider2D _collider; 

    // Start is called before the first frame update
    void Start()
    {
        _animator.speed = 0.0f;
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>(); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.A))
        {
            _animator.speed = _animSpeed;
            _animator.SetFloat("dir", -1.0f);
            _rigidBody.AddForce(Vector2.left * _moveSpeed, ForceMode2D.Force);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _animator.speed = _animSpeed;
            _animator.SetFloat("dir", 1.0f);
            _rigidBody.AddForce(Vector2.right * _moveSpeed, ForceMode2D.Force);
        }
        else
        {
            _animator.speed = 0.0f;
        }
        if (Input.GetKey(KeyCode.W) && IsOnFloor())
        {
            _rigidBody.AddForce(Vector2.up * _jumpImpulse, ForceMode2D.Impulse);
        }
    }

    bool IsOnFloor()
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
