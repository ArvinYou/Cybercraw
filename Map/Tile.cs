
using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private bool _HasWall = false;
    [SerializeField] private bool _IsGoal = false;
    [SerializeField] private bool _IsCheckpoint = false;
    [SerializeField] private bool _IsFakeWall = false;
    [SerializeField] private GameObject _MovementIndicator;



    private bool _HasCharacter = false;
    private bool _Selectable = false;
    private bool _IsInGHRange = false;
    private bool _HasEnemy = false;
    private Color currentColor;
    private Rigidbody _rigidbody;
    private GameObject _CurrentMovementIndicator;

    // private bool _CanGo = false;
    private bool _Target = false;
    private bool _IsPath = false;

    private bool[] _Visited;
    private Tile[] _Parent;
    private int[] _Distance;
    private Vector3 _MovementIndicatorSpawnLocation;

    public List<Tile> adjacentTile = new List<Tile>();

    // get rigidbody
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _MovementIndicatorSpawnLocation = transform.position + Vector3.up;
        //CheckTileOnTop();
    }

    private void Start()
    {
        _Visited = new bool[Characters.NUMOFCHARA];
        _Distance = new int[Characters.NUMOFCHARA];
        _Parent = new Tile[Characters.NUMOFCHARA];
        for (int i = 0; i < Characters.NUMOFCHARA; i++)
        {
            _Visited[i] = false;
            _Distance[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (_HasCharacter)
        {
            this.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green); ;
            currentColor = Color.green;
        }// when there is a player on top of a tile
        else if (_IsGoal)
        {
            this.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.magenta); ;
            currentColor = Color.magenta;
        }
        else if (_Target)
        {
            this.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.blue); ;
            currentColor = Color.blue;
        }// when player set a tile to target
        else if (_HasWall)
        {
            this.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.blue);
            currentColor = Color.black;
        }// when there is a wall
        else if (_IsPath)
        {
            //enemies path
            // this.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.yellow); ;
            // currentColor = Color.yellow;
        }
        else if (_Selectable)
        {
            this.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
            currentColor = Color.red;
        }// when the tile is selectable
        else if (_IsCheckpoint)
        {
            // this.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.cyan);
            // currentColor = Color.cyan;
        }// when the tile is a enemy checkpoint
        else if (_HasEnemy)
        {
            this.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green);
            currentColor = Color.green;
        }// when the tile has enemy on top
        else if (_IsInGHRange)
        {
            this.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
            currentColor = Color.red;
        }// when the tile is in the grappling hook range
        else if (_IsFakeWall)
        {
            this.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.clear);
            currentColor = Color.clear;
        }// when the tile is a fake wall
        else
        {
            this.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black); ;
            currentColor = Color.white;
        }//when it is a normal tile



    }

    private void OnMouseEnter()
    {
        if (_Selectable)
        {
            _CurrentMovementIndicator = Instantiate(_MovementIndicator, _MovementIndicatorSpawnLocation, Quaternion.identity);
        }
    }

    private void OnMouseExit()
    {
        Destroy(_CurrentMovementIndicator);
    }

    public void Reset()
    {
        for (int i = 0; i < Characters.NUMOFCHARA; i++)
        {
            _Visited[i] = false;
            _Distance[i] = 0;
        }
        _Selectable = false;
        _HasCharacter = false;
        _Target = false;
        _IsPath = false;
        //_HasEnemy = false;
        _IsInGHRange = false;
    }// reset tile's states

    public void SetHasCharacter()
    {
        this._HasCharacter = true;
    }// set _HasCharacter to true
    public void SetHasEnemy()
    {
        this._HasEnemy = true;
    }
    public void SetHasNoEnemy()
    {
        this._HasEnemy = false;
        // Debug.Log("SetHasNoEnemy() called");
    }
    public bool GetIsFakeWall()
    {
        return _IsFakeWall;
    }
    public void FindNeighbors()
    {
        CheckTile(Vector3.forward);
        CheckTile(-Vector3.forward);
        CheckTile(Vector3.right);
        CheckTile(-Vector3.right);
    }//check for tiles adjcent to this tile

    public void CheckTile(Vector3 direction)
    {
        Vector3 halfExtents = new Vector3(0.25f, 0.5f, 0.25f);
        Collider[] hitColliders = Physics.OverlapBox(transform.position + direction, halfExtents);
        foreach (Collider c in hitColliders)
        {
            Tile tile = c.GetComponent<Tile>();
            if (tile != null && !tile._HasWall && !tile._HasCharacter && !tile.GetIsFakeWall())
            {
                adjacentTile.Add(tile);
            }
        }
    }// check the is there a tile on the assigned direction and add that tile to the adjacentTIle list

    public bool Walkable()
    {
        return !_HasCharacter && !_HasWall;
    }

    public void SetVisited(int id)
    {
        _Visited[id] = true;
    }

    public void SetSelectable()
    {
        _Selectable = true;
    }

    public bool GetVisited(int id)
    {
        return _Visited[id];
    }

    public void SetParent(Tile t, int id)
    {
        _Parent[id] = t;
    }

    public void SetIsWall()
    {
        _HasWall = true;
    }

    public Tile GetParent(int id)
    {
        return _Parent[id];
    }

    public void SetDistance(int i, int id)
    {
        _Distance[id] = i;
    }

    public int GetDistance(int id)
    {
        return _Distance[id];
    }

    public void SetTarget()
    {
        _Target = true;
    }

    public bool IsSelectable()
    {
        return _Selectable;
    }

    public Color GetCurrentColor()
    {
        return currentColor;
    }
    public void SetIsPath()
    {
        _IsPath = true;
    }
    public bool IsGoal()
    {
        return _IsGoal;
    }

    public void Onclick()
    {
        _Target = true;
    }// when the target has been clicked

    public void SetIsCheckPoint()
    {
        _IsCheckpoint = true;
    }

    public void SetIsInGHRange()
    {
        _IsInGHRange = true;
    }
    public void SetIsNotInGHRange()
    {
        _IsInGHRange = false;
    }

    public bool GetIsInGHRange()
    {
        return _IsInGHRange;
    }

    public bool GetHasWall()
    {
        return _HasWall;
    }

    public bool GetHasCharacter()
    {
        return _HasCharacter || _HasEnemy;
    }

    public bool GetHasEnemy()
    {
        return _HasEnemy;
    }

    public void SetNotPath()
    {
        _IsPath = false;
    }

    public void SetIsNotWall()
    {
        _HasWall = false;
    }
}
