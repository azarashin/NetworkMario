using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    Animator _animator;

    [SerializeField]
    float _speed = 1.0f; 

    // Start is called before the first frame update
    void Start()
    {
        _animator.speed = 0.0f; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.A))
        {
            _animator.speed = _speed;
            _animator.SetFloat("dir", -1.0f); 
        } else if (Input.GetKey(KeyCode.D))
        {
            _animator.speed = _speed;
            _animator.SetFloat("dir", 1.0f);
        } else {
            _animator.speed = 0.0f;
        }
    }
}
