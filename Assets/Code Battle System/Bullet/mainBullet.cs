using System;
using Code_Battle_System.BatlleSystem;
using UnityEngine;



namespace Code_Battle_System.Bullet
{
    public enum Effect
    {
        Power,
        Frezz,
        Normal,
        Double
    }
    
    public class MainBullet : MonoBehaviour
    {
        public float gravity = 9.8f;
        private Rigidbody2D rb;
        private float angle;
        private float velocity;
        private float vecx;
        private float vecy;
        private float time = 0f;
        public int damage = 10;
        private float maxTimeFight = 5f;
        public Effect status = Effect.Normal;
        
        private CalculateAngle _calculateAngle;
        private CharacterControllerScript _testMoment;


        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            GameObject calAngle = GameObject.Find("Character Controller");
            if (calAngle != null)
            {
                _calculateAngle = calAngle.GetComponent<CalculateAngle>();
            }
            else
            {
                Debug.LogError("Data of Angle not found.");
            }
            angle = _calculateAngle.angle;
            GameObject enemyForce = GameObject.Find("Character Controller");
            if (enemyForce != null)
            {
                _testMoment = enemyForce.GetComponent<CharacterControllerScript>();
            }
            else
            {
                Debug.LogError("Data not found.");
            }
            velocity = _testMoment.froce * 12;
            float radianAngle = angle * Mathf.Deg2Rad;
            float initiaVelocityX = velocity * Mathf.Cos(radianAngle);
            float initiaVelocityY = velocity * Mathf.Sin(radianAngle);
            
            Vector2 initialvelocity = new Vector2(initiaVelocityX, initiaVelocityY);
            rb.velocity = initialvelocity;
            rb.gravityScale = gravity * 10;
            Vector3 posision = transform.position;
            vecx = posision.x;
            vecy = posision.y;
        }

        private void Update()
        {
            time += Time.deltaTime;
            float x = vecx + velocity * Mathf.Cos(angle * Mathf.Deg2Rad) * time;
            float y = vecy + velocity * Mathf.Sin(angle * Mathf.Deg2Rad) * time - 0.5f * gravity * time * time;
            transform.position = new Vector3(x, y, transform.position.z);
        }
    }
    
}