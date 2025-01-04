using System;
using TMPro;
using UnityEngine;

namespace Code_Battle_System.BatlleSystem
{
    public class CalculateAngle : MonoBehaviour
    {
        public Transform target;
        public GameObject Player;
        
        public TextMeshProUGUI angleText;
        public float angle;
        private float angle2;
        private Vector2 direction;

        private void Start()
        {
            target = transform.Find("target");
            GameObject textObject = GameObject.Find("AngleText");
            angleText = textObject.GetComponent<TextMeshProUGUI>();
        }
        private void Update()
        {
            direction = target.position - gameObject.transform.position;
            angle = Vector2.Angle(Vector2.right, direction);
            if (angle > 90)
            {
                angle2 = 180 - angle;
            }
            else
            {
                angle2 = angle;
            }
            angleText.text = angle2.ToString("F2");
        }
    }
}