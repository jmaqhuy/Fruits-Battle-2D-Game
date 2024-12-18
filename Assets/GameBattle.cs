using System.Collections;
using System.Collections.Generic;

using Cinemachine;
using Code_Battle_System.BatlleSystem;
using Code_Battle_System.Bullet;
using Code_Battle_System.Character;
using Lidgren.Network;
using NetworkThread;
using NetworkThread.Multiplayer;
using UI.Scripts;

using UnityEngine;
using static NetworkThread.Multiplayer.PacketTypes;

public class GameBattle : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    public Quaternion quaternion;
    public CinemachineVirtualCamera focusCamera;
    public GameObject characterController;
    // Ensure this is assigned in the Inspector
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
    private void Update()
    {

    }
    public void EndGame(EndGamePacket packet)
    {
        Unit script = Players[NetworkStaticManager.ClientHandle.GetUsername()].GetComponent<Unit>();
        if (packet.TeamWin == "Team1" && script.getTeam() == Team.Team1)
        {
            Debug.Log("You Win");
        }
        else if(packet.TeamWin == "Team2" && script.getTeam() == Team.Team2)
        {
            Debug.Log("You Win");
        }
        else
        {
            Debug.Log("You Lose");
        }
    }
    public void SpawnPlayer(SpawnPlayerPacket packet)
    {
        Debug.Log("Spawn player: " + packet.playerSpawn + " " + packet.X + " " + packet.Y);

        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is not assigned in the Inspector.");
            return;
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
        script.setUnitName(packet.playerSpawn);
        Physics.SyncTransforms();
        player.SetActive(true);
        Players[packet.playerSpawn] = player;

    }
    public void GetTurn(StartTurnPacket packet)
    {
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
