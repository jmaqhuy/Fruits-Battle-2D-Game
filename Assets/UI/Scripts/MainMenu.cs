using System.Collections;
using System.Collections.Generic;
using NetworkThread;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Basic User Information")]
    public TMP_Text displayName;
    public TMP_Text coins;

    public TMP_Text displayNameProfile;
    // Start is called before the first frame update
    void Start()
    {
        NetworkStaticManager.ClientHandle.RequestBasicUserInfo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDisplayNameTMP(string displayName)
    {
        this.displayName.text = displayName;
    }

    public void SetCoinsTMP(int coin)
    {
        this.coins.text = coin.ToString();
    }

    public void GoToShop()
    {
        SceneManager.LoadScene("Shop");
    }
    
    public void SelectPlayMode()
    {
        SceneManager.LoadScene("Select Play Mode");
    }

    public void ShowMyProfile()
    {
        displayNameProfile.text = displayName.text;
    }
}
