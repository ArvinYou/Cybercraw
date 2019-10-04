using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrapplePoint : MonoBehaviour
{
    [SerializeField] private float _PlayerCheckRange = 0f;
    [SerializeField] private LayerMask _PlayerMask;
    [SerializeField] private LayerMask _GroundMask;
    [SerializeField] private Player _Player = null;
    [SerializeField] private GameObject _Origin = null;

    private float _Distance = 0f;
    private Tile _TileAttached = null;
    private bool _IsPlayerInRange = false;
    private float _HalfHeight = 0.5f;
    private SpriteRenderer _GrapplePointIndicator;


    private void Awake()
    {
        _GrapplePointIndicator = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        CheckTileAttached();
        //Debug.Log("Tile Attached: " + _TileAttached.ToString());
        CheckPlayerDistance();
    }



    private void CheckPlayerDistance()
    {
        _Distance = Vector3.Distance(_Origin.transform.position, _Player.transform.position);
        RaycastHit hit;
        Vector3 heading = (_Player.transform.position - _Origin.transform.position).normalized;
        //Debug.Log("heading: " + heading.ToString());
        // create a raycast to check
        if (Physics.Raycast(_Origin.transform.position, heading, out hit, _PlayerCheckRange, _PlayerMask))
        {
            Debug.DrawRay(_Origin.transform.position, heading * _PlayerCheckRange, Color.green);
            _IsPlayerInRange = true;
            _GrapplePointIndicator.color = Color.green;
            //Debug.Log("Object On Hit: " + hit.transform.ToString());
        }
        else
        {
            Debug.DrawRay(_Origin.transform.position, heading * _PlayerCheckRange, Color.red);
            _IsPlayerInRange = false;
            _GrapplePointIndicator.color = Color.red;
        }
    }

    private void CheckTileAttached()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        // create a raycast to check
        if (Physics.Raycast(origin, -transform.forward, out hit, 1f, _GroundMask))
        {
            _TileAttached = hit.collider.GetComponent<Tile>();
            // Debug.Log("Tile hit.");
            // Debug.DrawRay(origin, -transform.forward, Color.red);
        }
        // Debug.DrawRay(origin, -transform.forward);
    }

    public bool GetIsPlayerInRange()
    {
        return _IsPlayerInRange;
    }
    public Tile GetTileAttached()
    {
        return _TileAttached;
    }
}
