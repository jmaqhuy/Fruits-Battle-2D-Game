using System.Collections.Generic;
using Cinemachine;
using Code_Battle_System.BatlleSystem;
using Code_Battle_System.Bullet;
using NetworkThread;
using UnityEngine;

public class GameBattle : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    public CinemachineVirtualCamera virtualCamera;
    
    public Dictionary<string, GameObject> Players;
    
    private void Awake()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
    }

    void Start()
    {
        Debug.Log("OK");
        Players = new Dictionary<string, GameObject>();
        
    }
    
}
