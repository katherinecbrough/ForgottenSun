/*Copyright (C) Katherine Brough in association with Vancouver Film School*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ranger : BaseAI
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _arrowSpawn;
    [SerializeField] private float _bulletDelay = 2;
    [HideInInspector] public Animator _animator;
    //static public List<Ranger> RangerList = new List<Ranger>();
    private BaseAI _base;
    private HealthScript _health;
    private Healer _healer;
    public List<GameObject> currentHitObjects = new List<GameObject>();
    private float currentHitDistance;
    public float maxDistance = 10;
    public float sphereRadius;
    public LayerMask layerMask;
    private Vector3 origin;
    private NavMeshAgent _self;
    private float _counter = 0;
    private GameMaster GameMaster;
    private Vector3 _surroundingdistance;
    private float dist;
    private float playerdist;

    private string _attackSound = "Play_Ranger_Attack_Effort";
    private string _arrowSound = "Play_Ranger_Arrow";

    public int dangerIndex = 1;

    

    //on death Unity event
    //[SerializeField] private UnityEvent OnDeath;
    // private void OnEnable()
    // {
    //     if(CompareTag("Healer")) _healimage.healerfillamount += HealerDeath;
    // }
    // private void OnDisable()
    // {
    //     if(CompareTag("Healer")) _healimage.healerfillamount -= HealerDeath;
    // }


    private IntesityController _intensityController;
    private bool _isInTriggerBox = false;

    private void OnTriggerEnter(Collider otherCollider) => _isInTriggerBox = otherCollider.CompareTag("IntensityController");
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
        _self = GetComponent<NavMeshAgent>();
        _health = GetComponent<HealthScript>();
        base.target = GameMaster.instance.Player.transform;                 //sets target as player in gamemaster
        _surroundingdistance = new Vector3(30, 0, 30);
        _animator = GetComponent<Animator>();
        _intensityController = FindObjectOfType<IntesityController>();
    }

    protected override void Update()
    {
        CheckDistance();
        origin = transform.position;
        base.Update();                      //update in baseAI
        _counter += Time.deltaTime;            //making this counter so there is a delay with spawn of projectiles coming from ranger
        //if remaining distance is less than stopping distance and the destination equals the targets destination, continue
        if (_self.remainingDistance < _self.stoppingDistance && _self.destination.x == base.target.position.x && _self.destination.z == base.target.position.z)
        {
            //makes ranger face target
            base.LookAtTarget();
            //if there is a correect amount of delay for the bullets spawned continue
            if (_bulletDelay < _counter)
            {
                _counter = 0;               //reset counter
                HasReached();
            }
        }
        // if health is lower than maxhealth, call  nearby healer
        if (_health._currentHealth < _health._maxHealth)
        {
            SphereCast();
        }

    }
    private void CheckDistance()
    {
        //if player is within a distance from player but ranger is not looking at player the ranger will start to attack
        dist = Vector3.Distance(transform.position, transform.position + _surroundingdistance);             //get the distnce from the ranger to distance around ranger
        playerdist = Vector3.Distance(transform.position, base.target.transform.position);
        if (playerdist < dist)
        {
            base._immediatleyEngaged = true;
        }
    }
    private void HasReached()
    {
        if (_health.isStunned == true) { return; }
        gameObject.transform.LookAt(GameMaster.instance.Player.gameObject.transform.position);
        _animator.Play("Attack");
        GameObject projectile = Instantiate(_projectile, _arrowSpawn.transform.position, _arrowSpawn.transform.rotation);
        projectile.GetComponent<Rigidbody>().AddForce(transform.forward * 60, ForceMode.Impulse);
        AkSoundEngine.PostEvent(_attackSound, gameObject);
        AkSoundEngine.PostEvent(_arrowSound, gameObject);
        Destroy(projectile, 2);
    }

    private void SphereCast()
    {
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
                //change healers target to self, so healer can heal
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


    private void Death()
    {
        self.isStopped = true;
        self.ResetPath();
        Destroy(_self);
    }

    // protected override void CharacterIsMoving(NavMeshAgent navAgent)
    // {
    //     var agentAverageVelocity = Mathf.Abs(navAgent.velocity.x) + Mathf.Abs(navAgent.velocity.z) / 2;
    //     _animator.SetFloat("VelY", agentAverageVelocity / navAgent.speed);
    // }
}
