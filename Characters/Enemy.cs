using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : Characters
{

    // public static bool PlayerDetectedByOneEnemy = false;

    [Header("Programming Stuff")]
    [SerializeField] private LayerMask _WallMask = new LayerMask();
    [SerializeField] private LayerMask _PlayerMask = new LayerMask();
    [SerializeField] private Player _Player = null;

    //[SerializeField] private Tile _TestTile;
    [Header("Enemy Status")]
    [SerializeField] private float _RotationSpeed = 180f;
    [SerializeField] private int _MaxMovementRange = 15;
    [SerializeField] private int _NumberOfTilesCanMoveWhenAlert = 10;
    [SerializeField] private List<Tile> _ListOfCheckpoints = new List<Tile>();
    [SerializeField] private int _NumberOfTurnsForChasing = 3;
    [SerializeField] private float _BackRayDistance = 1.5f;

    private float _EnemyRayDistance = 0.6f;
    private bool _HasWall = false;
    private int _TilesMoved = 0;
    private float _DistanceMoved = 0f;
    private bool _IsEnemyTurn = false;
    private float _AngleRotated = 0f;
    private int _TurnningDirection = 0;
    private bool _IsTurnning = false;
    private int _Counter = 0; //keep track of the list
    private bool _PatrolMove = false;
    private bool _IsChasingPlayer = false;
    private IEnumerator _CurrentState = null;
    private Tile _PlayerCurrentPosition;
    private bool _AIMovement = false;
    private int _TurnUsed = 0;
    private bool _IsPlayerInAttackRange = false;
    private List<Transform> _PlayerInSight = new List<Transform>();
    private List<Transform> _PlayerInAttackRange = new List<Transform>();

    [Header("Enemy Audio")]
    [SerializeField] private AudioClip _voDetects;
    [SerializeField] private AudioClip _voAttack;
    [SerializeField] private AudioClip _sfxLightning;
    [SerializeField] private AudioClip _voIdle;
    [SerializeField] private AudioClip _voLost;

    [SerializeField] private GameObject _Lightning;

    private void Awake()
    {
        //_Anim = gameObject.GetComponentInChildren<Animator>();

    }

    private void Start()
    {
        //Debug.Log("_Anim: " + _Anim.ToString());
        _PlayerCurrentPosition = GetTargetTile(_Player.gameObject);
        currentTile = GetTargetTile(gameObject);
        SetState(State_Idle());

        // _Anim = _CharacterMesh.GetComponent<Animator>();
    }

    #region StateMachine
    private void SetState(IEnumerator newState)
    {
        if (_CurrentState != null)
        {
            StopCoroutine(_CurrentState);
        }

        _CurrentState = newState;
        StartCoroutine(_CurrentState);
    }

    private IEnumerator State_Move()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.SetHasNoEnemy();
        // Debug.Log(transform.gameObject.name + ":  enemy move!");
        FindSelectableTiles(_MaxMovementRange, _NumberOfTilesCanMove, _IDNum);
        // Debug.Log(transform.gameObject.name + ":  " + _ListOfCheckpoints[_Counter].ToString());
        PlanPath(_ListOfCheckpoints[_Counter]);

        while (_PatrolMove && !_IsPlayerInAttackRange)
        {
            //Debug.Log("Moving!!!");
            MoveByPath(_NumberOfTilesCanMove);
            yield return null;
            // Debug.Log(transform.name + "_PatrolMove = " + _PatrolMove);
        }
        if (currentTile.transform == _ListOfCheckpoints[_ListOfCheckpoints.Count - 1].transform)
        {
            _Counter = 0;
        }
        if (currentTile.transform == _ListOfCheckpoints[_Counter].transform)
        {
            _Counter++;
        }
        //StopCoroutine(_CurrentState);
        //Debug.Log("end of enemy move!");
        UseActionPoint(1);
        // Debug.Log("Enemy actionpoint +1");
        if (_IsPlayerInAttackRange)
        {
            SetState(State_AttackingPlayer());
        }
        else
        {
            SetState(State_Idle());
            _ChrAnim.SetTrigger("isIdle");
        }
    }

    private IEnumerator State_Alert()
    {
        Debug.Log(transform.gameObject.name + ":  " + "Alert!!!!!!!");

        currentTile = GetTargetTile(gameObject);
        currentTile.SetHasNoEnemy();

        if ((_PlayerCurrentPosition.transform.position - currentTile.transform.position).sqrMagnitude <= 0.05f)
        {
            _AIMovement = true;
            _PatrolMove = false;
        }
        else
        {
            _PatrolMove = true;
            FindSelectableTiles(_MaxMovementRange, _MaxMovementRange, _IDNum);
            PlanPath(_PlayerCurrentPosition);
        }
        _TurnUsed++;
        while (_PatrolMove && !_IsPlayerInAttackRange)
        {
            Debug.Log("MoveByPath in Alert");
            MoveByPath(_NumberOfTilesCanMoveWhenAlert);
            _AudioSource.clip = _voDetects;
            _AudioSource.Play();
            yield return null;
        }
        while (_AIMovement && !_IsPlayerInAttackRange)
        {
            AIMovement();
            yield return null;
        }
        if (_IsPlayerInAttackRange)
        {
            this.transform.LookAt(_Player.transform);
            _ChrAnim.SetTrigger("isPunch");
            _AudioSource.clip = _voAttack;
            _AudioSource.Play();

            var _zapPlayer = Instantiate(_Lightning, transform.localPosition, transform.rotation);
            _zapPlayer.transform.rotation = transform.rotation;

            SetState(State_AttackingPlayer());
        }

        if (_TurnUsed >= _NumberOfTurnsForChasing)
        {
            _PatrolMove = true;
            // _AIMovement = false;
            _TurnUsed = 0;
            _IsChasingPlayer = false;
            _IsPlayerInAttackRange = false;
            _PlayerCurrentPosition = null;
            Reset();
        }
        UseActionPoint(1);
        Debug.Log("Enemy actionpoint +1");
        _ChrAnim.SetTrigger("isIdle");
        SetState(State_Idle());
        //StopCoroutine(_CurrentState);
    }

    private IEnumerator State_AttackingPlayer()
    {
        _IsPlayerInAttackRange = false;
        _Player.TakeDamage(_AttackDamage);
        Debug.Log("Attack! Player's health: " + _Player.GetHealth());
        yield return null;
        SetState(State_Idle());
    }

    private IEnumerator State_Idle()
    {
        //Debug.Log(transform.gameObject.name + ":  enemy idle!");
        // _Anim.SetBool("isWalk", false);
        // _Anim.SetBool("isRun", false);

        GetCurrentTile();
        _IsEnemyTurn = false;
        currentTile.SetHasEnemy();
        while (!_IsEnemyTurn)
        {
            // Debug.Log("enemy Idling");
            // GetCurrentTile();
            CheckForPlayer();
            CheckBack();
            yield return null;
        }
        _Player.SetCannotBackStab();
        //Debug.Log("End of state idle");

        if (_IsChasingPlayer)
        {
            _ChrAnim.SetTrigger("isRun");
            SetState(State_Alert());
        }
        else
        {
            _ChrAnim.SetTrigger("isWalk");
            SetState(State_Move());
        }

    }

    #endregion

    private void CheckBack()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        // create a raycast to check player
        if (Physics.Raycast(origin, -transform.forward, out hit, _BackRayDistance, _PlayerMask))
        {
            // Debug.Log("checking player!!!");
            if (hit.transform.tag == "Player")
            {

                Debug.DrawRay(origin, -transform.forward * _BackRayDistance, Color.green);
                _Player.SetCanBackStab(gameObject);
                // PlayerDetectedByOneEnemy = true;
            }
        }
        // else
        // {
        //     Debug.DrawRay(origin, -transform.forward * _BackRayDistance, Color.red);
        //     _Player.SetCannotBackStab();
        // }
    }

    private void MoveByPath(float numOfTilesToMove)
    {
        if (_TilesMoved < numOfTilesToMove && path.Count > 0)
        {
            // Debug.Log(transform.name + "Move By Path with condition: !_IsChasingPlayer");
            //Debug.Log(path.ToString());
            Tile t = path.Peek();//peek the first tile in the stack
            Vector3 target = t.transform.position;// get the first element in the stack's position

            target.y += 2 * _HalfHeight;// adjust the y value of the target
                                        // Debug.Log(transform.gameObject.name + " Moving to " + target.ToString());
            Vector3 heading = target - transform.position;// get the vector of where the player should be heading
            Vector3 norm = heading.normalized;// normalized the heading vector
            if ((target - transform.position).magnitude <= 0.05f)
            {
                //Debug.Log("path.pop()");
                t.SetNotPath();
                path.Pop();// remove the first element in the stack
                _TilesMoved++;
                // Debug.Log(transform.name + "_TilesMoved = " + _TilesMoved);
            }
            else
            {
                //Debug.Log("Enemy Moving to " + target.ToString());
                transform.forward = norm;// adjust where the player is facing
                transform.position = Vector3.MoveTowards(transform.position, target, _MovementSpeed * Time.deltaTime);// use Time.delataTime to create a smooth movement
                CheckForPlayer();
            }

        }
        else
        {
            // Debug.Log(transform.name + "Reset() called !_IsChasingPlayer");
            Reset();
            _PatrolMove = false;
            _TilesMoved = 0;
        }
    }

    private void AIMovement()
    {
        _AudioSource.clip = _voLost;
        _AudioSource.Play();

        if ((int)_DistanceMoved < _NumberOfTilesCanMoveWhenAlert)
        {
            _HasWall = CheckWalls(gameObject);
            if (!_HasWall)
            {
                transform.Translate(Vector3.forward * _MovementSpeed * Time.deltaTime, Space.Self);
                _DistanceMoved += _MovementSpeed * Time.deltaTime;
                CheckForPlayer();
            }
            else
            {
                if (!_IsTurnning)
                {
                    CheckTurningDirection();
                    _IsTurnning = true;
                }
                else
                {
                    if (_TurnningDirection == 0)
                    {
                        TurnLeft();
                    }
                    else if (_TurnningDirection == 2)
                    {
                        TurnRight();
                    }
                    else
                    {
                        TurnBack();
                    }
                }
            }

        }
        else
        {
            _AIMovement = false;
            _DistanceMoved = 0;
        }
    }

    private void PlanPath(Tile target)
    {
        // Debug.Log(gameObject.name + "'s target is: " + target.gameObject.name);
        MakePath(target);
        _PatrolMove = true;

    }

    #region  AIMovement

    private void TurnLeft()
    {
        if (_AngleRotated < 90f)
        {
            transform.Rotate(-transform.up, _RotationSpeed * Time.deltaTime);
            _AngleRotated += _RotationSpeed * Time.deltaTime;
        }
        else
        {
            _HasWall = false;
            _AngleRotated = 0f;
            _IsTurnning = false;
        }
    }
    private void TurnRight()
    {
        if (_AngleRotated < 90f)
        {
            transform.Rotate(transform.up, _RotationSpeed * Time.deltaTime);
            _AngleRotated += _RotationSpeed * Time.deltaTime;
        }
        else
        {
            _HasWall = false;
            _AngleRotated = 0f;
            _IsTurnning = false;
        }
    }
    private void TurnBack()
    {
        if (_AngleRotated < 180f)
        {
            transform.Rotate(transform.up, _RotationSpeed * Time.deltaTime);
            _AngleRotated += _RotationSpeed * Time.deltaTime;
        }
        else
        {
            _HasWall = false;
            _AngleRotated = 0f;
            _IsTurnning = false;
        }
    }


    public bool CheckWalls(GameObject target)
    {
        bool isFacingWall = false;
        RaycastHit hit;
        Vector3 origin = target.transform.position;
        // create a raycast to check wall
        if (Physics.Raycast(origin, transform.forward, out hit, _EnemyRayDistance, _WallMask))
        {
            Debug.DrawRay(origin, transform.forward, Color.green);
            isFacingWall = true;
        }
        Debug.DrawRay(origin, transform.forward, Color.red);
        return isFacingWall;
    }

    private void CheckTurningDirection()
    {
        bool hasWallOnLeft = false;
        bool hasWallOnRight = false;
        RaycastHit hit;
        Vector3 origin = transform.position;
        // create a raycast to check wall
        if (Physics.Raycast(origin, transform.right, out hit, _EnemyRayDistance, _WallMask))
        {
            Debug.DrawRay(origin, transform.forward, Color.green);
            hasWallOnRight = true;
        }
        Debug.DrawRay(origin, transform.forward, Color.red);
        if (Physics.Raycast(origin, -transform.right, out hit, _EnemyRayDistance, _WallMask))
        {
            Debug.DrawRay(origin, transform.forward, Color.green);
            hasWallOnLeft = true;
        }
        Debug.DrawRay(origin, transform.forward, Color.red);

        if (hasWallOnLeft && hasWallOnRight)
        {
            _TurnningDirection = 1;
        }
        else if (hasWallOnLeft && !hasWallOnRight)
        {
            _TurnningDirection = 2;
        }
        else if (!hasWallOnLeft && hasWallOnRight)
        {
            _TurnningDirection = 0;
        }
        else if (!hasWallOnLeft && !hasWallOnRight)
        {
            if (_TurnningDirection == 0)
            {
                _TurnningDirection = 2;
            }
            else
            {
                _TurnningDirection = 0;
            }
        }
    }

    #endregion

    public void SetToMove()
    {
        _IsEnemyTurn = true;
    }

    private void CheckForPlayer()
    {
        _PlayerInSight = GetComponent<FieldOfView>().FindVisibleTargets();
        _PlayerInAttackRange = GetComponent<FieldOfAttackRange>().FindVisibleTargets();

        if (_PlayerInSight.Count >= 1)
        {

            foreach (Transform p in _PlayerInSight)
            {
                if (p != null)
                {
                    _IsChasingPlayer = true;
                    _TurnUsed = 0;
                    ChasePlayer();
                    Debug.Log("Player spotted in view range!");

                }
            }
        }

        if (_PlayerInAttackRange.Count >= 1)
        {
            foreach (Transform pir in _PlayerInAttackRange)
            {
                if (pir != null)
                {
                    _IsPlayerInAttackRange = true;
                    _IsChasingPlayer = true;
                    _TurnUsed = 0;
                    ChasePlayer();
                    Debug.Log("Player spotted in attack range!");
                    // _AudioSource.Stop();
                }
            }
        }
        else
        {
            _IsPlayerInAttackRange = false;
        }
    }

    private void ChasePlayer()
    {
        Tile tmp = GetTargetTile(_Player.gameObject);

        if (tmp != null && tmp.transform.position.y == currentTile.transform.position.y)
        {
            _PlayerCurrentPosition = tmp;
            // path.Clear();
            Debug.Log("Player position update!");
        }
        //Reset();
        // _TilesMoved = 0;
    }

    public bool GetIsEnemyMoved()
    {
        return (_ActionPointUsed >= actionPoint);
    }

    public bool GetIsEnemyTurn()
    {
        return _IsEnemyTurn;
    }

}
