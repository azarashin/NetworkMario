using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    [SerializeField]
    GameObject _prefabCoin; 

    [SerializeField]
    bool _debugTrigger = false;

    [SerializeField]
    float _speedToBreak = 0.5f; 

    SpriteRenderer _renderer; 



    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(_debugTrigger)
        {
            OpenBox();
        }
    }

    void OpenBox()
    {
        Instantiate(_prefabCoin, transform.position, Quaternion.identity);
        Destroy(gameObject); 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rigid_body = other.GetComponent<Rigidbody2D>();
        if(rigid_body && rigid_body.velocity.y < -_speedToBreak)
        {
            OpenBox(); 
        }
    }
}
