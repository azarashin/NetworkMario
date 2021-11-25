using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    float _speed = 1.0f;

    [SerializeField]
    float _lifeTime = 1.0f;

    float _leftTime; 

    // Start is called before the first frame update
    void Start()
    {
        _leftTime = _lifeTime; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += Vector3.up * _speed * Time.deltaTime;
        _leftTime -= Time.deltaTime; 
        if(_leftTime <= 0.0f)
        {
            Destroy(gameObject); 
        }
    }
}
