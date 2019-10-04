
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private GameObject _Camera = null;
    // [SerializeField] private float _Length = 0f;
    // [SerializeField] private Quaternion _TargetRotation = new Quaternion();
    [SerializeField] private GameObject _TargetPosition = null;
    [SerializeField] private GameObject _TargetPosition2 = null;
    // [SerializeField] private LayerMask _PlayerMask = new LayerMask();


    [Range(0f, 1f)] public float _RotationRate;
    private Quaternion _CameraInitialRotation;
    private Vector3 _CollisonPosisiton;
    private float _Distance;
    private float _InitialDistance;


    private void Awake()
    {
        _CameraInitialRotation = _Camera.transform.rotation;
        _InitialDistance = Vector3.Distance(_TargetPosition.transform.position, _TargetPosition2.transform.position);

    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.transform.tag == "Player")
    //     {
    //         Player player = other.GetComponent<Player>();
    //         // _CollisonPosisiton = c.contacts[0].point;
    //         if (_InitialDistance <= 0.02f)
    //         {
    //             _InitialDistance = Vector3.Distance(_TargetPosition.transform.position, player.transform.position);
    //         }
    //     }

    // }
    void OnTriggerStay(Collider other)
    {
        Debug.Log("Player enters the trigger");
        if (other.transform.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                _Distance = Vector3.Distance(_TargetPosition.transform.position, player.transform.position);
                Debug.Log("_Distance: " + _Distance);
                _RotationRate = (_InitialDistance - _Distance) / _InitialDistance;
                Debug.Log("_RotationRate: " + _RotationRate);
            }
        }
    }

    private void Update()
    {

        // _Camera.transform.rotation = Quaternion.Lerp(_CameraInitialRotation, _TargetRotation, _RotationRate);
        float angle = Mathf.LerpAngle(-45, -135, _RotationRate);
        _Camera.transform.eulerAngles = new Vector3(0, angle, 0);
    }
}
