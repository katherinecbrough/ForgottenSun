using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    //[SerializeField] private GameObject _KiinWeapon;
    private Weapon _weaponscript;
    private PlayerController _player;
    private int _anim;
    [SerializeField] private int _damagechanger;
    public int _initialdamage = 2;
    public bool isMeleeattack;

    [SerializeField] private float _beginning;
    [SerializeField] private float _ending;
    private FieldOfView _fov;
    private int counter = 0;
    // private Weapon _weaponscript;
 

    // // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player = animator.GetComponent<PlayerController>();
        _player._isAttacking = true;
        _player.ResetVelocity();
        _player._ishit = true;
        if (isMeleeattack == true)
        {
            _weaponscript = animator.GetComponentInChildren<Weapon>();
            _weaponscript._damage = _damagechanger;
        }
        _fov = animator.GetComponent<FieldOfView>();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //int counter = 0;
        if (animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime >= _beginning && animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime <= _ending)
        {
            var targets = animator.transform.GetTargetsInFoV(_fov.ViewRadius, _fov.ViewAngle, _fov.TargetMask, _fov.ObstacleMask);
            //if(targets.Count <= 0) return;
            if(targets.Count > 0)
            {
            for (int i = 0; i < targets.Count; i++)
            {
                // Debug.Log("Colliders in FOV: " + targets[i]);
                if (targets[i].CompareTag("ShockWave"))
                { 
                    // targets[i].GetComponent<ShockWave>()._onhit = true;
                   // Debug.Log("whats good shockwave :D");
                    if(counter == 0)
                    {
                       // Debug.Log("velocity is negative on shockwave");
                    targets[i].GetComponent<Rigidbody>().velocity = -targets[i].GetComponent<Rigidbody>().velocity;
                    //  targets[i].GetComponent<ShockWave>()._onhit = false;
                    counter++;
                   }
                }
               // Debug.Log("Damaging field of view peeps");
                targets[i].GetComponent<HealthScript>().Damage(_damagechanger, AttackType.PHYSICAL, animator.gameObject);
            }
            }
        }
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        counter = 0;
        _player = animator.GetComponent<PlayerController>();
        _player._ishit = false;
        // _player._isAttacking = false;
        if (isMeleeattack == true)
        {
            _weaponscript = animator.GetComponentInChildren<Weapon>();
            _weaponscript._damage = _initialdamage;
        }


    }





}

