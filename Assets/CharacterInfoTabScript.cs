using System;
using System.Linq;
using DataTransfer;
using NetworkThread;
using NetworkThread.Multiplayer.Packets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoTabScript : MonoBehaviour
{
    [Header("Characters Data")]
    public CharactersData characterData;

    private CharacterPacket character;
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
    
    private int _availablePoint;
    
    private int _newHpPoint;
    private int _newDamagePoint;
    private int _newArmorPoint;
    private int _newLuckyPoint;
    private int _remainingPoints;

    private void Start()
    {
        saveAddPointButton.onClick.AddListener(OnClickSaveAddPoint);
        character = characterData.Characters
            .FirstOrDefault(c => c.IsSelected);
    }

    private void OnClickSaveAddPoint()
    {
        character.HpPoint = _newHpPoint;
        character.DamagePoint = _newDamagePoint;
        character.ArmorPoint = _newArmorPoint;
        character.LuckPoint = _newLuckyPoint;
        _availablePoint = _remainingPoints;
        NetworkStaticManager.ClientHandle.SendChangeCharacterPointPacket(character);
        UpdateCharacterInformation();
        UpdateButtonStates();
    }

    public void ParseCharacterBaseInfo()
    {
        
        if (character != null)
        {
            characterName.text = character.CharacterName;
            foreach (var cl in characterLevel)
            {
                cl.text = character.CharacterLevel.ToString();
            }
            characterExperience.text = "Exp: " + character.CharacterXp;
            
            _newHpPoint = character.HpPoint;
            _newDamagePoint = character.DamagePoint;
            _newArmorPoint = character.ArmorPoint;
            _newLuckyPoint = character.LuckPoint;
            _availablePoint = _remainingPoints = character.CharacterLevel - 1 -
                                                 ( character.HpPoint 
                                                 + character.DamagePoint
                                                 + character.ArmorPoint
                                                 + character.LuckPoint);
            UpdateCharacterInformation();
        }
    }

    private void UpdateCharacterInformation()
    {
        characterHp.text = (character.CharacterHp + character.HpPoint * 100).ToString();
        characterDamage.text = (character.CharacterDamage + character.DamagePoint * 10).ToString();
        characterArmor.text = (character.CharacterArmor + character.ArmorPoint * 10).ToString();
        characterLucky.text = (character.CharacterLuck + character.LuckPoint).ToString();
        characterHp.color = Color.black;
        characterDamage.color = Color.black;
        characterArmor.color = Color.black;
        characterLucky.color = Color.black;
        characterHpPoint.text = character.HpPoint.ToString();
        characterDamagePoint.text = character.DamagePoint.ToString();
        characterArmorPoint.text = character.ArmorPoint.ToString();
        characterLuckyPoint.text = character.LuckPoint.ToString();
        availablePoint.text = _availablePoint.ToString();
        UpdateButtonStates();
    }

    public void AddHpPoint() => AdjustPoint(ref _newHpPoint, characterHpPoint, characterHp, 1,1);

    public void AddDamagePoint() => AdjustPoint(ref _newDamagePoint, characterDamagePoint, characterDamage, 1,2);
    
    public void AddArmorPoint() => AdjustPoint(ref _newArmorPoint, characterArmorPoint, characterArmor, 1,3);
    
    public void AddLuckyPoint() => AdjustPoint(ref _newLuckyPoint, characterLuckyPoint, characterLucky, 1,4);

    public void RemoveHpPoint() => AdjustPoint(ref _newHpPoint, characterHpPoint, characterHp, -1,1);

    public void RemoveDamagePoint() => AdjustPoint(ref _newDamagePoint, characterDamagePoint, characterDamage, -1,2);

    public void RemoveArmorPoint() => AdjustPoint(ref _newArmorPoint, characterArmorPoint, characterArmor, -1,3);

    public void RemoveLuckyPoint() => AdjustPoint(ref _newLuckyPoint, characterLuckyPoint, characterLucky, -1,4);
    
    private void AdjustPoint(ref int currentPoint, TextMeshProUGUI pointText, TextMeshProUGUI value, int adjustment, int type)
    {
        currentPoint += adjustment;
        _remainingPoints -= adjustment;
        pointText.text = currentPoint.ToString();
        availablePoint.text = _remainingPoints.ToString();
        value.color = Color.green;
        switch (type)
        {
            default:
                value.text = (character.CharacterHp + currentPoint * 100).ToString();
                break;
            case 2:
                value.text = (character.CharacterDamage + currentPoint * 10).ToString();
                break;
            case 3:
                value.text = (character.CharacterArmor + currentPoint * 10).ToString();
                break;
            case 4:
                value.text = (character.CharacterLuck + currentPoint * 10).ToString();
                break;
        }
        UpdateButtonStates();
    }
    
    private int TotalPointChange()
    {
        return (_newHpPoint - character.HpPoint)
            + (_newDamagePoint - character.DamagePoint)
            + (_newArmorPoint - character.ArmorPoint)
            + (_newLuckyPoint - character.LuckPoint);
    }

    public void CancelAddPoint()
    {
        _newHpPoint = character.HpPoint;
        _newDamagePoint = character.DamagePoint;
        _newArmorPoint = character.ArmorPoint;
        _newLuckyPoint = character.LuckPoint;
        _remainingPoints = _availablePoint;
        
        UpdateCharacterInformation();
        UpdateButtonStates();
    }
    public void UpdateButtonStates()
    {
        minusPointHp.interactable = character.HpPoint != _newHpPoint;
        minusPointDamage.interactable = character.DamagePoint != _newDamagePoint;
        minusPointArmor.interactable = character.ArmorPoint != _newArmorPoint;
        minusPointLucky.interactable = character.LuckPoint != _newLuckyPoint;
        
        addPointButton.interactable = _availablePoint != 0;
        
        saveAddPointButton.interactable = _availablePoint != _remainingPoints;
        resetPointsButton.interactable = (character.HpPoint +
                                          character.DamagePoint +
                                          character.ArmorPoint +
                                          character.LuckPoint) != 0;

        var maxPointsReached = TotalPointChange() == _availablePoint;
        addPointHp.interactable = !maxPointsReached;
        addPointDamage.interactable = !maxPointsReached;
        addPointLucky.interactable = !maxPointsReached;
        addPointArmor.interactable = !maxPointsReached;
    }
}
