
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Ladder : MonoBehaviour
{
    [Header("Progamming Stuff")]
    [SerializeField] private LayerMask _PlayerMask = new LayerMask();
    [SerializeField] private LayerMask _TileMask = new LayerMask();
    [SerializeField] private Button _LadderButton = null;
    // [SerializeField] private TextMeshProUGUI _TMP = null;

    [Header("Ladder Status")]
    [SerializeField] private float _Height = 3f;
    [SerializeField] private int _EnergyNeeded = 2;
    [SerializeField] private bool _IsAllowGoUp = false;

    private float _HalfHeight;
    // private bool _HasPlayerAtTop = false;
    // private bool _HasPlayerAtBottom = false;
    private float _CheckPlayerRayDistanceAtTop = 1f;
    private float _CheckPlayerRayDistanceAtButtom = 0.7f;
    private Player _Player;
    private bool _MoveToTop = false;
    private bool _MoveToBottom = false;
    private Vector3 _TargetPositionAtTop;
    private Vector3 _TargetPositionAtBottom;
    private Vector3 _TransPoint;
    private Vector3 _Origin;
    private bool _ReachedTransPoint = false;
    private bool _IsButtonClicked = false;
    private bool _IsMoving = false;
    private bool _IsPlayerInRange = false;
    private bool _ReachDestination = false;

    private Transform _lookHere;

    private void Awake()
    {
        _lookHere = GetComponentInChildren<Transform>();
        
        _Origin = transform.position - (transform.forward * 0.05f);
        _HalfHeight = _Height / 2;
        GameObject g = GameObject.FindGameObjectWithTag("Player");

        if (g != null)
        {
            _Player = g.GetComponent<Player>();
        }
        _TransPoint = _Origin;
        _TransPoint.y += _HalfHeight + 0.5f;

        GetTargetTileAtTop();

        _TargetPositionAtBottom = _Origin;
        _TargetPositionAtBottom.y -= (_HalfHeight - 0.5f);

        //Debug.Log("_TransPoint: " + _TransPoint);

    }

    private void GetTargetTileAtTop()
    {
        RaycastHit hit;
        Vector3 origin = _Origin;
        origin.y += _HalfHeight - 0.5f;
        // create a raycast to check player
        if (Physics.Raycast(origin, -transform.forward, out hit, 1f, _TileMask))
        {
            if (hit.transform.tag == "tile")
            {
                Tile t = hit.transform.GetComponent<Tile>();
                _TargetPositionAtTop = t.transform.position;
                _TargetPositionAtTop.y += 1f;
                //Debug.Log("_TargetPositionAtTop: " + _TargetPositionAtTop);
            }
        }
        Debug.DrawRay(origin, -transform.forward * 1f, Color.red);
    }
    //check player at top 

    private bool CheckPlayerAtTop()
    {
        RaycastHit hit;
        Vector3 origin = _Origin;
        origin.y += _HalfHeight + 0.5f;
        //Debug.Log("checking player at top!!!");
        // create a raycast to check player
        if (Physics.Raycast(origin, -transform.forward, out hit, _CheckPlayerRayDistanceAtTop, _PlayerMask))
        {
            //Debug.Log("checking player at top!!!");
            if (hit.transform.tag == "Player")
            {
                if (_LadderButton != null && !_IsMoving)
                {
                    //Debug.Log("button active");
                    //_Player.SetIsNearLadder();
                    _LadderButton.gameObject.SetActive(true);
                    _MoveToBottom = true;
                    _ReachDestination = false;
                    return true;
                }
                Debug.DrawRay(origin, -transform.forward * _CheckPlayerRayDistanceAtTop, Color.green);
            }
        }
        //Debug.Log("Checking player at top");
        Debug.DrawRay(origin, -transform.forward * _CheckPlayerRayDistanceAtTop, Color.red);
        return false;
    }
    //check player at bottom

    private bool CheckPlayerAtBottom()
    {
        RaycastHit hit;
        Vector3 origin = _Origin;
        origin.y -= _HalfHeight - 0.5f;
        origin -= transform.forward * 0.5f;
        // create a raycast to check player
        //Debug.Log("checking player at bottom");
        if (Physics.Raycast(origin, transform.forward, out hit, _CheckPlayerRayDistanceAtButtom, _PlayerMask))
        {
            //Debug.Log("checking player at bottom!!!");

            if (hit.transform.tag == "Player")
            {
                if (_LadderButton != null && !_IsMoving)
                {
                    //Debug.Log("button active");
                    //_Player.SetIsNearLadder();
                    _LadderButton.gameObject.SetActive(true);
                    _MoveToTop = true;
                    _ReachDestination = false;
                    return true;
                }
                Debug.DrawRay(origin, transform.forward * _CheckPlayerRayDistanceAtButtom, Color.green);
            }
        }
        //Debug.Log("Checking player at bottom");
        Debug.DrawRay(origin, transform.forward * _CheckPlayerRayDistanceAtButtom, Color.red);
        return false;
    }
    //move player from top to bottom 
    private void MovePlayer()
    {
        Vector3 playerPosition = _Player.transform.position;
        _Player.transform.forward = -transform.forward;

        if (_MoveToBottom)
        {
            if (!_ReachedTransPoint)
            {

                if (_Player.MoveToByLocation(_TransPoint, (_TransPoint - playerPosition).normalized))
                {
                    _ReachedTransPoint = false;
                }
                else if (!_Player.MoveToByLocation(_TransPoint, (_TransPoint - playerPosition).normalized))
                {
                    _ReachedTransPoint = true;
                }
                //Debug.Log("Moving towards the transpoint");
            }
            else
            {
                if (_Player.MoveToByLocation(_TargetPositionAtBottom, (_TargetPositionAtBottom - _TransPoint).normalized))
                {
                    //Debug.Log("_TargetPositionAtBottom: " + _TargetPositionAtBottom);
                    // _MoveToTop = false;
                }
                else
                {
                    _Player.IdleAnimation();
                    _MoveToBottom = false;
                    _IsButtonClicked = false;
                    _IsMoving = false;
                    _ReachedTransPoint = false;
                    _ReachDestination = true;
                }
                //Debug.Log("Moving towards the endpoint");
            }
        }
        else if (_MoveToTop)
        {
            if (!_ReachedTransPoint)
            {

                if (_Player.MoveToByLocation(_TransPoint, (_TransPoint - playerPosition).normalized))
                {
                    _ReachedTransPoint = false;
                }
                else if (!_Player.MoveToByLocation(_TransPoint, (_TransPoint - playerPosition).normalized))
                {
                    _ReachedTransPoint = true;
                }
                //Debug.Log("Moving towards the transpoint");
            }
            else
            {
                if (_Player.MoveToByLocation(_TargetPositionAtTop, (_TargetPositionAtTop - _TransPoint).normalized))
                {
                    //Debug.Log("_TargetPositionAtTop: " + _TargetPositionAtTop);
                    // _MoveToTop = false;
                }
                else
                {
                    _Player.IdleAnimation();
                    _MoveToTop = false;
                    _IsButtonClicked = false;
                    _IsMoving = false;
                    _ReachedTransPoint = false;
                    _ReachDestination = true;
                }
                //Debug.Log("Moving towards the endpoint");
            }
            //Debug.Log("Reached transpoint: " + _ReachedTransPoint);
        }
        // move player from bottom to top
    }

    private void CheckPlayer()
    {
        if (_IsAllowGoUp)
        {
            if (CheckPlayerAtBottom() || CheckPlayerAtTop())
            {
                _IsPlayerInRange = true;
            }
            else
            {
                _IsPlayerInRange = false;
            }
        }else{
             if (CheckPlayerAtTop())
            {
                _IsPlayerInRange = true;
            }
            else
            {
                _IsPlayerInRange = false;
            }
        }
    }

    private void Update()
    {
        // Debug.Log("Updating!");

        if ((_IsPlayerInRange || _IsMoving) && !_Player.ismoving && _IsButtonClicked && !_ReachDestination)
        {
            //Debug.Log("Moving");
            MovePlayer();
        }
        CheckPlayer();
        if (!_IsPlayerInRange || _IsMoving)
        {

            _LadderButton.gameObject.SetActive(false);
        }


    }

    public void ButtonClicked()
    {
        _IsButtonClicked = !_IsButtonClicked;
        _IsMoving = true;
        _ReachedTransPoint = false;
        _Player.ClimbLadderAnimation();
        //Debug.Log("Button Clicked");

    }

    private void Reset()
    {
        //_LadderButton.gameObject.SetActive(false);
        //Debug.Log("Setting");
        _IsMoving = false;
        _IsButtonClicked = false;
        _ReachedTransPoint = false;
        _Player.IdleAnimation();
    }

    public int GetEnergyNeeded()
    {
        return _EnergyNeeded;
    }
}
