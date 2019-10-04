using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Player _Player = null;
    [SerializeField] private GameObject _Health1 = null;
    [SerializeField] private GameObject _Health2 = null;
    [SerializeField] private GameObject _Health3 = null;


    private void Update()
    {
        if (_Player.GetHealth() == 2)
        {
            _Health3.gameObject.SetActive(false);
        }
        else if (_Player.GetHealth() == 1)
        {
            _Health2.gameObject.SetActive(false);
        }
        else if (_Player.GetHealth() == 0)
        {
            _Health1.gameObject.SetActive(false);
        }
    }
}
