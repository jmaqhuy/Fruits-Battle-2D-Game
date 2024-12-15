using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPrefabs : MonoBehaviour
{
   public GameObject characterPrefab;
   public Image characterImage;
   public TextMeshProUGUI characterName;
   public TextMeshProUGUI characterLevel;
   public GameObject isSelected;

   public void ChangeColor()
   {
      characterPrefab.GetComponent<Image>().color = Color.yellow;
   }
}
