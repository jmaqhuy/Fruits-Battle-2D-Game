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
        CharacterInfoTabScript.ParseCharacterBaseInfo();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
