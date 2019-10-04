using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class Characters : MonoBehaviour
{

    public static int NUMOFCHARA;

    //serialized field
    [Header("Programming Stuff")]
    [SerializeField] protected LayerMask _GroundMask = new LayerMask();
    [SerializeField] protected GrapplingHook _GH = null;

    [Header("Character Status")]
    [SerializeField] protected int _IDNum;
    [SerializeField] protected int _Health;
    [SerializeField] protected int _AttackDamage;
    [SerializeField] protected int _NumberOfTilesCanMove = 5;
    [SerializeField] protected float _MovementSpeed = 5f;
    [SerializeField] protected int _Energy = 100;
    [SerializeField] protected TextMeshProUGUI _TMP;
    [SerializeField] protected int actionPoint = 2;
    protected int apCounter = 2;

    // variables


    private Enemy[] _EnemiesList;
    protected Tile currentTile;
    protected List<Tile> selectableTiles = new List<Tile>();
    protected Stack<Tile> path = new Stack<Tile>();
    public bool ismoving = false;
    protected float _HalfHeight = 0.5f;
    protected bool _ClickedOnPlayer = false;
    protected bool _IsActionEmpty = false;
    protected int _EnergyUsed = 0;
    protected int _ActionPointUsed = 0;

    private TacticMovement _TactMoveRef;

    [SerializeField] private GameObject _deathSplash;

    public Animator _ChrAnim = null;
    [SerializeField] protected AudioSource _AudioSource = null;
    [Header("Audio")]
    [SerializeField] private AudioClip _clipMove;
    [SerializeField] private AudioClip _clipPlayerDamaged;
    [SerializeField] private AudioClip _clipPlayerDie;
    

    //array of all the tiles
    protected GameObject[] tiles;

    // called when the scene is loaded
    private void Awake()
    {
        _EnemiesList = FindObjectsOfType<Enemy>();// find all the enemies in the scene

        NUMOFCHARA = _EnemiesList.Length + 1;
        tiles = GameObject.FindGameObjectsWithTag("tile");// fing all the tiles in the scene
        FindAdjacentTiles();// find the adjacent tile for each tile
    }

    #region  findtiles
    // find the tile which player stands on
    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);// pass the gameobject to function GetTargetTile()
        if (gameObject.tag == "Player")
        {
            currentTile.SetHasCharacter();// set the current tile to have character
        }
    }

    // using raycast to find the tile that is under the player
    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;
        Vector3 origin = target.transform.position;
        // create a raycast to check
        if (Physics.Raycast(origin, -Vector3.up, out hit, 1f, _GroundMask))
        {
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    // find the adjacent tiles for each tiles inside the scene
    public void FindAdjacentTiles()
    {
        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors();// find the neighbor tiles from all four direction
        }
    }

    //find all the tiles that the player can walk on
    public void FindSelectableTiles(int numofmovement, int _IDNum)
    {
        GetCurrentTile();//get the current tile which the player stands on
        selectableTiles.Clear();// clear the previews tiles inside the list
        currentTile.SetVisited(_IDNum);// set the current tile to visited

        Queue<Tile> process = new Queue<Tile>();// create a queue to store all the tiles, and wait for processing
        process.Clear();// clear the previews tiles insid the queue
        process.Enqueue(currentTile);//put the current tile inside the queue, and wait for processing

        while (process.Count > 0)// run through the process queue until it is empty
        {
            Tile t = process.Dequeue();//get the first element in the queue

            selectableTiles.Add(t);// assign the that tile to the selectableTile list
            currentTile.SetParent(null, _IDNum);
            t.SetSelectable();
            if (t.GetDistance(_IDNum) < numofmovement)
            {
                foreach (Tile tmp in t.adjacentTile)
                {
                    if (!tmp.GetVisited(_IDNum) && !tmp.GetHasEnemy() && !tmp.GetHasWall())
                    {
                        tmp.SetParent(t, _IDNum);
                        tmp.SetVisited(_IDNum);
                        tmp.SetDistance(t.GetDistance(_IDNum) + 1, _IDNum);
                        process.Enqueue(tmp);
                    }
                }
            }// find the path based on how many tiles the player can walk per turn
        }// exam alll the element inside the queue
    }
    public void FindSelectableTiles(int maxMovement, int numofmovement, int _IDNum)
    {
        GetCurrentTile();//get the current tile which the player stands on
        selectableTiles.Clear();// clear the previews tiles inside the list
        currentTile.SetVisited(_IDNum);// set the current tile to visited

        Queue<Tile> process = new Queue<Tile>();// create a queue to store all the tiles, and wait for processing
        process.Clear();// clear the previews tiles insid the queue
        process.Enqueue(currentTile);//put the current tile inside the queue, and wait for processing

        while (process.Count > 0)// run through the process queue until it is empty
        {

            Tile t = process.Dequeue();//get the first element in the queue

            selectableTiles.Add(t);// assign the that tile to the selectableTile list
            currentTile.SetParent(null, _IDNum);
            if (t.GetDistance(_IDNum) < maxMovement)
            {
                foreach (Tile tmp in t.adjacentTile)
                {
                    if (!tmp.GetVisited(_IDNum))
                    {
                        tmp.SetParent(t, _IDNum);
                        tmp.SetVisited(_IDNum);
                        tmp.SetDistance(t.GetDistance(_IDNum) + 1, _IDNum);
                        process.Enqueue(tmp);
                    }
                }
            }// find the path based on how many tiles the player can walk per turn

        }// exam alll the element inside the queue
    }
    #endregion

    #region  movement

    public void MakePath(Tile tile)
    {
        path.Clear();// clear the path list 
        Tile next = tile;

        while (next != null)
        {
            _AudioSource.clip = _clipMove;
            _AudioSource.Play();
            _ChrAnim.SetTrigger("isMove");
            next.SetIsPath();
            path.Push(next);// push the tile into the stack
            next = next.GetParent(_IDNum);
        }

    }

    //add movement to the player character
    protected void Move()
    {
        if (GetHasActionPoint() && path.Count > 0)
        {
            Tile t = path.Peek();//peek the first tile in the stack
            Vector3 target = t.transform.position;// get the first element in the stack's position

            target.y += 2 * _HalfHeight;// adjust the y value of the target

            Vector3 heading = target - transform.position;// get the vector of where the player should be heading
            Vector3 norm = heading.normalized;// normalized the heading vector
            // Vector3 velocity = norm * _MovementSpeed;
            // float v = velocity.magnitude;
            if ((target - transform.position).magnitude <= 0.05f)
            {

                path.Pop();// remove the first element in the stack

            }
            else
            {
                transform.forward = norm;// adjust where the player is facing
                transform.position = Vector3.MoveTowards(transform.position, target, _MovementSpeed * Time.deltaTime);// use Time.delataTime to create a smooth movement
            }
            ismoving = true;
        }
        else
        {
            _ChrAnim.SetTrigger("isIdle");
            UseActionPoint(1);
            apCounter--;
            Reset();
            _GH._IsUsed = false;// reset the rapling hook
            GetCurrentTile();
            if (currentTile.IsGoal())
            {
                Scene scene = SceneManager.GetActiveScene();
                if (scene.name == "Level 1")
                {
                    SceneManager.LoadSceneAsync("Level 2", LoadSceneMode.Single);// load the win scene when touch the goal
                }
                else
                {

                    SceneManager.LoadSceneAsync("EndGame", LoadSceneMode.Single);// load the win scene when touch the goal
                }
            }

        }
    }
    public void Reset()
    {
        foreach (Tile t in selectableTiles)
        {

            t.Reset();
        }
        _ClickedOnPlayer = false;
        ismoving = false;
    }// reset all states

    public bool IsClickedOnPlayer()
    {
        return _ClickedOnPlayer;
    }

    public void MoveTo(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.forward = direction;
        _GH.transform.position = target;
        transform.Translate(Vector3.forward * _MovementSpeed * Time.deltaTime, Space.Self);
        if ((target - transform.position).magnitude <= 0.1f)
        {
            _GH._IsFinishedMoving = false;
            _ChrAnim.SetTrigger("isIdle");
            // transform.rotation.x = 0;
        }
    }
    public bool MoveToByLocation(Vector3 target, Vector3 direction)
    {
        Vector3 heading = (target - transform.position).normalized;
        transform.Translate(direction * _MovementSpeed * 0.5f * Time.deltaTime, Space.World);
        //Debug.Log("target - transform.position: " + (target - transform.position).magnitude);
        if ((target - gameObject.transform.position).magnitude >= 0.1f)
        {
            return true;
        }
        return false;
    }

    //Animations called to other scripts
    public void ClimbLadderAnimation()
    {
        _ChrAnim.SetTrigger("isClimb");
    }

    public void IdleAnimation()
    {
        _ChrAnim.SetTrigger("isIdle");
    }

    public void GrappleAnimation()
    {
        _ChrAnim.SetTrigger("isGrapple");
    }

    #endregion

    public bool GetIsmoveing()
    {
        return ismoving;
    }

    public void TakeDamage(int _AttackDamage)
    {
        _Health -= _AttackDamage;
        _AudioSource.clip = _clipPlayerDamaged;
        _AudioSource.Play();
        _ChrAnim.SetTrigger("isDamaged");
        if (_Health <= 0)
        {
            if (gameObject.tag == "Player")
            {
                _AudioSource.clip = _clipPlayerDie;
                _AudioSource.Play();
                _ChrAnim.SetTrigger("isDead");
                UIScenesManager.ShowUI<DeathUI>();
            }
            else
            {
                _ChrAnim.SetTrigger("isDead");
                Instantiate(_deathSplash,transform.position,transform.rotation);
                DetachChild();

                gameObject.SetActive(false);
                currentTile.SetHasNoEnemy();
            }
        }

    }
    public int GetHealth()
    {
        return _Health;
    }

    private Enemy_Death_DetachParent _EnemyMesh;

    private void DetachChild()
    {
        _EnemyMesh = GetComponentInChildren<Enemy_Death_DetachParent>();
        _EnemyMesh.SeperateMesh();
    }


    #region engergy
    public void UseEnergy(int amount)
    {
        _EnergyUsed += amount;
    }
    public int GetEnergyUsed()
    {
        return _EnergyUsed;
    }

    public int GetEnergyTotal()
    {
        return _Energy;
    }

    protected void ResetEnergy()
    {
        _EnergyUsed = 0;
    }

    #endregion

    #region ActionPoint
    public bool GetHasActionPoint()
    {
        return (actionPoint > _ActionPointUsed);
    }
    public void ResetActionPoint()
    {
        _ActionPointUsed = 0;
        apCounter = 2;
    }

    public void UseActionPoint(int amount)
    {
        _ActionPointUsed += amount;    
    }

    public int GetAP()
    {
        return _ActionPointUsed;
    }


    #endregion
}
