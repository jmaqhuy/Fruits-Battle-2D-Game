using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTransfer;
using NetworkThread;
using NetworkThread.Multiplayer;
using NetworkThread.Multiplayer.Packets;
using Resources;
using RoomEnum;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RankScene : MonoBehaviour
{
    public UserData userData;
    public RoomData roomData;
    public PositionsData positionsData;
    
    public Transform playersHolder;
    private Dictionary<RoomType, Vector3> uiAnimation = new Dictionary<RoomType, Vector3>();
    
    public TextMeshProUGUI RoomId;
    public TextMeshProUGUI RoomName;

    private RoomType _roomType;
    public TMP_Dropdown gameTypeDropdown;

    public Button ReadyButton;
    public TextMeshProUGUI ReadyText;
    public GameObject[] players;
    
    [Header("Self Rank Panel")]
    public TextMeshProUGUI rankName;
    public SpriteRenderer rankIcon;
    public GameObject stars;
    public GameObject starIcon;
    private List<GameObject> starList = new List<GameObject>();
    
    [Header("Error Panel")]
    public GameObject errorPanel;
    public TextMeshProUGUI errorText;
    
    [Header("Time Counter")]
    public GameObject timeCounterPanel;
    public TextMeshProUGUI timeCounterText;
    private Coroutine timeCounterCoroutine;
    
    [Header("Match Found Panel")]
    public GameObject matchFoundPanel;
    public TextMeshProUGUI matchFoundText;
    
    
    
    private int _currentDropdownValue;
    // Start is called before the first frame update
    void Start()
    {
        NetworkStaticManager.ClientHandle.SetUiScripts(this);
        
        uiAnimation.Add(RoomType.FourVsFour, new Vector3(1, 1, 1));
        uiAnimation.Add(RoomType.OneVsOne, new Vector3(1.45f, 1.45f, 1.45f));
        uiAnimation.Add(RoomType.TwoVsTwo, new Vector3(1.3f, 1.3f, 1.3f));
        gameTypeDropdown.onValueChanged.AddListener(OnGameTypeDropdownValueChanged);
        ParseSelfRank();
        ParseRoomInfo();
        _currentDropdownValue = gameTypeDropdown.value;
        ReadyButton.onClick.AddListener(OnReadyButtonClicked);
        timeCounterPanel.GetComponent<Button>().onClick.AddListener(OnStopFindMatchButtonClicked);
    }

    private void OnStopFindMatchButtonClicked()
    {
        NetworkStaticManager.ClientHandle.SendMatchmakingPacket(roomData.RoomPacket.Id, false);
    }

    private void OnReadyButtonClicked()
    {
        PlayerInWaitingRoom selfScript = players[0].transform.Find("Player").GetComponent<PlayerInWaitingRoom>();
        if (selfScript.isHost.activeSelf)
        {
            NetworkStaticManager.ClientHandle.SendMatchmakingPacket(roomData.RoomPacket.Id, true);
            return;
        }
        NetworkStaticManager.ClientHandle.SendPlayerReadyPacket(roomData.RoomPacket.Id, !selfScript.isReady.activeSelf);
    }

    public void ReceivedStartMatchmakingPacket()
    {
        timeCounterCoroutine = StartCoroutine(Clock());
    }

    public void ReceivedStopMatchmakingPacket(bool matchFound)
    {
        PlayerInWaitingRoom selfScript = players[0].transform.Find("Player").GetComponent<PlayerInWaitingRoom>();
        gameTypeDropdown.interactable = selfScript.isHost.activeSelf;
        StopCoroutine(timeCounterCoroutine);
        timeCounterPanel.SetActive(false);
        if (matchFound)
        {
            StartCoroutine(CountDownMatchFound());
        }
    }

    private IEnumerator CountDownMatchFound()
    {
        matchFoundPanel.SetActive(true);
        var timeCounter = 6;
        while (timeCounter-- > 0)
        {
            matchFoundText.text = timeCounter.ToString();
            yield return new WaitForSeconds(1);
        }
        NetworkStaticManager.ClientHandle.SendStartGamePacket(roomData.RoomPacket.Id);
    }
    public void ReceiveStartGame(SpawnPlayerPacketToAll packet)
    {
        positionsData.spawnPlayerPackets = packet.SPPacket;
        SceneManager.LoadScene("Normal Mode Map");
    }
    
    private IEnumerator Clock()
    {
        gameTypeDropdown.interactable = false;
        timeCounterPanel.SetActive(true);
        var second = 0;
        var minute = 0;
        while (true)
        {
            if (second == 60)
            {
                minute++;
                second = 0;
            }
            timeCounterText.text = $"{minute:D2}:{second:D2}";
            second++;
            yield return new WaitForSeconds(1);
        }
    }


    private void ParseSelfRank()
    {
        Sprite rIcon = ResourceLoader.LoadRankSprite(userData.CurrentRank.rankAssetName);
        if (rIcon != null)
        {
            rankIcon.sprite = rIcon;
        }
        else
        {
            Debug.LogError($"Failed to load rank icon");
        }
        var starNum = userData.CurrentRank.currentStar;
        Debug.Log($"star num: {starNum}");
        starIcon.SetActive(true);
        if (starNum != 0)
        {
            for (int i = 0; i < starNum; i++)
            {
                starList.Add(Instantiate(starIcon, stars.transform));
            }
        }
        else
        {
            GameObject starGO = Instantiate(starIcon, stars.transform);
            starGO.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
            starList.Add(starGO);
        }
        Debug.Log($"star list size: {starList.Count}");
        rankName.text = userData.CurrentRank.rankName;
        starIcon.SetActive(false);
    }

    private void OnGameTypeDropdownValueChanged(int arg0)
    {
        Debug.Log($"Number of player in room: {roomData.PlayersInRoom.Count}");
        switch (gameTypeDropdown.value)
        {
            case 0:
                if (roomData.PlayersInRoom.Count > 1)
                {
                    StartCoroutine(ShowErrorPanel("There is more than one player in the room"));
                    gameTypeDropdown.value = _currentDropdownValue;
                    return;
                }
                OnOneVsOneButtonClick();
                break;
            case 1:
                if (roomData.PlayersInRoom.Count > 2)
                {
                    StartCoroutine(ShowErrorPanel("There is more than two player in the room"));
                    gameTypeDropdown.value = _currentDropdownValue;
                    return;
                }
                OnTwoVsTwoButtonClick();
                break;
            case 2:
                OnFourVsFourButtonClick();
                break;
        }
        PlayerInWaitingRoom selfScript = players[0].transform.Find("Player").GetComponent<PlayerInWaitingRoom>();
        if (selfScript.isHost.activeSelf)
        {
            NetworkStaticManager.ClientHandle.SendChangeRoomTypePacket(roomData.RoomPacket);
        }
        _currentDropdownValue = gameTypeDropdown.value;
        
    }

    private void OnFourVsFourButtonClick()
    {
        _roomType = RoomType.FourVsFour;
        ApplyUI(_roomType);
        foreach (var pl in players)
        {
            pl.SetActive(true);
        }
    }

    private void OnTwoVsTwoButtonClick()
    {
        players[3].SetActive(false);
        players[2].SetActive(false);
        players[1].SetActive(true);
        _roomType = RoomType.TwoVsTwo;
        ApplyUI(_roomType);
    }

    private void OnOneVsOneButtonClick()
    {
        players[3].SetActive(false);
        players[2].SetActive(false);
        players[1].SetActive(false);
        _roomType = RoomType.OneVsOne;
        ApplyUI(_roomType);
    }

    public void BackToSelectPlayMode()
    {
        PlayerInWaitingRoom selfScript = players[0].transform.Find("Player").GetComponent<PlayerInWaitingRoom>();
        if (selfScript.isReady.activeSelf)
        {
            StartCoroutine(ShowErrorPanel("You can't go out because you are ready!"));
            return;
        }
        NetworkStaticManager.ClientHandle.SendExitRoomPacket(roomData.RoomPacket.Id);
        roomData.RoomPacket = null;
        roomData.PlayersInRoom.Clear();
        SceneManager.LoadScene("Select Play Mode");
    }
    void ApplyUI(RoomType roomType)
    {
        if (uiAnimation.TryGetValue(roomType, out Vector3 targetScale))
        {
            StartCoroutine(AnimateScale(playersHolder, targetScale, 0.2f));
        }
        ParsePlayersInfo();
        roomData.RoomPacket.roomType = roomType;
    }

    public void ParsePlayersInfo()
    {
        foreach (var p in players)
        {
            var plTransform = p.transform.Find("Player");
            plTransform.gameObject.SetActive(false);
        }

        var position = 1;
        foreach (var p in roomData.PlayersInRoom)
        {
            if (p.username == NetworkStaticManager.ClientHandle.GetUsername())
            {
                ParsePlayerInfo(p, 0);
                Debug.Log($"Player 0 has been parsed");
                continue;
            }
            Debug.Log($"Player {position} has been parsed");
            ParsePlayerInfo(p, position++);
            
        }
    }
    private void ParsePlayerInfo(PlayerInRoomPacket playerInRoomPacket, int position)
    {
        PlayerInWaitingRoom selfScript = players[position].transform.Find("Player").GetComponent<PlayerInWaitingRoom>();
        selfScript.username = playerInRoomPacket.username;
        selfScript.PlayerName.text = playerInRoomPacket.displayname;
        selfScript.isHost.SetActive(playerInRoomPacket.isHost);
        selfScript.isReady.SetActive(playerInRoomPacket.isReady);
        players[position].transform.Find("Player/Player Info/Rank").GetComponent<SpriteRenderer>().sprite 
            = ResourceLoader.LoadRankSprite(userData.CurrentRank.rankAssetName);
        
        players[position].transform.Find("Player").gameObject.SetActive(true);
        if (position == 0)
        {
            selfScript.PlayerName.color = Color.yellow;
            if (playerInRoomPacket.isHost)
            {
                gameTypeDropdown.interactable = true;
                ReadyButton.GetComponent<Image>().color = Color.green;
                ReadyText.text = "Start Game";
            }
            else
            {
                gameTypeDropdown.interactable = false;
                ReadyButton.GetComponent<Image>().color = playerInRoomPacket.isReady ? Color.red : Color.green;
                ReadyText.text = playerInRoomPacket.isReady ? "Not Ready" : "Ready";
            }
        }
        
    }

    IEnumerator AnimateScale(Transform target, Vector3 targetScale, float duration)
    {
        Vector3 initialScale = target.localScale;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            target.localScale = Vector3.Lerp(initialScale, targetScale, time / duration);
            yield return null;
        }
        target.localScale = targetScale;
    }

    public void UpdatePlayersInRoom(List<PlayerInRoomPacket> playersInRoom)
    {
        Debug.Log($"Number of players in room: {playersInRoom.Count}");
        roomData.PlayersInRoom = playersInRoom;
        ParsePlayersInfo();
    }

    public void UpdateRoomInfo(RoomPacket roomPacket)
    {
        roomData.RoomPacket = roomPacket;
        ParseRoomInfo();
    }

    private void ParseRoomInfo()
    {
        RoomId.text = "Room ID: " + roomData.RoomPacket.Id;
        RoomName.text = "Room Name: " + roomData.RoomPacket.Name;
        if (roomData.RoomPacket.roomType == RoomType.FourVsFour)
        {
            gameTypeDropdown.value = 2;
            OnFourVsFourButtonClick();
        } else if (roomData.RoomPacket.roomType == RoomType.TwoVsTwo)
        {
            gameTypeDropdown.value = 1;
            OnTwoVsTwoButtonClick();
        } else if (roomData.RoomPacket.roomType == RoomType.OneVsOne)
        {
            gameTypeDropdown.value = 0;
            OnOneVsOneButtonClick();
        }
    }

    IEnumerator ShowErrorPanel(string content)
    {
        var time = 0;
        errorText.text = content;
        errorPanel.SetActive(true);
        while (time++ < 3)
        {
            yield return new WaitForSeconds(1);
        }
        errorPanel.SetActive(false);
    }

    public void ChangeTeam(ChangeTeamPacket packet)
    {
        Debug.Log($"Get Change Team Packet. Packet.Team {packet.team}");
        if (string.IsNullOrEmpty(packet.username))
        {
            Debug.Log("Change Team for all");
            foreach (var pl in roomData.PlayersInRoom)
            {
                pl.team = packet.team;
                Debug.Log($"{pl.displayname} in team {pl.team} now");
            }
        }
    }
}
