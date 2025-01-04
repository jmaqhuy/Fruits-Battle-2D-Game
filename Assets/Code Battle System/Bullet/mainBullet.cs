using System;
using Code_Battle_System.BatlleSystem;
using UI.Scripts;
using UnityEngine;
using Code_Battle_System.Character;
using NetworkThread;
using System.Collections;
using RoomEnum;
using Unity.Collections.LowLevel.Unsafe;
using Random = Unity.Mathematics.Random;


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
        private float force;
        private float velocity;
        private float vecx;
        private float vecy;
        private float time = 0f;
        public int damage;
        private float maxTimeFly = 3f;
        public Effect status = Effect.Normal;
        private string shooter;
        private Team team;
        private bool IsUsePower =false;
        private void Awake()
        {
            angle = 0;
            force = 0;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            velocity = force * 12;
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
            if (time > maxTimeFly)
            {
                Miss();
                if (gameObject != null)
                {
                    Destroy(gameObject);
                }


            }
        }
        public void setIsUsePower(bool isUsePower)
        {
            IsUsePower = isUsePower;
        }
        public void SetAngle(float SetAngle)
        {
            angle = SetAngle;
        }
        public void SetForce(float Force)
        {
            force = Force;
        }
        public void SetShooter(string name)
        {
            this.shooter = name;
        }
        public void setTeam(Team team)
        {
            this.team = team;
        }
        public void setDamage(int damage)
        {
            this.damage = damage;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Unit uniEnemy = other.GetComponent<Unit>();

            // Check if the object is not named "game map 1_1" and that it's a valid Unit
            if (other.name != "game map 1_1" && uniEnemy != null)
            {
                Debug.Log(other.name);
                if (this.team != uniEnemy.getTeam())
                {
                    int currentHP;
                    int takeDame;

                    // Damage calculation based on armor
                    if (this.damage - uniEnemy.getArmor() <= damage*0.1)
                    {
                        takeDame = (int)(this.damage * 0.1);  // Reduced damage if armor is high
                    }
                    else
                    {
                        takeDame = this.damage - uniEnemy.getArmor();  // Full damage minus armor
                    }
                    if (IsUsePower)
                    {
                        takeDame = takeDame * 2;
                    }
                    float lucky = (float)uniEnemy.getLucky()/100;
                    
                    if (UnityEngine.Random.value <= lucky)
                    {
                        takeDame = takeDame * 2;
                    }
                    // Health calculation after damage
                    if (uniEnemy.getHealthCurrent() <= takeDame)
                    {
                        currentHP = 0;
                    }
                    else
                    {
                        currentHP = uniEnemy.getHealthCurrent() - takeDame;
                    }

                    // Send updated health information if the shooter is the client
                    if (NetworkStaticManager.ClientHandle.GetUsername() == this.shooter)
                    {
                        NetworkStaticManager.ClientHandle.SendHPPacket(uniEnemy.getUnitName(), currentHP);
                        NetworkStaticManager.ClientHandle.SendEndTurn(this.shooter);
                    }

                    // Destroy the projectile (bullet)
                    Destroy(gameObject);
                }

            }
            else
            {
                // If the bullet hits an object named "game map 1_1", don't destroy it and call Miss()
                if (other.name == "game map 1_1")
                {
                    Debug.Log("Bullet hit game map 1_1, not destroying.");
                }
                //Miss();  // Call Miss() if the bullet didn't hit a valid target
            }


        }
        void Miss()
        {
            if (NetworkStaticManager.ClientHandle.GetUsername() == this.shooter)
            {

                NetworkStaticManager.ClientHandle.SendEndTurn(this.shooter);
                Debug.Log("Send end Turn");

            }


        }



    }

}