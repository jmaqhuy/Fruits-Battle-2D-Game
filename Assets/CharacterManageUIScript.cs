using System.Collections;
using System.Collections.Generic;
using NetworkThread;
using NetworkThread.Multiplayer.Packets;
using UnityEngine;

public class CharacterManageUIScript : MonoBehaviour
{
    public GameObject CharacterInfoPanel;
    public CharacterInfoTabScript CharacterInfoTabScript;
    private CharacterPacket _currentCharacter;
    
    
    public GameObject SelectCharacterPanel;
    public SelectCharacterTabScript SelectCharacterTabScript;
    
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        NetworkStaticManager.ClientHandle.SendCurrentCharacterPacket();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetCharacterPacket(CharacterPacket packet)
    {
        _currentCharacter = packet;
        StartCoroutine(UpdateCharacterInfoPanel());
    }

    private IEnumerator UpdateCharacterInfoPanel()
    {
        var name = _currentCharacter.CharacterName;
        var level = _currentCharacter.CharacterLevel;
        var exp = _currentCharacter.CharacterXp;
        var hp = _currentCharacter.CharacterHp;
        var damage = _currentCharacter.CharacterDamage;
        var armor = _currentCharacter.CharacterArmor;
        var luck = _currentCharacter.CharacterLuck;
        var hpPoint = _currentCharacter.HpPoint;
        var damagePoint = _currentCharacter.DamagePoint;
        var armorPoint = _currentCharacter.ArmorPoint;
        var luckPoint = _currentCharacter.LuckPoint;
        CharacterInfoTabScript.ParseCharacterProperties(hpPoint, damagePoint, armorPoint, luckPoint,
            level-1-(hpPoint + damagePoint + armorPoint + luckPoint));
        CharacterInfoTabScript.ParseCharacterBaseInfo(name, level, exp, hp, damage, armor, luck);
        yield return null;
    }
}
