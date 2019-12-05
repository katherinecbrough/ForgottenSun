/*Copyright (C) Katherine Brough in association with Vancouver Film School*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ResurrectedTank : BaseAI
{
    private HealthScript _health;
    public List<GameObject> currentHitObjects = new List<GameObject>();
    private Animator _anim;
    private float currentHitDistance;
    public float maxDistance = 10;
    public float sphereRadius;
    public LayerMask layerMask = -1;
    private Vector3 origin;
    private NavMeshAgent _self;

    private string _tankAttackEffort = "Play_Tank_Attack_Effort";
    private string _tankWhooshSound = "Play_Tank_Attack_Whoosh";

    protected override void Start()
    {
        _self = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _immediatleyEngaged = true;
        _engaged = true;
       // DEBUGME = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        
        origin = transform.position;
        if(base.target == null)
        {
        SphereCast();   
        }
        //self.SetDestination(target.position);   
        if (_self.remainingDistance <= _self.stoppingDistance &&  _self.destination.x == base.target.position.x && _self.destination.z == base.target.position.z)
        {
            HasReached();
            base.LookAtTarget();                    //makes tanks forward vector face his/her target
        }
        else{
            _self.SetDestination(base.target.transform.position);
        }

    }
    private void HasReached()
    {
       // if(DEBUGME == true) Debug.Log("HASREACHED");
        _anim.Play("Attack");                       //play attack animation to damage player 
        AkSoundEngine.PostEvent(_tankAttackEffort, gameObject);
        AkSoundEngine.PostEvent(_tankWhooshSound, gameObject);
        return;
    }
    private void SphereCast()
    {
       // if(DEBUGME == true) Debug.Log("SphereCast");
         //makes a sphere cast around the tank and only adds enemies to the list to attack
        currentHitDistance = maxDistance;
        currentHitObjects.Clear();
        RaycastHit[] hits = Physics.SphereCastAll(origin, sphereRadius, transform.forward, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.GetComponent<Tank>() || hit.transform.gameObject.GetComponent<Ranger>() || hit.transform.gameObject.GetComponent<Healer>() || hit.transform.gameObject.GetComponent<BaseBoss>())
            {
                currentHitObjects.Add(hit.transform.gameObject);
                currentHitDistance = hit.distance;
            }
        }
        if(currentHitObjects.Count <= 0)
        {
            for(int i = 0; i - 1 < BaseAI.AIList.Count; i++)
            {
                float _compare = base.SortByDistance(BaseAI.AIList[i], BaseAI.AIList[i + 1]);
                if(_compare > 0)
                {
              //  Debug.Log("in for loop");
                    BaseAI _tmp = BaseAI.AIList[i];
                    BaseAI.AIList[i] = BaseAI.AIList[i + 1];
                    BaseAI.AIList[i + 1] = _tmp;
                  //  Debug.Log("end forloop");
                    return;
                }
                continue;
            }
            base.target = BaseAI.AIList[0].transform;
        }
        else
        {
        base.target = currentHitObjects[0].transform;                                    //makes new target the enemies that is the first in the list
        }
      
       
        _self.SetDestination(base.target.transform.position);
    
        //Debug.Log("Resurrection Tank Taregt: " + base.target);

    }
     private void OnDrawGizmosSelected() {
        
        Gizmos.color = Color.red;
        Debug.DrawLine(origin, origin + transform.forward * currentHitDistance);
        Gizmos.DrawWireSphere(origin + transform.forward * currentHitDistance, sphereRadius);
        
    }
}
