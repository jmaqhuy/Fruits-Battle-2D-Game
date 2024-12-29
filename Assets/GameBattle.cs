using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Code_Battle_System.BatlleSystem;
using Code_Battle_System.Bullet;
using Code_Battle_System.Character;
using DataTransfer;
using NetworkThread;
using NetworkThread.Multiplayer;
using NetworkThread.Multiplayer.Packets;
using RoomEnum;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameBattle : MonoBehaviour
{
    [Header("Battle Information")]
    public TextMeshProUGUI TimeText;

    [Header("End Game Panel")]
    public GameObject EndGamePanel;
    public TextMeshProUGUI EndText;
    
    [Header("Game Prefab")]
    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    public GameObject updateHpTextPrefab;

    public GameObject Item1;
    public GameObject Item2;
    public CinemachineVirtualCamera focusCamera;
    public GameObject characterController;
    // Ensure this is assigned in the Inspector
    public Dictionary<string, GameObject> Players;
    public PositionsData positionsData;
    public RoomData roomData;
    private Team _myTeam;
    private Coroutine clockCoroutine;
    private Coroutine blinkNameCoroutine;
    private GameObject _currentPlayerName;

    private Transform _oldCameraPosition;
    private Transform _targetCameraPosition;
    private Transform _currentCameraPosition;
    private float cameraTransitionTime = 1f;
    private Vector3 velocity = Vector3.zero; 
    private bool Item1IsUsed = false;
    private bool Item2IsUsed = false;
    private bool IsUsedItem = false;
    
    private void Awake()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        _myTeam = roomData.PlayersInRoom
            .FirstOrDefault(u => u.username == NetworkStaticManager.ClientHandle.GetUsername())
            .team;
        Players = new Dictionary<string, GameObject>();
        foreach (var packet in positionsData.spawnPlayerPackets)
        {
            StartCoroutine(SpawnPlayer(packet));
        }
        Debug.Log(positionsData.spawnPlayerPackets.Count + " players spawned");
    }

    void Start()
    {
        Debug.Log("OK");
    }

    public void EndGame(EndGamePacket packet)
    {
        Debug.Log("my team is "+_myTeam) ;
        if(packet.TeamWin == _myTeam)
        {
            EndText.text = "Victory";
            
            

        }
        else
        {
            EndText.text = "Defeat";
        }

        if (Players[NetworkStaticManager.ClientHandle.GetUsername()] != null)
        {
            MonoBehaviour[] components = Players[NetworkStaticManager.ClientHandle.GetUsername()].GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                Destroy(component);
            }
        }
       
        EndGamePanel.SetActive(true);
    }
    public void BackToRoom()
    {
        SceneManager.LoadScene("Waiting Room");
    }

    private IEnumerator Clock(int start)
    {
        while (start-- > 0)
        { 
            TimeText.text = start.ToString();
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator BlinkName(GameObject playerName)
    {
        while (true)
        {
            if (playerName != null)
            {
                playerName.SetActive(!playerName.activeSelf);
            }
            
            yield return new WaitForSeconds(1);
        }
    }
    
    void StopAllCoroutinesManually(GameObject playerName = null)
    {
        if (clockCoroutine != null)
        {
            StopCoroutine(clockCoroutine);
            clockCoroutine = null;
        }
        if (blinkNameCoroutine != null)
        {
            StopCoroutine(blinkNameCoroutine);
            blinkNameCoroutine = null;
        }
        if (playerName != null)
        {
            playerName.SetActive(true);
        }
    }

    private IEnumerator SpawnPlayer(SpawnPlayerPacket packet)
    {
        Debug.Log("Spawn player: " + packet.playerSpawn + " " + packet.X + " " + packet.Y);

        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is not assigned in the Inspector.");
            yield return null;
        }

        Vector3 position = new Vector3(packet.X, packet.Y, 0); // Adjust Z-axis if needed
        Quaternion rotation = Quaternion.Euler(0, -180, 0); // Default rotation (0, 0, 0)

        GameObject player = Instantiate(playerPrefab, position, rotation);
        player.transform.SetParent(null);
        player.name = packet.playerSpawn;
        Unit script = player.GetComponent<Unit>();
        // If the player prefab has a Rigidbody, we want to update its rotation
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Use MoveRotation to set the initial rotation
            rb.MoveRotation(rotation);
        }
        script.setHealthMax(packet.HP);
        script.setHealthCurrent(packet.HP);
        script.setTeam(packet.Team);
        script.setAttack(packet.Attack);
        script.setLucky(packet.Lucky);
        script.setArmor(packet.Amor);
        script.setIsLest(true);
        Color color = _myTeam == packet.Team ? Color.green : Color.red;
        if (packet.playerSpawn == NetworkStaticManager.ClientHandle.GetUsername()) color = Color.yellow;
        script.setUnitName(packet.playerSpawn , color);
        Physics.SyncTransforms();
        player.SetActive(true);
        Players[packet.playerSpawn] = player;

    }
    public void GetTurn(StartTurnPacket packet)
    {
        
        //
        StopAllCoroutinesManually(_currentPlayerName);
        if (Players.ContainsKey(NetworkStaticManager.ClientHandle.GetUsername()))
        {
            if (NetworkStaticManager.ClientHandle.GetUsername() == packet.playerName)
            {
                characterController.SetActive(true);
                if (Players[packet.playerName].GetComponent<Controller>() == null) { Players[packet.playerName].AddComponent<Controller>(); }
                if (Players[packet.playerName].GetComponent<CalculateAngle>() == null) { Players[packet.playerName].AddComponent<CalculateAngle>(); }
                if (Players[packet.playerName].GetComponent<BananaGO>() == null) { Players[packet.playerName].AddComponent<BananaGO>(); }

            }
            else
            {
                characterController.SetActive(false);
            }

        }

        IsUsedItem = false;
        clockCoroutine = StartCoroutine(Clock(20));
        _currentPlayerName = Players[packet.playerName].GetComponent<Unit>().nameText.gameObject;
        blinkNameCoroutine = StartCoroutine(BlinkName(_currentPlayerName));
       
        focusCamera.Follow = Players[packet.playerName].transform;


    }
    


    
    public void EndTurn(EndTurnPacket packet)
    {
        if (Players.ContainsKey(NetworkStaticManager.ClientHandle.GetUsername()))
        {
            if (NetworkStaticManager.ClientHandle.GetUsername() == packet.playerName)
            {
                // This will remove all MonoBehaviour components (scripts) from the GameObject
                MonoBehaviour[] components = Players[packet.playerName].GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour component in components)
                {
                    if (!(component is Unit))
                    {
                        Destroy(component);
                    }
                }
            }

        }
    }
    public void Shoot(Shoot packet)
    {
        Vector3 position = new Vector3(packet.X, packet.Y, 0);
        if (bulletPrefab == null)
        {
            Debug.Log("bullet is not set");
            return;
        }
        GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        
        bullet.SetActive(true);
        focusCamera.Follow = bullet.transform;
        Unit script = Players[packet.playerName].GetComponent<Unit>();
        if (script == null)
        {
            Debug.Log("player unit is not found");
        }
        MainBullet mainBullet = bullet.GetComponent<MainBullet>();
        if (mainBullet != null)
        {
            mainBullet.SetAngle(packet.angle);
            mainBullet.SetForce(packet.force);
            mainBullet.SetShooter(packet.playerName);
            mainBullet.setTeam(script.getTeam());
            mainBullet.setDamage(script.getAttack());

        }
        if (clockCoroutine != null)
        {
            StopCoroutine(clockCoroutine);
            clockCoroutine = null;
        }
    }
    public void UpdatePosition(PositionPacket packet)
    {
        Animator animator = Players[packet.playerName].GetComponent<Animator>();
        Transform targetTransform = Players[packet.playerName].transform.Find("target");
        Unit script = Players[packet.playerName].GetComponent<Unit>();
        animator.SetBool("moving", true);

        if (packet.X - Players[packet.playerName].transform.position.x < 0 && script.getIsLest()) { }
        else if (packet.X - Players[packet.playerName].transform.position.x > 0 && !script.getIsLest()) { }
        else if (packet.X - Players[packet.playerName].transform.position.x > 0 && script.getIsLest())
        {
            Players[packet.playerName].transform.rotation = Quaternion.Euler(0, 0, 0);
            script.setIsLest(false);
        }
        else
        {
            Players[packet.playerName].transform.rotation = Quaternion.Euler(0, 180, 0);
            script.setIsLest(true);
        }

        Players[packet.playerName].transform.position = new Vector3(packet.X, packet.Y, Players[packet.playerName].transform.position.z);
        animator.SetBool("moving", false);
    }
    public void UpdateHP(HealthPointPacket packet)
    {
        Unit script = Players[packet.PlayerName].GetComponent<Unit>();
        createUpdateHpText(packet.HP-script.getHealthCurrent(),Players[packet.PlayerName]);
        if (script != null)
        {
            script.setHealthCurrent(packet.HP);
        }
       
    }
    public void createUpdateHpText(int takeDamage,GameObject player)
    {
        string text = takeDamage.ToString();
        if (takeDamage > 0)
        {
            text =  "+" + takeDamage.ToString();
        }
        Vector3 textPosition = new Vector3(player.transform.position.x, player.transform.position.y+3, player.transform.position.z);
        if (updateHpTextPrefab != null)
        {
            Debug.Log("updateHpText spawn");
            GameObject newText = (GameObject)Instantiate(updateHpTextPrefab, textPosition, Quaternion.identity);
            newText.transform.GetComponent<TextMeshPro>().text = text;
            if(takeDamage > 0){newText.transform.GetComponent<TextMeshPro>().color = Color.green;}
            
            newText.gameObject.SetActive(true);
            // newText.transform.SetParent(canvas.transform, false);
        }
       
    }
    public void PlayerDie(PlayerDiePacket packet)
    {
        if (Players[packet.player] != null)
        {
            Unit script = Players[packet.player].GetComponent<Unit>();
            if (script != null)
            {
                script.Destroy();
                Debug.Log("Destroy player");
            }
        }
    }

    public void UseItem1()
    
    {
        if (!Item1IsUsed && !IsUsedItem)
        {
            if (Players[NetworkStaticManager.ClientHandle.GetUsername()] != null)
            {
                Unit script = Players[NetworkStaticManager.ClientHandle.GetUsername()].GetComponent<Unit>();
                if (script != null)
                {
                    int NewHp = script.getHealthCurrent()+350;
                    if(NewHp >= script.getHealthMax())
                    {
                        NewHp = script.getHealthMax();
                    }
                    NetworkStaticManager.ClientHandle.SendHPPacket(NetworkStaticManager.ClientHandle.GetUsername(),NewHp);
                    Item1IsUsed = true;
                    IsUsedItem = true;
                    if (Item1 != null)
                    {
                        Image image = Item1.GetComponent<Image>();
                        image.color = new Color(125f / 255f, 125f / 255f, 125f / 255f, 1f);
                    }   
                }
            }
        }
    }
    public void UseItem2()
    
    {
        if (!Item2IsUsed && !IsUsedItem)
        {
            if (Players[NetworkStaticManager.ClientHandle.GetUsername()] != null)
            {
                Unit script = Players[NetworkStaticManager.ClientHandle.GetUsername()].GetComponent<Unit>();
                if (script != null)
                {
                    int NewHp = script.getHealthCurrent()+350;
                    if(NewHp >= script.getHealthMax())
                    {
                        NewHp = script.getHealthMax();
                    }
                    NetworkStaticManager.ClientHandle.SendHPPacket(NetworkStaticManager.ClientHandle.GetUsername(),NewHp);
                    Item2IsUsed = true;
                    IsUsedItem = true;
                    if (Item2 != null)
                    {
                        Image image = Item2.GetComponent<Image>();
                        image.color = new Color(125f / 255f, 125f / 255f, 125f / 255f, 1f);
                    }   
                }
            }
        }
    }

}
