using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    [SerializeField]
    GameObject _prefabCoin; 

    [SerializeField]
    bool _debugTrigger = false; 

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
        Instantiate(_prefabCoin);
        Destroy(gameObject); 
    }
}
