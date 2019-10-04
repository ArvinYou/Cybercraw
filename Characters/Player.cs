using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;

public class Player : Characters
{
    [Header("Programming Stuff")]
    [SerializeField] private UnityEvent _BackStab = null;
    [SerializeField] private Button _BackStabButton = null;
    [SerializeField] private Button _EndTurnButton = null;
    [SerializeField] private int _EnergyCostForEachTerm = 2;

    private bool _CanBackStab = false;
    private bool _IsBackStabUsed = false;
    private Enemy _EnemyInFront = null;
    private bool _EndTheTurn = false;

    [Header("Audio")]
    [SerializeField] private AudioClip _clipInvalid;
    [SerializeField] private AudioClip _clipEnd;
    [SerializeField] private AudioClip _clipOver;
    [SerializeField] private AudioClip _clipPlayer;
    [SerializeField] private AudioClip _enemyKilled;

    [SerializeField] private TextMeshProUGUI _ActionPointText;
    [SerializeField] private Image _PlayerTurn;
    [SerializeField] private GameObject _slashParticle;

    public void Onclick()
    {
        if (!_ClickedOnPlayer)
        {
            _ClickedOnPlayer = true;
            FindSelectableTiles(_NumberOfTilesCanMove, _IDNum);
            _CanBackStab = false;
            _AudioSource.clip = _clipPlayer;
            _AudioSource.Play();
        }
        else
        {
            _AudioSource.clip = _clipInvalid;
            _AudioSource.Play();
            Reset();
        }
    }// when player been clicked

    private void Update()
    {
        _ActionPointText.text = apCounter.ToString();
        if (ismoving)
        {
            Move();
            _CanBackStab = false;
        }// when the path has been found then move the player

        if (_CanBackStab && !_IsBackStabUsed)
        {
            if (_BackStab != null)
            {
                _BackStab.Invoke();
            }
        }
        else if (!_CanBackStab || !_IsBackStabUsed)
        {
            if (_BackStabButton != null)
            {
                // Debug.Log("BackStab Button diactive");
                _BackStabButton.gameObject.SetActive(false);
            }
        }

        if (_EnergyUsed >= _Energy)
        {
            _TMP.SetText("You have used all your energy!");
        }
        else if (_ActionPointUsed >= actionPoint)
        {
            _EndTheTurn = true;
        }

        if (_EndTheTurn)
        {
            TacticMovement.count++;
            ResetActionPoint();
            _EndTheTurn = false;
            _CanBackStab = false;
            _IsBackStabUsed = false;
            _TMP.SetText("");
            _GH.SetNotUsed();
            _EndTurnButton.gameObject.SetActive(false);
            UseEnergy(_EnergyCostForEachTerm);
            _AudioSource.clip = _clipEnd;
            _AudioSource.Play();    
        }
    }

    #region BackStab

    public void BackStab()
    {
        float _backToIdleTimer = 2;
        _backToIdleTimer -= Time.deltaTime;

        _AudioSource.clip = _enemyKilled;
        _AudioSource.Play();
        _ChrAnim.SetTrigger("isBackstab");
        // _ChrAnim.SetTrigger("isIdle");
        this.transform.LookAt(_EnemyInFront.transform);
        Instantiate(_slashParticle,transform.position, transform.rotation);
        _EnemyInFront.TakeDamage(_AttackDamage);
        _CanBackStab = false;
        _IsBackStabUsed = true;
        UseActionPoint(1);
        apCounter--;

    }

    public void SetCanBackStab(GameObject enemy)
    {
        // Debug.Log("SetCanBackStab callled");
        _EnemyInFront = enemy.transform.GetComponent<Enemy>();
        //Debug.Log(_EnemyInFront.ToString());
        _CanBackStab = true;
    }

    public void SetCannotBackStab()
    {
        _CanBackStab = false;
        // Debug.Log("SetCannotBackStab Called");
    }
    #endregion  

    public void SetEndTheTurn()
    {
        _EnemyInFront = null;
        _EndTheTurn = true;
        _CanBackStab = false;
    }

    public bool GetIsTurnEnd()
    {
        return _EndTheTurn;
    }

    public void ResetRotation()
    {
        Vector3 facing = transform.forward;
        facing.y = 0;
        transform.forward = facing;
    }

}
