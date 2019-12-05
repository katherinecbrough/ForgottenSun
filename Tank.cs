/*Copyright (C) Katherine Brough in association with Vancouver Film School*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Tank : BaseAI
{
    private NavMeshAgent _self;
    [HideInInspector] public Animator _anim;
    private HealthScript _health;
    private Healer _healer;
    public List<GameObject> currentHitObjects = new List<GameObject>();
    //static public List<Tank> AIList = new List<Tank>();
    private float currentHitDistance;
    public float maxDistance = 10;
    public float sphereRadius = 20f;
    public LayerMask layerMask = ~0;        //layermask is set to everything
    private Vector3 origin;
    private GameMaster GameMaster;


    private string _swingSound = "Play_Tank_Attack_Whoosh";
    private string _attackEffortSound = "Play_Tank_Attack_Effort";

    public int dangerIndex = 1;

    private IntesityController _intensityController;
    private bool _isInTriggerBox = false;

    private void OnTriggerEnter(Collider otherCollider) => _isInTriggerBox = otherCollider.CompareTag("IntensityController") ? true : _isInTriggerBox;
    private void OnTriggerExit(Collider otherCollider) => _isInTriggerBox = otherCollider.CompareTag("IntensityController") ? false : _isInTriggerBox;

    private void OnDestroy()
    {
        if (_isInTriggerBox)
        {
            _intensityController.Intensity -= dangerIndex;
        }
    }

    protected override void Start()
    {
        base.Start();
        _anim = GetComponent<Animator>();
        _health = GetComponent<HealthScript>();
        _intensityController = FindObjectOfType<IntesityController>();
        // AIList.Add(this);
        //_health.OnDeathFromEvent += OnDeathFromEvent;
    }

    // Update is called once per frame
    protected override void Update()
    {
        _self = GetComponent<NavMeshAgent>();
        base.target = GameMaster.instance.Player.transform;                     //sets target as gamemasters player
        origin = transform.position;
        //goes to baseAI update
        base.Update();
        //if remaining distance is less than stopping distance and the destination equals the targets destination, continue
        if (_self.remainingDistance < _self.stoppingDistance && _self.destination.x == base.target.position.x && _self.destination.z == base.target.position.z && base._engaged == true)
        {
            HasReached();
            base.LookAtTarget();                    //enemies target transform to face player
        }
        //if health is lower than maxhealth grab a healer nearby and change its target to heal you.
        //MyDebug.Log("currenthealth: " + _health._currentHealth + "maxHealth" + _health._maxHealth);
        if (_health._currentHealth < _health._maxHealth)
        {
            SphereCast();
        }
    }

    private void HasReached()
    {
        if (_health.isStunned == true) { return; }
        //play the hit animation so tank can damage player with collider on hand
        _anim.SetTrigger("Attack");
        AkSoundEngine.PostEvent(_attackEffortSound, gameObject);
        AkSoundEngine.PostEvent(_swingSound, gameObject);
        return;
    }

    private void SphereCast()
    {
        // MyDebug.Log("SPHERE CAST TANK");
        currentHitDistance = maxDistance;
        currentHitObjects.Clear();
        //create a sphere to check if healer is nearby
        RaycastHit[] hits = Physics.SphereCastAll(origin, sphereRadius, transform.forward, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
        foreach (RaycastHit hit in hits)
        {
            //add objects to list
            currentHitObjects.Add(hit.transform.gameObject);
            currentHitDistance = hit.distance;
            if (hit.transform.gameObject.GetComponent<Healer>())
            {
                _healer = hit.transform.gameObject.GetComponent<Healer>();
                //telling healer nearby to come heal me
                _healer._gotoheal = true;
                _healer._self.SetDestination(this.gameObject.transform.position);
                _healer.target = this.transform;
            }
        }
    }

    //draws gizmos so i can see sphere in scene for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(origin, origin + transform.forward * currentHitDistance);
        Gizmos.DrawWireSphere(origin + transform.forward * currentHitDistance, sphereRadius);
    }

    // protected override void CharacterIsMoving(NavMeshAgent navAgent)
    // {
    //     var agentAverageVelocity = Mathf.Abs(navAgent.velocity.x) + Mathf.Abs(navAgent.velocity.z) /2;  
    //     _anim.SetFloat("VelY", agentAverageVelocity/navAgent.speed);
    // }
}

