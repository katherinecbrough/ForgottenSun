/*
*  Copyright (C) Katherine Brough in association with Vancouver Film School
*/
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;


public class PlayerController : MonoBehaviour
{
    public GameObject _pos;
    [SerializeField] private float _MoveSpeed = .5f;
    [SerializeField] private float _rotationspeed = .5f;
    [SerializeField] private Transform _CameraBase;
    [SerializeField] private GameObject _resurectparticle;
    [HideInInspector] public bool isStruck;
    public bool _isAttacking;
    private Rigidbody _rigidbody;
    private float _SpeedMult;
    protected float _XInput;
    private float _ZInput;
    private float deadzone = 0.2f;
    [HideInInspector] public Animator _Anim;
    public int _AnimationIndex = 0;
    public AttackButton _LastInput;
    //the bool that makes the axis act like buttons
    private bool m_isAxisInUse = false;
    public bool _intriggerhand = false;
    private bool _pleaserotatekiin = false;
    public bool _ishit = false;
    
    //private ChromaticAberration _chromaticAberration;

    private GameMaster _gameMaster;

    private string _lightAttackSound = "Play_Club_Light_Swing";
    private string _heavyAttackSound = "Play_Club_Heavy_Swing";
    private string _respawnDialogeSound = "Play_Revive";

    private AbilitySelection _selector;

    public void Awake()
    {
        Time.timeScale = 1.0f;
        _gameMaster = FindObjectOfType<GameMaster>();
        GameMaster.instance.Player = this;
        _rigidbody = GetComponent<Rigidbody>();
        _Anim = GetComponent<Animator>();
        GameMaster.instance.DeathCount["Ranger"] = 0;
        GameMaster.instance.DeathCount["Tank"] = 0;
        GameMaster.instance._ressurectionbuildup["Ranger"] = 0;
        GameMaster.instance._ressurectionbuildup["Tank"] = 0;

        //on awake change players position to last checkpoints position through gamemaster singleton
    }

    private void Start()
    {
        Destroy(Instantiate(_resurectparticle, this.gameObject.transform.position, this.gameObject.transform.rotation), 2f);
        _selector = FindObjectOfType<AbilitySelection>();

    }

    private void OnEnable()
    {
        
        _isAttacking = false;
        if (Checkpoint.LastCheckPointPos != Vector3.zero && SceneManager.GetActiveScene().buildIndex != 2 && GameMaster.instance._loadAgain == false)
        {
            transform.position = Checkpoint.LastCheckPointPos;
            transform.rotation = Checkpoint.LastCheckPointRot;

        }
        AkSoundEngine.PostEvent(_respawnDialogeSound, gameObject);
        GameMaster.instance.DeathCount["Ranger"] = 0;
        GameMaster.instance.DeathCount["Tank"] = 0;
    }

    private void Update()
    {
        //Debug.Log(_chromaticAberration.intensity.value);
      // _chromaticAberration.intensity.value = 0.0f;
        if (GameMaster.instance._loadAgain == true)
        {
            //Debug.Log("Player TRUEEEEEE loadagain");
            transform.position = new Vector3(1480, 40, -1366);
            GameMaster.instance._loadAgain = false;
            Time.timeScale = 1.0f;
            // GameMaster.instance._loadAgain = false;
        }
        if (SceneManager.GetActiveScene().buildIndex == 2 && _pleaserotatekiin == false)
        {
            this.gameObject.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        }

        if (Input.GetAxis("Vertical") > deadzone)
        {
            _pleaserotatekiin = true;
        }
        ReadMoveInputs();
        UpdateRotation();
        ReadAttackInputs();
        if (_intriggerhand == true)
        {
            transform.position = _pos.transform.position;
        }
    }


    private void UpdateRotation()
    {
        if (_pleaserotatekiin == true)
        {
            if (_CameraBase == null)
            {
                //null check for camera in the serialize field
                print("Camera is not in Player Controllers SerializedField, Please drag Main Camera in");
                return;
            }
            //when the camera rotates the controls adapt to it for smooth game play
            float degrees = Mathf.Rad2Deg * Mathf.Atan2(_XInput, _ZInput) / 4;
            _rigidbody.transform.eulerAngles = new Vector3(transform.eulerAngles.x, degrees + _CameraBase.eulerAngles.y, transform.eulerAngles.z);
        }


    }

    private void FixedUpdate()
    {
        ApplyMovePhysics();
    }

    private void ApplyMovePhysics()
    {
        if (_isAttacking == false)
        //_isAttacking is called in Attack Behaviour in animation so player does not move when attacking
        {

            //movement for player
            //rotationspeed is side to side speed
            //movespeed is forward and backward speed
            var newVel = new Vector3(_XInput * _rotationspeed, 0f, _ZInput * _MoveSpeed);
            if (newVel.magnitude < deadzone)
            {
                _rigidbody.velocity = Vector3.up * _rigidbody.velocity.y;
                return;
            }
            newVel = transform.TransformVector(newVel);
            newVel.y = _rigidbody.velocity.y;
            _rigidbody.velocity = newVel;

        }

    }


    private void ReadMoveInputs()
    {
        _XInput = Input.GetAxis("Horizontal");
        _ZInput = Input.GetAxis("Vertical");
        //trying to make the player  not move with joystick on idle
        //adjusting deadzone on controller
        Vector2 stickInput = new Vector2(_XInput, _ZInput);
        if (stickInput.magnitude < deadzone)
        {
            _XInput = 0f;
            _ZInput = 0f;
            //_rigidbody.velocity = new Vector3(0,0,0);
        }
        //Debug.Log("xinput: " + _XInput + "zinput: " + _ZInput + "rigidbody: " + _rigidbody.velocity);
    }

    private void ReadAttackInputs()
    {
        //if buttons are false
        if (Input.GetAxisRaw("HeavyAttack") == 0 && Input.GetAxisRaw("LightAttack") == 0)
        {
            m_isAxisInUse = false;
        }
        //from willy for animation combo
        if (Input.GetAxisRaw("HeavyAttack") != 0) // HeavyAttack
        {
            if (m_isAxisInUse == false)
            {
                _LastInput = AttackButton.HeavyAttack;
                m_isAxisInUse = true;
                AkSoundEngine.PostEvent(_heavyAttackSound, gameObject);
            }
        }
        else if (Input.GetAxisRaw("LightAttack") != 0) // Light Attack
        {
            if (m_isAxisInUse == false)
            {
                _LastInput = AttackButton.LightAttack;
                m_isAxisInUse = true;
                AkSoundEngine.PostEvent(_lightAttackSound, gameObject);
            }
        }

        if (_LastInput != AttackButton.None && _AnimationIndex == 0)
        {
            NextAnimation();
        }
    }

    public void NextAnimation()
    {
        _AnimationIndex++;
        if (_AnimationIndex >= 4)
        {
            _AnimationIndex = 0;
        }

        if (_LastInput == AttackButton.None)
        {
            _AnimationIndex = 0;
        }
        else
        {
            _isAttacking = true;
        }

        _Anim.SetInteger("ComboIndex", _AnimationIndex);
        _Anim.SetInteger("HeavyLight", (int)_LastInput);
        _LastInput = AttackButton.None;
        //Debug.Log("NexytAnimation");
    }

    public void ResetVelocity()
    {
        _rigidbody.velocity = new Vector3();
        //called in attack behaviour on animations to reset velocity so sthat player cannot move on attack
    }
}
