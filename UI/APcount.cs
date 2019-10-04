using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class APcount : MonoBehaviour
{
    [SerializeField] private Player _Player;
    [SerializeField] private Image _apImage;

    private void Update()
    {
        if(_Player.GetAP() == 1)
        {
            _apImage.gameObject.SetActive(false);
        }


    }
}
