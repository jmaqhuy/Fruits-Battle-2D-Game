using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class confirmationWindow : MonoBehaviour
{
    public Button yesButton;
    public Button noButton;
    public TextMeshProUGUI message;

    private static confirmationWindow _instance;
    
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
}
