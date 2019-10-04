
using System;
using System.Collections.Generic;
using UnityEngine;

public class Transparency : MonoBehaviour
{
    [SerializeField] private LayerMask _WallMask = new LayerMask();
    [SerializeField] private float _Angle = 5f;

    private List<Transform> _TargetObjectsOnHit = new List<Transform>();
    private List<Transform> _Tmp = new List<Transform>();
    private Vector3 _Direction;
    private Player _Player;
    private Vector3 _RayCastPosition1;

    private void Awake()
    {
        _Player = FindObjectOfType<Player>();
        
    }


    private void GetAllObjectOnHit()
    {
        _TargetObjectsOnHit.Clear();

        _RayCastPosition1 = _Player.transform.position - Vector3.up;
        // RaycastHit[] hits;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(_RayCastPosition1, _Direction, 200f, _WallMask);
        CombineArray(hits);
        
        Debug.DrawRay(_RayCastPosition1, _Direction * 200f, Color.red);

        foreach (Transform t in _TargetObjectsOnHit)
        {
            // Debug.Log("Trans contains t? " + _Tmp.Contains(t));
            // Debug.Log("2. _Tmp.Count " + _Tmp.Count);
            TransparencyContainer tmp = t.GetComponent<TransparencyContainer>();
            if (tmp != null && !_Tmp.Contains(t))
            {
                tmp.ToTrans();
            }
        }
        foreach (Transform t in _Tmp)
        {
            TransparencyContainer tmp = t.GetComponent<TransparencyContainer>();
            if (tmp != null && !_TargetObjectsOnHit.Contains(t))
            {
                tmp.ToOrigin();
                _Tmp.Remove(t);
            }
        }
        _Tmp = new List<Transform>(_TargetObjectsOnHit);
    }


    private void CombineArray(RaycastHit[] hits)
    {
        foreach (RaycastHit r in hits)
        {
            if (_TargetObjectsOnHit.Contains(r.transform) == false)
            {
                _TargetObjectsOnHit.Add(r.transform);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 initialPosition = gameObject.transform.position;
        initialPosition.y -= _Angle;
        _Direction = (initialPosition - _Player.transform.position).normalized;
        GetAllObjectOnHit();
    }

}
