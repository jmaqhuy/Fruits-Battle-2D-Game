using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoTabScript : MonoBehaviour
{
    public GameObject characterImage;
    public GameObject characterShadow;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI[] characterLevel;
    public TextMeshProUGUI characterExperience;
    public Slider characterExperienceSlider;
    public TextMeshProUGUI availablePoint;
    
    [Header("Character Attributes")]
    public TextMeshProUGUI characterHp;
    public TextMeshProUGUI characterDamage;
    public TextMeshProUGUI characterArmor;
    public TextMeshProUGUI characterLucky;

    public TextMeshProUGUI characterHpPoint;
    public TextMeshProUGUI characterDamagePoint;
    public TextMeshProUGUI characterArmorPoint;
    public TextMeshProUGUI characterLuckyPoint;

    [Header("Normal Panel")] 
    public Button addPointButton;
    public Button resetPointsButton;
    
    [Header("Add Point Panel")]
    public Button cancelAddPointButton;
    public Button saveAddPointButton;
    public Button addPointHp;
    public Button minusPointHp;
    public Button addPointDamage;
    public Button minusPointDamage;
    public Button addPointArmor;
    public Button minusPointArmor;
    public Button addPointLucky;
    public Button minusPointLucky;

    private int _currentHpPoint;
    private int _currentDamagePoint;
    private int _currentArmorPoint;
    private int _currentLuckyPoint;
    private int _availablePoint;
    
    private int _newHpPoint;
    private int _newDamagePoint;
    private int _newArmorPoint;
    private int _newLuckyPoint;
    private int _remainingPoints;

    public void ParseCharacterBaseInfo(string cName, int cLevel, int cExperience, int cHp, int cDamage, int cArmor, int cLucky)
    {
        characterName.text = cName;
        foreach (var cl in characterLevel)
        {
            cl.text = cLevel.ToString();
        }
        characterExperience.text = "Exp: " + cExperience;
        characterHp.text = (cHp + _currentHpPoint * 100).ToString();
        characterDamage.text = (cDamage + _currentDamagePoint * 10).ToString();
        characterArmor.text = (cArmor + _currentArmorPoint * 10).ToString();
        characterLucky.text = (cLucky + _currentLuckyPoint * 10).ToString();
        
    }

    public void ParseCharacterProperties(int cHpPoint, int cDamagePoint, int cArmorPoint, int cLuckyPoint,
        int avPoint)
    {
        _currentHpPoint = _newHpPoint = cHpPoint;
        _currentDamagePoint = _newDamagePoint = cDamagePoint;
        _currentArmorPoint = _newArmorPoint =cArmorPoint;
        _currentLuckyPoint = _newLuckyPoint = cLuckyPoint;
        _availablePoint = _remainingPoints = avPoint;
        
        characterHpPoint.text = _currentHpPoint.ToString();
        characterDamagePoint.text = _currentDamagePoint.ToString();
        characterArmorPoint.text = _currentArmorPoint.ToString();
        characterLuckyPoint.text = _currentLuckyPoint.ToString();
        availablePoint.text = _availablePoint.ToString();
        UpdateButtonStates();
    }

    public void AddHpPoint() => AdjustPoint(ref _newHpPoint, characterHpPoint, 1);

    public void AddDamagePoint() => AdjustPoint(ref _newDamagePoint, characterDamagePoint, 1);
    
    public void AddArmorPoint() => AdjustPoint(ref _newArmorPoint, characterArmorPoint, 1);
    
    public void AddLuckyPoint() => AdjustPoint(ref _newLuckyPoint, characterLuckyPoint, 1);

    public void RemoveHpPoint() => AdjustPoint(ref _newHpPoint, characterHpPoint, -1);

    public void RemoveDamagePoint() => AdjustPoint(ref _newDamagePoint, characterDamagePoint, -1);

    public void RemoveArmorPoint() => AdjustPoint(ref _newArmorPoint, characterArmorPoint, -1);

    public void RemoveLuckyPoint() => AdjustPoint(ref _newLuckyPoint, characterLuckyPoint, -1);
    
    private void AdjustPoint(ref int currentPoint, TextMeshProUGUI pointText, int adjustment)
    {
        currentPoint += adjustment;
        _remainingPoints -= adjustment;
        pointText.text = currentPoint.ToString();
        availablePoint.text = _remainingPoints.ToString();
        UpdateButtonStates();
    }
    
    private int TotalPointChange()
    {
        return (_newHpPoint - _currentHpPoint)
            + (_newDamagePoint - _currentDamagePoint)
            + (_newArmorPoint - _currentArmorPoint)
            + (_newLuckyPoint - _currentLuckyPoint);
    }

    public void CancelAddPoint()
    {
        _newHpPoint = _currentHpPoint;
        characterHpPoint.text = _currentHpPoint.ToString();
        _newDamagePoint = _currentDamagePoint;
        characterDamagePoint.text = _currentDamagePoint.ToString();
        _newArmorPoint = _currentArmorPoint;
        characterArmorPoint.text = _currentArmorPoint.ToString();
        _newLuckyPoint = _currentLuckyPoint;
        characterLuckyPoint.text = _currentLuckyPoint.ToString();
        _remainingPoints = _availablePoint;
        availablePoint.text = _availablePoint.ToString();
        UpdateButtonStates();
    }
    private void UpdateButtonStates()
    {
        minusPointHp.interactable = _currentHpPoint != _newHpPoint;
        minusPointDamage.interactable = _currentDamagePoint != _newDamagePoint;
        minusPointArmor.interactable = _currentArmorPoint != _newArmorPoint;
        minusPointLucky.interactable = _currentLuckyPoint != _newLuckyPoint;
        
        addPointButton.interactable = _availablePoint != 0;
        
        saveAddPointButton.interactable = _availablePoint != _remainingPoints;
        resetPointsButton.interactable = (_currentHpPoint +
                                          _currentDamagePoint +
                                          _currentArmorPoint +
                                          _currentLuckyPoint) != 0;

        var maxPointsReached = TotalPointChange() == _availablePoint;
        addPointHp.interactable = !maxPointsReached;
        addPointDamage.interactable = !maxPointsReached;
        addPointLucky.interactable = !maxPointsReached;
        addPointArmor.interactable = !maxPointsReached;
    }
}
