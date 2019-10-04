using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HackingCounter : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI _Text;
    [SerializeField] private DoorControl _Door;

    public int _NumOfHacktile;
    public int _Test;

    private static int _HackCounter;

    private void Awake()
    {
        EntanceMiniGame[] tmp = FindObjectsOfType<EntanceMiniGame>();
        _NumOfHacktile = tmp.Length;
        // Debug.Log(_NumOfHacktile);
    }

    public static void OnHackSuccess () 
    {
        _HackCounter++;
    }

    private void Update()
    {
        _Text.text = "Hack Count: " + _HackCounter;
        if(_NumOfHacktile == _HackCounter)
        {
            _Door.OpenDoor();
            //open door!!!! or something
        }
    }
}