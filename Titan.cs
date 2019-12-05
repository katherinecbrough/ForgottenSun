/*Copyright (c) Katherine Brough in Association with Vancouver Film School*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Titan : MonoBehaviour
{
    [SerializeField] private float cooldown = 9;
    [SerializeField] private GameObject _shockwave;
   [SerializeField] private GameObject _indicator;
    [SerializeField] private HealthScript _DeathBoss;
    [SerializeField] float _povX = -73.9f;
    [SerializeField] float _povZ = 36.3f;
    [SerializeField] private GameObject _spawnpoint;
    [SerializeField] private float _spawnzpos = 10;
    [SerializeField] private float _xoffset = 10;
    [SerializeField] public GameObject _shockwavehint;
    private TitansHand _titanshand;
    private Animator _anim;
    private Transform _player;
    private float attackTimer = 0;
    public float rotationDamping = 2;
    private float randomValue;
    public float _dangeroffset = 5;
    //private float _attacktimer = 0;
    public bool _meleeAttack = false;
    private float _currentLerpTime = 0f;
    private float _lerptime = 100f;
    public float _force = 100;
    public int _Zdist = 10;
    public int _shockwaveZ = 10;
    public bool _switchingbool = false;
    private int _counter = 0;
    //public int _Xdist = 20;
    //public float _Ydist = 4;

    private string _scytheSwingSound = "Play_Death_Titan_Scyth_Swing";
    private string _effortSound = "Play_Death_Titan_Efforts";

    private void Start()
    {
        _shockwavehint.SetActive(false);
        _player = GameMaster.instance.Player.transform;
        _anim = GetComponent<Animator>();
        _titanshand = GetComponentInChildren<TitansHand>();
        _counter = 0;

    }

    private void Update()
    {
        LookAtTarget(_player.transform);

        if (attackTimer > cooldown && _switchingbool == false)
        {
            _shockwavehint.SetActive(false);
            AttacksForTitan();
        }
        else
        {
            attackTimer += Time.deltaTime;
        }
        if (_DeathBoss._currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }


    private void AttacksForTitan()
    {
        attackTimer = 0;
        int _attack = UnityEngine.Random.Range(0, 2);
        //Debug.Log("attack is: " + _attack);

        if (_attack == 0)
        {
            MeleeAttack();
          //  _shockwavehint.SetActive(false);
         }
        if (_attack == 1)
        {
           ShockWave();
           _shockwavehint.SetActive(false);
     }
    }

    private void ShockWave()
    {
        _anim.SetTrigger("SpawnProjectile");
        Vector3 _pos = new Vector3(_player.transform.position.x - _xoffset, _spawnpoint.transform.position.y ,_player.transform.position.z);
        _spawnpoint.transform.LookAt(_pos);
        Vector3 _indicatoroffset = new Vector3(_player.transform.position.x - 5, _spawnpoint.transform.position.y + 10.3f,_player.transform.position.z + _dangeroffset);
       Destroy(Instantiate(_indicator, _indicatoroffset, _spawnpoint.transform.rotation), 5f);
        Vector3 _spawnpostion = new Vector3(_spawnpoint.transform.position.x, _spawnpoint.transform.position.y, _spawnpoint.transform.position.z - 10);
        GameObject waveOne = Instantiate(_shockwave, _spawnpostion, _spawnpoint.transform.rotation);
        waveOne.GetComponent<Rigidbody>().AddForce(_spawnpoint.transform.forward * _force, ForceMode.Impulse);
        Destroy(waveOne, 10f);
        AkSoundEngine.PostEvent(_effortSound, gameObject);
        _counter++;
        if(_counter ==2)
        {
            _shockwavehint.SetActive(true);
           
        }
    }

    private void MeleeAttack()
    {
        _meleeAttack = true;
        // Debug.Log("in attack titan");
        _anim.SetTrigger("Attack");
        // var transform.rotation.y = transform.rotation.y * 3f;
        AkSoundEngine.PostEvent(_scytheSwingSound, gameObject);
        AkSoundEngine.PostEvent(_effortSound, gameObject);
    }

    protected void LookAtTarget(Transform playerstransform)
    {
        //updating rotation so players forward vector is looking at player 
        Vector3 dir = playerstransform.position - transform.position;
        dir.y = 0;
        if (_meleeAttack == true)
        {
            //dir.x = dir.x - _Xdist;
            dir.z = dir.z + _Zdist;
            //dir.y = dir.y + _Ydist;
            MoveTitan(new Vector3(_povX, 0, _povZ));
        }
        if (_meleeAttack == false)
        {
            dir.z = dir.z + _shockwaveZ;
        }
        var rotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationDamping);
        return;
    }

    private void MoveTitan(Vector3 _distancebetweenfist)
    {
        _currentLerpTime += Time.deltaTime;
        if (_currentLerpTime > _lerptime)
        {
            _currentLerpTime = _lerptime;
            _currentLerpTime = 0;
        }
        // Debug.Log("in MoveTitan Titan");
        float perc = _currentLerpTime / _lerptime;
        Vector3 _pointofinterest = _player.position - _distancebetweenfist;
        _pointofinterest.y = this.gameObject.transform.position.y;
        this.gameObject.transform.position = Vector3.Lerp(transform.position, _pointofinterest, perc);
        //_spawnpoint.transform.LookAt(_player.position);
        // Quaternion.LookRotation(_player.position - transform.position, transform.up);

        //transform.LookAt(new Vector3(_player.position.x, 0, _player.position.z));
    }

    public void LerpToStartLocation(Vector3 _gameobjecttransform)
    {
        _currentLerpTime += Time.deltaTime;
        if (_currentLerpTime > _lerptime)
        {
            _currentLerpTime = _lerptime;
            _currentLerpTime = 0;
        }
        // Debug.Log("in MoveTitan Titan");
        float perc = _currentLerpTime / _lerptime;
        // Vector3 _pointofinterest = _player.position - _distancebetweenfist;
        _gameobjecttransform.y = this.gameObject.transform.position.y;
        this.gameObject.transform.position = Vector3.Lerp(transform.position, _gameobjecttransform, perc);
    }
}