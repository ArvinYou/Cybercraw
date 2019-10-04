using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TacticMovement : MonoBehaviour
{
    //Serialize Field
    [Header("Programming Stuff")]
    [SerializeField] private Player p = null;
    [SerializeField] private TextMeshProUGUI _TMP = null;
    [SerializeField] private LayerMask _CameraMask = new LayerMask();
    [SerializeField] private GameObject _CameraControl = null;
    [SerializeField] private Button _EndTurnButton = null;

    // variables
    private Enemy[] _EnemiesList;
    public RaycastHit _Hit;
    private Ray _Ray;
    protected GameObject _object;
    public bool _CanMove = false;
    private Tile t;
    private string _GameObjectTag;
    public static int count = 1;

    //UI
    [SerializeField] private GameObject _pTurn;
    [SerializeField] private GameObject _eTurn;
    [SerializeField] private GameObject _pVignet;
    [SerializeField] private GameObject _eVignet;
    [SerializeField] protected GameObject _apCounter1;
    [SerializeField] protected GameObject _apCounter2;

    private void Awake()
    {
        _EnemiesList = FindObjectsOfType<Enemy>();// find all the enemies in the scene
    }

    // Update is called once per frame
    private void Update()
    {
        if (count % 2 == 0)
        {
            EnemyTurn();
            _TMP.SetText("");
            //Debug.Log("Enemy's turn");
        }// when the remainder is 0 then its enemies' turn
        else if (count % 2 == 1)
        {
            Playerturn();
            // Debug.Log("Player's turn");
            _EndTurnButton.gameObject.SetActive(true);
        }// when the remainder is 1 then its player's turn
        //read the mouse input
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (p.GetHasActionPoint())
            {
                _CameraControl.GetComponent<CameraControl>().SetCenterToCharacter();
                // p = _Hit.collider.GetComponent<Player>();
                p.Onclick();
            }
        }
    }

    private void Playerturn()
    {
        _eTurn.SetActive(false);
        _eVignet.SetActive(false);
        _pTurn.SetActive(true);
        _pVignet.SetActive(true);

        _apCounter1.SetActive(true);
        _apCounter2.SetActive(true);

        if (Input.GetButtonDown("Fire1"))
        {

            if (EventSystem.current.IsPointerOverGameObject())
            {
                p.Reset();
            }
            else
            {
                _Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_Ray, out _Hit, 100, _CameraMask))
                {
                    //Debug.Log(_Hit.transform.name, _Hit.transform);
                    _GameObjectTag = _Hit.collider.gameObject.tag;//get the type of object been clicked
                    if (_GameObjectTag == "Player")
                    {
                        if (p.GetHasActionPoint())
                        {
                            _CameraControl.GetComponent<CameraControl>().SetCenterToCharacter();
                            // p = _Hit.collider.GetComponent<Player>();
                            p.Onclick();
                        }
                        else
                        {
                            _TMP.SetText("You have no more action point!");
                        }
                    }//If the player character has been clicked
                    else if (_GameObjectTag == "tile")
                    {
                        t = _Hit.collider.GetComponent<Tile>();
                        if (t.IsSelectable() && !t.GetHasCharacter() && !p.ismoving)
                        {
                            t.Onclick();//call Tile's OnClick() funcation
                            p.MakePath(t);// create the path
                            p.ismoving = true;
                            // Debug.Log("End player turn!!!!!!!!!");
                            // Debug.Log("-------------------------------------------------");
                        }// if the tile been clicked is selectable then move the player to that location
                        else if (t.GetCurrentColor() == Color.white)
                        {
                            p.Reset();// reset the player (treat the white tile the same as click on player)
                        }//if the tile been clicked is not selectable then reset the player 
                    }// when player click on the tile
                }
            }

            // create raycast to read where the player clicked on
        }

    }

    private void EnemyTurn()
    {

        var tmp = 0;
        _eTurn.SetActive(true);
        _eVignet.SetActive(true);
        _pTurn.SetActive(false);
        _pVignet.SetActive(false);
        _apCounter1.SetActive(false);
        _apCounter2.SetActive(false);

        foreach (Enemy g in _EnemiesList)
        {
            if (!g.GetIsEnemyMoved())
            {
                g.SetToMove();
            }
        }

        foreach (Enemy g in _EnemiesList)
        {
            if (g.GetIsEnemyMoved() || g.gameObject.activeSelf == false)
            {
                // Debug.Log("Get Is Enemy moved: " + g.GetIsEnemyMoved().ToString());
                tmp++;
            }
        }
        if (tmp == _EnemiesList.Length)
        {
            // Debug.Log("End of enemy's turn");
            count++;
            foreach (Enemy g in _EnemiesList)
            {
                g.ResetActionPoint();
                //Debug.Log(g.transform.gameObject.name + "End of Enemy turn!!!!!!!!!!");
                // Debug.Log("-------------------------------------------------");
            }
        }
        else
        {
            tmp = 0;
        }

    }

}
