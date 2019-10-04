using UnityEngine;
using UnityEngine.EventSystems;
public class GrapplingHook : Gadgets
{

    [Header("Grappling Hook Status")]
    [SerializeField] private float _Speed = 500f;
    [SerializeField] private float _Radius = 8f;



    public RaycastHit _Hit;
    private Ray _Ray;
    private Vector3 _MouseClickedPosition = Vector3.zero;
    private Vector3 _Heading;
    private Rigidbody _RB;
    private Vector3 _InitialPosition;
    private float halfHeight = 0.5f;
    private Vector3 _ObjectOnCollsion = Vector3.zero;
    private bool _OnHit = false;
    private bool _GrapplePointHit = false;
    public bool _IsUsed = false;
    public bool _IsFinishedMoving = false;
    private Vector3 _StartingPoint;
    private Vector3 _EndingPoint;
    private Collider[] hitColliders = null;
    private Player _Player;

    private void Awake()
    {
        _Player = p.GetComponent<Player>();
        _RB = GetComponent<Rigidbody>();
        _InitialPosition = transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            ButtonClicked();
        }
        if (_IsButtonClicked && !_IsUsed)
        {
            _Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Input.GetButtonDown("Fire1"))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log(EventSystem.current);
                    Debug.Log("Is over UI");
                    ResetInitialPosisiton();
                }
                else if (Physics.Raycast(_Ray, out _Hit))
                {
                    //Debug.Log("Is NOT UI");
                    Tile t = _Hit.collider.GetComponent<Tile>();
                    GrapplePoint g = _Hit.collider.GetComponent<GrapplePoint>();
                    Debug.Log("Grapple Point: " + (g != null));
                    if (t != null && t.GetIsInGHRange())
                    {
                        _MouseClickedPosition = t.transform.position;
                        _MouseClickedPosition.y += halfHeight;
                        _Heading = _MouseClickedPosition - transform.position;
                        GetComponent<Collider>().enabled = true;
                        //Debug.Log("Shooting");
                        Shoot();
                        _IsButtonClicked = false;
                        _sfxGadgetSource.clip = _sfxGadgetClip;
                        _sfxGadgetSource.Play();

                    }
                    else if (g != null && g.GetIsPlayerInRange())
                    {

                        Debug.Log("GrapplePoint:  " + g.ToString());
                        _MouseClickedPosition = g.transform.position;
                        _MouseClickedPosition.y += halfHeight;
                        _Heading = _MouseClickedPosition - transform.position;
                        GetComponent<Collider>().enabled = true;
                        //Debug.Log("Shooting");
                        Shoot();
                        _IsButtonClicked = false;
                        _sfxGadgetSource.clip = _sfxGadgetClip;
                        _sfxGadgetSource.Play();
                    }
                    else
                    {
                        _TMP.SetText("You can't shoot toward that position!");
                        _IsButtonClicked = false;
                        ResetInitialPosisiton();
                    }
                }
            }
        }
        else
        {
            if (hitColliders != null)
            {
                Reset();
            }
        }
        // else if (_IsUsed && !p.GetIsTurnEnd())
        // {
        //     _TMP.SetText("You have used the grapling hook!!!");
        // }
        if (_OnHit || _GrapplePointHit)
        {

            p.MoveTo(_ObjectOnCollsion);
            if (!_IsFinishedMoving)
            {
                p.UseEnergy(_EnergyNeeded);
                p.UseActionPoint(_ActionPointNeeded);
                ResetInitialPosisiton();
                p.ResetRotation();

            }
            _IsUsed = true;
        }
        //move the player
    }

    public void SetNotUsed()
    {
        _IsUsed = false;
    }

    private void Reset()
    {
        foreach (Collider c in hitColliders)
        {
            Tile t = c.GetComponent<Tile>();
            if (t != null && !t.GetHasWall())
            {
                //Debug.Log("setting is in gh range.");
                t.SetIsNotInGHRange();
                //Debug.Log(t.GetIsInGHRange());
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        GrapplePoint g = other.collider.GetComponent<GrapplePoint>();
        Tile t = other.collider.GetComponent<Tile>();
        // Debug.Log("Object on hit: " + other.transform.ToString());
        if (t != null && !t.GetHasWall() && !t.GetIsFakeWall())
        {
            _ObjectOnCollsion = other.transform.position;
            _ObjectOnCollsion.y += 2 * halfHeight;
            ResetInitialPosisiton();
            _OnHit = true;
        }
        else if (g != null)
        {
            _ObjectOnCollsion = g.GetTileAttached().transform.position;
            _ObjectOnCollsion.y += 2 * halfHeight;
            ResetInitialPosisiton();
            _GrapplePointHit = true;
        }
        else
        {
            _TMP.SetText("You cannot reach there!");
            ResetInitialPosisiton();
        }
    }

    private void Shoot()
    {
        //move the hook
        if (_Heading.magnitude >= 0.05f)
        {
            _Player.GrappleAnimation();
            _RB.isKinematic = false;
            _RB.AddForce(_Heading.normalized * _Speed);
        }
    }

    private void ResetInitialPosisiton()
    {
        transform.localPosition = _InitialPosition;
        _RB.isKinematic = true;
        _GrapplePointHit = false;
        _OnHit = false;
        GetComponent<Collider>().enabled = false;
        _IsFinishedMoving = true;
        if (hitColliders != null)
        {
            foreach (Collider c in hitColliders)
            {
                Tile t = c.GetComponent<Tile>();
                if (t != null && t.GetIsInGHRange())
                {
                    t.Reset();
                }
            }
        }
    }//reset the position and states

    public void ButtonClicked()
    {
        UiSound();
        //Debug.Log("Button clicked");
        if ((p.GetEnergyUsed() + _EnergyNeeded) <= p.GetEnergyTotal())
        {
            _IsButtonClicked = !_IsButtonClicked;
            if (_IsButtonClicked)
            {
                FindTilesInGHRange();
            }
        }
        else
        {
            _TMP.SetText("Energy Not Enough!");
        }
        if (_IsUsed)
        {
            _TMP.SetText("You have used the grappling hook!!!");
        }
    }

    private void FindTilesInGHRange()
    {
        if (!p.IsClickedOnPlayer() && !_IsUsed)
        {
            _StartingPoint = transform.position + new Vector3(0f, 3f, 0f);
            _EndingPoint = transform.position + new Vector3(0f, -3f, 0f);
            hitColliders = Physics.OverlapSphere(transform.position, _Radius);

            foreach (Collider c in hitColliders)
            {
                Tile t = c.GetComponent<Tile>();
                if (t != null && (transform.position.y > t.transform.position.y) && (transform.position.y - t.transform.position.y) < 2f && !t.GetHasWall() && !t.GetIsFakeWall())
                {
                    //Debug.Log("setting is in gh range.");
                    t.SetIsInGHRange();
                    //Debug.Log(t.GetIsInGHRange());
                }
            }
        }
    }

    private void UiSound()
    {
        _sfxGadgetSource.clip = _sfxUIClip;
        _sfxGadgetSource.Play();
    }

}
