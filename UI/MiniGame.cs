
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MiniGame : UIScenes
{
    // serialized field 
    [SerializeField] private List<Button> _Buttons = new List<Button>();
    [SerializeField] private List<Image> _Images = new List<Image>();

    // variables
    private bool _Result = true;

    // lists
    private List<int> _ButtonClicked = new List<int>();
    private List<Color> _Order = new List<Color>();
    private List<Color> _Colors = new List<Color> { Color.red, Color.blue, Color.yellow, Color.green };
    private List<Color> _ColorsClone = null;

    private void OnEnable()
    {
        Time.timeScale = 0f;
        int tmp = 0;
        _ColorsClone = new List<Color>(_Colors);
        while (_Colors.Count > 0)
        {
            tmp = (int)Random.Range(0, _Colors.Count);
            _Order.Add(_Colors[tmp]);
            _Colors.RemoveAt(tmp);
        }
        for (int i = 0; i < _Order.Count; i++)
        {
            _Images[i].color = _Order[i];
            ColorBlock color = _Buttons[i].colors;
            // Debug.Log("Button Color before: " + _Buttons[i].colors.normalColor.ToString());
            color.normalColor = _ColorsClone[i];
            _Buttons[i].colors = color;
            // Debug.Log("Button Color After: " + _Buttons[i].colors.normalColor.ToString());
        }
        // foreach (var item in _Order)
        // {
        //     Debug.Log("_Order list: " + item.ToString());
        // }
        // foreach (var item in _Buttons)
        // {
        //     Debug.Log("Button Color: " + item.colors.normalColor.ToString());
        // }

    }

    // get the index of the button clicked
    public void GetButtonIndex(int index)
    {
        if (index == 0)
        {
            _ButtonClicked.Add(index);
            //Debug.Log("_ButtonOneClicked: " + _ButtonOneClicked.ToString());
        }
        else if (index == 1)
        {
            _ButtonClicked.Add(index);
            //Debug.Log("_ButtonTwoClicked: " + _ButtonTwoClicked.ToString());
        }
        else if (index == 2)
        {
            _ButtonClicked.Add(index);
            //Debug.Log("_ButtonThreeClicked: " + _ButtonThreeClicked.ToString());
        }
        else if (index == 3)
        {
            _ButtonClicked.Add(index);
            //Debug.Log("_ButtonFourClicked: " + _ButtonFourClicked.ToString());
        }
    }

    // when the index of the list is equals to 4 then exam the answer
    private void Update()
    {
        if (_ButtonClicked.Count == 4)
        {
            CheckForResult();
        }
    }

    // reset the bool variablies
    public void Reset()
    {
        _Result = true;
        _ButtonClicked.Clear();
        // Debug.Log("Count: " + _ButtonClicked.Count);
    }

    // check if the answer matches the right order
    private void CheckForResult()
    {
        int tmp = 0;
        foreach (int i in _ButtonClicked)
        {
            // Debug.Log("Color at " + tmp + " is: " + _Buttons[i].colors.normalColor);
            // Debug.Log("Color at " + tmp + " in Order is: " + _Order[tmp]);
            _Result = (_Result && _Buttons[i].colors.normalColor == _Order[tmp]);
            tmp++;
        }
        // Debug.Log("Checking the answer!" + _Result.ToString());
        if (_Result)
        {
            UIScenesManager.ShowUI<GamePlayUI>();
            HackingCounter.OnHackSuccess();
            // Debug.Log("Exit!");
        }// if the answer is correct then unload the scene
        Reset();
    }
}
