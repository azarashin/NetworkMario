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

    Rigidbody2D _rigidBody; 

    // Start is called before the first frame update
    void Start()
    {
        _animator.speed = 0.0f;
        _rigidBody = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.A))
        {
            _animator.speed = _animSpeed;
            _animator.SetFloat("dir", -1.0f);
            _rigidBody.AddForce(Vector2.left * _moveSpeed, ForceMode2D.Force);
        } else if (Input.GetKey(KeyCode.D))
        {
            _animator.speed = _animSpeed;
            _animator.SetFloat("dir", 1.0f);
            _rigidBody.AddForce(Vector2.right * _moveSpeed, ForceMode2D.Force);
        }
        else {
            _animator.speed = 0.0f;
        }
    }
}
