/*Copyright (C) Katherine Brough in association with Vancouver Film School*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class Healer : BaseAI
{
    //[SerializeField] private GameObject _spawnpoint;
    [SerializeField] private GameObject _particleobject;
   // [SerializeField] private ParticleSystem _particle;
   // private ParticleSystem _particle;
    private BaseAI _base;
    public NavMeshAgent _self;
    private int _rangermaxhealth = 2;
    private HealthScript _targetsHealth;
    public bool _gotoheal;
    private HealthScript _health;
    private Animator _anim;

    private string _healEffortSound = "Play_Healer_Attack_Effort";
    private string _healEnemySound = "Play_Enemy_Heal";

    protected override void Start()
    {
        base.Start();                   //calls baseAi Start
        _self = GetComponent<NavMeshAgent>();
        _health = GetComponent<HealthScript>();
        _gotoheal = false;
        _anim = GetComponent<Animator>();
        _particleobject.SetActive(false);
       // _particle = _particleobject.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (base.target == null)
        {
        _particleobject.SetActive(false);
            base.Wandering();
            return;
        }
        base.Update();          //calls baseAI update
        //if remaining distance is less than stopping distance and the destination equals the targets destination, continue
        if (_self.remainingDistance <= _self.stoppingDistance && _self.destination.x == base.target.position.x && _self.destination.z == base.target.position.z)
        {
            //Debug.Log("IN THE STOPPING DISTANCE HEALER");
            if (_gotoheal == true)               // this bool is updated in ranger and tank so healer goes and heals them
            {
                _self.SetDestination(target.position);
                //Debug.Log("gotrue is true" + _self.destination);
                HasReached();
            }
        }

    }
    // has reached destination
    private void HasReached()
    {
        if (_health.isStunned == true) { return; }
        //Debug.Log("healer in the healing");
      //  Instantiate(_particle, _spawnpoint.transform.position,_spawnpoint.transform.rotation);
        _targetsHealth = target.GetComponent<HealthScript>();               //finds targets health
        _particleobject.SetActive(true);                                                                  //increase heallth until max health is reached
        for (int i = 0; i < _targetsHealth._maxHealth; i++)
        {
            if (_targetsHealth._currentHealth >= _targetsHealth._maxHealth)
            {
                _targetsHealth._currentHealth = _targetsHealth._maxHealth;              //current health cannot exceed max health
                _gotoheal = false;          //cannot go into function anymore
            }
            _anim.Play("Heal");
            _targetsHealth._currentHealth++;              //increase targets health
           AkSoundEngine.PostEvent(_healEffortSound, gameObject);
           AkSoundEngine.PostEvent(_healEnemySound, _targetsHealth.gameObject);
        }
        base.Wandering();       //else stay back to wanding in BaseAi
    }


    void Death()
    {
        self.isStopped = true;
        self.ResetPath();
        Destroy(_self);
    }

    // protected override void CharacterIsMoving(NavMeshAgent navAgent)
    // {
    //     var agentAverageVelocity = Mathf.Abs(navAgent.velocity.x) + Mathf.Abs(navAgent.velocity.z) /2;  
    //     _anim.SetFloat("VelY", agentAverageVelocity/navAgent.speed);
    // }
}

