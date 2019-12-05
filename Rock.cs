using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private Animator _animator;
    private bool _doesOnce = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() && _doesOnce == false)
        {
            _animator.SetBool("RockClose", true);
            _doesOnce = true;
        }
    }

}
