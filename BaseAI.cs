/*Copyright (C) Katherine Brough in association with Vancouver Film School*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class BaseAI : MonoBehaviour
{
    // [SerializeField] public Transform target;
    public Transform target;
    //[SerializeField] public GameObject player;
    [SerializeField] protected float wanderRadius;
    [SerializeField] private float FOV;
    //static public List <BaseAI> AIList = new List<BaseAI>();
    private float rotationDamping = 2;
    protected Transform _selftransform;
    static public List<BaseAI> AIList = new List<BaseAI>();
    [HideInInspector] public bool _SawPlayer = false;
    public bool _engaged = false;
    public bool _immediatleyEngaged = false;
    protected NavMeshAgent self;
    private LayerMask layermask;
    private Vector3 dest;
    private HealthScript stunHealth;
    private float angle;
    private float _count = 0;
    private float _WanderDelay = 2;
    [HideInInspector] public bool isStruck = false;
    [HideInInspector] public bool _tankRun = false;
    [SerializeField] private float _rangerZdist = 5;
    /*[HideInInspector]*/
    public bool IsHit = false;
    private bool _isBaseBoss = false;
    private bool _LookForEnemy = false;
    private bool _hasStunHealth = false;

    protected virtual void Awake()
    {
        _selftransform = GetComponent<Transform>();
        self = GetComponent<NavMeshAgent>();
        stunHealth = GetComponent<HealthScript>();
        if(stunHealth != null)
        {
            _hasStunHealth = true;
        }
        self.Warp(transform.position);
        if (!CompareTag("Tank") && !CompareTag("Ranger"))
        {
            //if baseai is attached to Boss or Healer we want the engaged to always be true so they are excluded from the engaged list
            _engaged = true;
        }
        if (CompareTag("Tank") || CompareTag("Ranger"))
        {
            AIList.Add(this);
        }
        if (this.gameObject.GetComponent<BaseBoss>())
        {
        _isBaseBoss = true;
        }
        
    }

    protected virtual void Start()
    {
        //dest = self.destination;
        if (gameObject.GetComponent<Healer>()) return;
      //  DEBUGME = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_hasStunHealth && _isBaseBoss)
        {
            if (stunHealth.isStunned == true)
            {
                self.isStopped = true;
              //  Debug.Log("Boss is stopped");
            }
            else
            {
                self.isStopped = false;
            }
        }

        //getting angle between the enemies target and their forward vector to see if they can see the player
        Vector3 targetDir = target.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);


        if (angle < FOV && _engaged == true || _SawPlayer && _engaged == true || _immediatleyEngaged == true)
        {
            //if player is within field of view, they are engaged and they have seen the player, go to player.
            _SawPlayer = true;
            MoveTowards();
        }
        else if (angle > FOV && !_SawPlayer) // if not wander
        {
            Wandering();
        }
        // else
        // {
        //     //error check
        //     // MyDebug.Log("error");
        // }
    }

    private void FixedUpdate()
    {
        if(_LookForEnemy == true)
        {
            LookForEnemy();
            _LookForEnemy = false;
        }
    }

  //  public bool DEBUGME = false;

    protected void Wandering()
    {
     //   if (DEBUGME == true) MyDebug.Log("Wandering");
        //makes a counter so when the enemies are wandering they wait to get a new destination
        _count += Time.deltaTime;
        //make enemy wander if target is not within field of view cone
        if (_isBaseBoss == true) { return; }

        Vector3 newPos = RandomNavSphere(self.transform.position, wanderRadius, -1);
        // float distance = Vector3.Distance(transform.position, target.transform.position);
        // if (distance < 20 && _tankRun == true && _WanderDelay < _count)
        // {
        //             Vector3 disToPlayer = transform.position - target.transform.position;
        //             Vector3 newpostion = RandomNavSphere(self.transform.position, distance, -1);
        //             self.SetDestination(newpostion);

        // }
        if (self.remainingDistance <= self.stoppingDistance)
        {
            if (_WanderDelay < _count)
            {
                self.SetDestination(newPos);
                _count = 0;

            }
        }

    }
    protected void MoveTowards()
    {
        if (target == null)                          //null checking
        {
            _SawPlayer = false;
            // print("Target is NULL");
            return;
        }
        self.SetDestination(target.position);           //moving towards player

        if (self.remainingDistance < self.stoppingDistance)
        {
            //if player has been seen already dont lose them follow!
            _LookForEnemy = true;
        }
    }

    private void LookForEnemy()
    {
        //looking around enemy
        var aroundMe = Physics.OverlapSphere(transform.position, wanderRadius, layermask);

        foreach (var item in aroundMe)
        {
            var otherUnit = target;
            if (otherUnit != null)
            {
                //making sure enemy stays in this loop of moving towards and looking for until he/she reaches destination
                target = otherUnit;
                MoveTowards();
                //if (DEBUGME == true) MyDebug.Log("Lookforenemy");
            }
            else
            {
                //player is not around, enemy should start wandering
                _SawPlayer = false;
            }
        }
    }

    protected void LookAtTarget()
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        if (CompareTag("Ranger") && _engaged == false)
        {
            dir.z = dir.z + _rangerZdist;
            //  MyDebug.Log("RANGER IN HERE MISSING PLAYER");
        }
        //updating rotation so players forward vector is looking at player 
        var rotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationDamping);
        return;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        //used to change the location of the sphere
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * distance;
        //add the newly generated direction to move in
        randDirection += origin;
        //create a navhit
        NavMeshHit navHit;
        //change the position of the object
        NavMesh.SamplePosition(randDirection, out navHit, distance, layermask);
        //then move the object
        return navHit.position;
    }

    public int SortByDistance(BaseAI a, BaseAI b)                            //mode this to sort the list of enemies by distance from maincharacter so i could make this script more complex if needed
    {
        float Range1 = (a.transform.position - transform.position).sqrMagnitude;
        float Range2 = (b.transform.position - transform.position).sqrMagnitude;
        return Range1.CompareTo(Range2);
    }

}
