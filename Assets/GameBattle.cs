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

public class GameBattle : MonoBehaviour
{
    [Header("Battle Information")]
    public TextMeshProUGUI TimeText;

    [Header("End Game Panel")]
    public GameObject EndGamePanel;
    public TextMeshProUGUI EndText;

    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    public Quaternion quaternion;
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
        if(packet.TeamWin == _myTeam)
        {
            EndText.text = "Victory";
            

        }
        else
        {
            EndText.text = "Defeat";
        }
        MonoBehaviour[] components = Players[NetworkStaticManager.ClientHandle.GetUsername()].GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in components)
        {
            Destroy(component);
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
            playerName.SetActive(!playerName.activeSelf);
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
        focusCamera.Follow = Players[packet.playerName].transform;
        clockCoroutine = StartCoroutine(Clock(20));
        _currentPlayerName = Players[packet.playerName].GetComponent<Unit>().nameText.gameObject;
        blinkNameCoroutine = StartCoroutine(BlinkName(_currentPlayerName));

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
        if (script != null)
        {
            script.setHealthCurrent(packet.HP);
        }
    }
    public void PlayerDie(PlayerDiePacket packet)
    {
        Unit script = Players[packet.player].GetComponent<Unit>();
        if (script != null)
        {
            script.Destroy();
            Debug.Log("Destroy player");
        }
    }

}
