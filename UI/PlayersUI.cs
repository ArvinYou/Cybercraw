using UnityEngine;
using UnityEngine.UI;

public class PlayersUI : MonoBehaviour
{
    [SerializeField] private Player _Player = null;

    [SerializeField] private Image _Health1 = null;
    [SerializeField] private Image _Health2 = null;
    [SerializeField] private Image _Health3 = null;

    [SerializeField] private Image _EnergyBar = null;
    [SerializeField] private Image _grapple = null;

    // private int _healthCount = 3;

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


        _EnergyBar.fillAmount = (float)(_Player.GetEnergyTotal() - _Player.GetEnergyUsed()) / (float)_Player.GetEnergyTotal();

    }

    private void OnMouseOver()
    {
        _grapple.color = Color.gray;
    }


}
