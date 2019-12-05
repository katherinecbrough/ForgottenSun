/*Copyright (C) Katherine Brough in association with Vancouver Film School*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ResurrectedHealer : BaseAI
{
    private NavMeshAgent _self;
    private HealthScript _targetsHealth;
    private GameMaster GameMaster;


    protected override void Start()
    {
        base.Start();
        _self = GetComponent<NavMeshAgent>();
        base.target = GameMaster.instance.Player.transform;             //make baseAi target the main character so the healer heals the main character 
    }

    protected override void Update()
    {
        base.Update();
        if (_self.remainingDistance <= _self.stoppingDistance)
        {
            HasReached();
        }

    }

    private void HasReached()
    {
        _targetsHealth = base.target.GetComponent<HealthScript>();              //get main characters health component
        for (int i = 0; i < _targetsHealth._maxHealth; i++)
        {
            if (_targetsHealth._currentHealth >= _targetsHealth._maxHealth)
            {
                _targetsHealth._currentHealth = _targetsHealth._maxHealth;              //so the healer doesnt heal past max health
            }
            _targetsHealth._currentHealth++;                      //increase main characers health
            //_targetsHealth.HealthEvent?.Invoke(_targetsHealth.HealthPercentage);
            _targetsHealth.InvokeEventHealth();

            
        }
        base.Wandering();                                           //retrun to wandering
    }

}
