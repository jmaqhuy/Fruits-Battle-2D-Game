using System;
using Code_Battle_System.BatlleSystem;
using NetworkThread;
using UnityEngine;
using UnityEngine.UI;

namespace Code_Battle_System.Character
{
    
    public class BananaGO : MonoBehaviour
    {
        //For Force bar can work.
        public float speedFill = 1f;
        private bool filling = false;
        private bool fillingReverse = true;
        private Image fill;
        public float force;
        private float angle;
        private bool isShoot;
        private Transform pointObject;
        
        private GameObject bulletPerfab;
        
        private Animator animator;
        private bool animationCompleted = false;
        private CalculateAngle _calculateAngle;
        private void Start()
        {
            GameObject fillImage = GameObject.Find("ForceBarFight");
            fill = fillImage.GetComponent<Image>();
            _calculateAngle = GetComponent<CalculateAngle>();
            animator = GetComponent<Animator>();
            isShoot = false;
            GameObject point = GameObject.Find("target");
            if (point != null)
            {
                pointObject = point.transform;

            }
            else
            {
                Debug.Log("No Point Object Selected");
            }
            
            bulletPerfab = GameObject.Find("bullet");
            if (bulletPerfab != null)
            {
                Debug.Log("Bullet Perfab Selected");
            }
            else
            {
                Debug.Log("No Bullet Object Selected");
            }
            
        }

        private void Update()
        {
            if (!isShoot) 
            {
                if (Input.GetButtonDown("Jump"))
                {
                    filling = true;
                }
                if (Input.GetButtonUp("Jump"))
                {
                    animator.SetBool("shooting", true);
                    filling = false;
                    force = fill.fillAmount;
                    angle = _calculateAngle.angle;

                }
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("banana_shoot") && animator.GetBool("shooting") && !animationCompleted)
                {
                    animationCompleted = true;
                    Shoot();
                    animator.SetBool("shooting", false);
                    animationCompleted = false;
                }
                Strong();
            }
        }
        //How the force bar work
        void Strong()
        {
            if ((filling == true) && (fillingReverse == true))
            {
                if (fill.fillAmount < 1.0F)
                {
                    // slider.value += speedFill * Time.deltaTime;
                    fill.fillAmount += speedFill * Time.deltaTime;
                }
                else
                {
                    // slider.value = maxValue;
                    fill.fillAmount = 1.0F;
                    fillingReverse = false;
                }
            }
            else if ((filling == true) && (fillingReverse == false))
            {
                if (fill.fillAmount >0)
                {
                    // slider.value -= speedFill * Time.deltaTime;
                    fill.fillAmount -= speedFill * Time.deltaTime;
                }
            
                else
                {
                    // slider.value = 0;
                    fill.fillAmount = 0F;
                    fillingReverse = true;
                }
            }

            if (filling == false)
            {
                fillingReverse = true;
            }

            // if (fixTimeTurn == 0)
            // {
            //     if (Input.GetKey(KeyCode.Space))
            //     {
            //         filling = false;
            //         froce = slider.value;
            //         Shoot();
            //         controll = false;
            //     }
            //     else
            //     {
            //         filling = false;
            //         froce = 0;
            //         Shoot();
            //         controll = false;
            //     }
            // }
        }
      
        //Create Bullet
        void Shoot()
        {
            /*
             * angle, froce, pointObject.position
             */

            // GameObject bullet = Instantiate(bulletPerfab, pointObject.position, pointObject.rotation);
            //animator.SetBool("shooting", false);
            //animationCompleted = false;
            NetworkStaticManager.ClientHandle.SendShootPacket(NetworkStaticManager.ClientHandle.GetUsername(), angle, force, pointObject.position.x, pointObject.position.y);
            isShoot = true;

        }
    }
}