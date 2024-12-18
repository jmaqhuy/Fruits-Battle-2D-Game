using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTransfer;
using NetworkThread;
using NetworkThread.Multiplayer.Packets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManageUIScript : MonoBehaviour
{
    public GameObject CharacterInfoPanel;
    public CharacterInfoTabScript CharacterInfoTabScript;
    private CharacterPacket _currentCharacter;
    
    
    public GameObject SelectCharacterPanel;
    public SelectCharacterTabScript SelectCharacterTabScript;
    
    public CharactersData CharactersData;
    
    private Animator _animator;
    
    void Start()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        var cp = CharactersData.Characters
            .FirstOrDefault(c => c.IsSelected);
        UpdateCharacterInfoPanel(cp);
    }

    private void UpdateCharacterInfoPanel(CharacterPacket cp)
    {
        var name = cp.CharacterName;
        var level = cp.CharacterLevel;
        var exp = cp.CharacterXp;
        var hp = cp.CharacterHp;
        var damage = cp.CharacterDamage;
        var armor = cp.CharacterArmor;
        var luck = cp.CharacterLuck;
        var hpPoint = cp.HpPoint;
        var damagePoint = cp.DamagePoint;
        var armorPoint = cp.ArmorPoint;
        var luckPoint = cp.LuckPoint;
        CharacterInfoTabScript.ParseCharacterProperties(hpPoint, damagePoint, armorPoint, luckPoint,
            level-1-(hpPoint + damagePoint + armorPoint + luckPoint));
        CharacterInfoTabScript.ParseCharacterBaseInfo(name, level, exp, hp, damage, armor, luck);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
