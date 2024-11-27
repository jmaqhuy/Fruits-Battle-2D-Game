using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class FriendOnlineStatus : MonoBehaviour
{
    public TextMeshProUGUI friendName;
    private static FriendOnlineStatus _instance;
    
    private void Awake()
    {
        if (_instance != null)
        {   
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
