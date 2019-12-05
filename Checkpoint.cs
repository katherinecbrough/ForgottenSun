/*Copyright (C) Katherine Brough in association with Vancouver Film School*/
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
  //  [SerializeField] private string _UnloadLevel;
    public static Vector3 LastCheckPointPos;
    public static Quaternion LastCheckPointRot;

    private GameMaster GetMaster;
    private Color _MyColor = Color.red;
    private SphereCollider _Collider;

    private void Awake()
    {
        _Collider = GetComponent<SphereCollider>();
        _Collider.isTrigger = true;
    }

    private void OnValidate()
    {
        _Collider = GetComponent<SphereCollider>();
        _Collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.GetComponent<PlayerController>())
        {
            //if the object that enters the trigger is the main character change the colour of the checkpoint ball to green and reset spawn point when character dies
            //to the position of this checkpoint
            _MyColor = Color.green;
            LastCheckPointPos = transform.position;
            LastCheckPointRot = transform.rotation;
            // if(_UnloadLevel == null)
            // {
            //     return;
            // }
      //      SceneManager.UnloadSceneAsync(_UnloadLevel);
           
        }
    }

    private void OnDrawGizmos()
    {
        if (_Collider == null) return;
        var oldColor = Gizmos.color;
        Gizmos.color = _MyColor;
        Gizmos.DrawWireSphere(transform.position + _Collider.center, _Collider.radius);
        Gizmos.color = oldColor;
    }
}
