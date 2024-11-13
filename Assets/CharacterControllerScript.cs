using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterControllerScript : MonoBehaviour
{
    public Image powerImage;
    public GameObject powerEffect;
    public Button powerButton;
    void Start()
    {
        resetPowerFillAmount();
        
    }
    
    void Update()
    {
        if (powerImage.fillAmount >= 1.0F)
        {
            powerEffect.SetActive(true);
            powerButton.onClick.AddListener(OnPowerButtonClicked);
        }
        
    }
    void OnPowerButtonClicked()
    {
        resetPowerFillAmount();
        powerEffect.SetActive(false);
    }

    void resetPowerFillAmount()
    {
        powerImage.fillAmount = 0;
    }
}
