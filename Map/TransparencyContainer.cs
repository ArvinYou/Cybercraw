using System;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyContainer : MonoBehaviour
{

    [SerializeField] private TransparentObject[] _Objects;

    private void Awake()
    {
        _Objects = GetComponentsInChildren<TransparentObject>();
    }
    public void ToTrans()
    {
        foreach (TransparentObject t in _Objects)
        {
            t.ToTrans();
        }
    }

    public void ToOrigin()
    {
        foreach (TransparentObject t in _Objects)
        {
            t.ToOrigin();
        }

    }
}