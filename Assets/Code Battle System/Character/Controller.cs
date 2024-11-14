using Code_Battle_System.Batlle_System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code_Battle_System.Character
{
    public class Controller : MonoBehaviour
    {
        public Transform pointObject;
        public Transform centerObject;
        public float radius = 2f;
        private float radius1;
        private float _radius2;
        public float speed = 1f;
        private float _angle;
        private Vector3 _previousPoint;
        
        public float speedMovement = 5f;
        public Rigidbody2D rb;
        private Vector2 _movement;
        
        public Slider slider;
        public float speedFill = 1f;
        public int maxValue = 10;
        private bool filling = false;
        private bool fillingReverse = true;
        public Image fill;
        public Gradient gtadient;
        public float froce;
        private int windforce;
        
        public bool controll = true;
        public CalculateAngle cal;
        
        private float timeTurn = 30f;
        public TextMeshProUGUI timeText;
        private int fixTimeTurn;
        
        public GameObject bulletPerfab;
        public Winds _winds;
        // Start is called before the first frame update
        void Start()
        {
            fill.color = gtadient.Evaluate(1f);
            slider.maxValue = maxValue;
            radius1 = radius;
            _radius2 = -radius;
        }

        // Update is called once per frame
        void Update()
        {
            if (controll == true)
            {
                windforce = _winds.SpeedOfWinds;
                if (Input.GetKey(KeyCode.Space) == false)
                {
                    movement();
                }
                if (Input.GetButtonDown("Jump"))
                {
                    filling = true;
                }
                if (Input.GetButtonUp("Jump"))
                {
                    filling = false;
                    froce = slider.value + windforce;
                    Shoot();
                              
                }
                Strong();
            }
            else
            {
                pointObject.gameObject.SetActive(false);
                timeTurn = 15f;
                _angle = 0;
                _angle = Mathf.Clamp(_angle, 0f, 3*Mathf.PI / 2);
                float x = centerObject.position.x + Mathf.Cos(_angle) * radius;
                float y = centerObject.position.y + Mathf.Sin(_angle) * radius;
                pointObject.position = new Vector3(x, y, 0f);
            }
        }
        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + _movement * speedMovement * Time.fixedDeltaTime);
        }
        void Shoot()
        {
            GameObject bullet = Instantiate(bulletPerfab, pointObject.position, pointObject.rotation);
        }
        void CircleMovement()
        {
            float x = centerObject.position.x + Mathf.Cos(_angle) * radius;
            float y = centerObject.position.y + Mathf.Sin(_angle) * radius;
            pointObject.position = new Vector3(x, y, 0f);
            Vector3 direction = pointObject.position - centerObject.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            pointObject.rotation = Quaternion.Euler(0f, 0f, targetAngle);
            _previousPoint = pointObject.position;
        }
        void CircleMovementFlip()
        {
            float x = centerObject.position.x - Mathf.Cos(_angle) * radius;
            float y = centerObject.position.y + Mathf.Sin(_angle) * radius;
            pointObject.position = new Vector3(x, y, 0f);
            Vector3 direction = pointObject.position - centerObject.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            pointObject.rotation = Quaternion.Euler(0f, 0f, targetAngle);
            _previousPoint = pointObject.position;
        }
        void Strong()
        {
            if ((filling == true) && (fillingReverse == true))
            {
                if (slider.value < maxValue)
                {
                    slider.value += speedFill * Time.deltaTime;
                }
                else
                {
                    slider.value = maxValue;
                    fillingReverse = false;
                }
            }
            else if ((filling == true) && (fillingReverse == false))
            {
                if (slider.value >0)
                {
                    slider.value -= speedFill * Time.deltaTime;
                }
                else
                {
                    slider.value = 0;
                    fillingReverse = true;
                }
            }

            if (filling == false)
            {
                fillingReverse = true;
            }

            if (fixTimeTurn == 0)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    filling = false;
                    froce = slider.value;
                    Shoot();
                    controll = false;
                }
                else
                {
                    filling = false;
                    froce = 0;
                    Shoot();
                    controll = false;
                }
            }
        }
        void movement()
        {
            timeTurn -= Time.deltaTime; 
            fixTimeTurn = Mathf.CeilToInt(timeTurn); 
            timeText.text = fixTimeTurn.ToString(); 
            pointObject.gameObject.SetActive(true); 
            cal.controll = true; 
            float verticalInput = Input.GetAxisRaw("Vertical"); 
            _angle += verticalInput * speed * Time.deltaTime; 
            _angle = Mathf.Clamp(_angle, 0f, 3 * Mathf.PI / 2); 
            _movement.x = Input.GetAxisRaw("Horizontal"); 
            if (_movement.x < 0) 
            { 
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                
            }
            if (_movement.x > 0) 
            { 
                transform.rotation = Quaternion.Euler(0f, 0f, 0f); 
            }
            if (Mathf.Approximately(transform.rotation.eulerAngles.y, 0)) 
            { 
                radius = radius1; 
                if (_angle >= Mathf.PI) 
                { 
                    float constAngle = _angle; 
                    _angle = constAngle - Mathf.PI; 
                } 
                CircleMovement(); 
                if (_angle > Mathf.PI / 2) 
                { 
                    _angle = Mathf.PI / 2; 
                }
            }
            else if (Mathf.Approximately(transform.rotation.eulerAngles.y, 180)) 
            { 
                radius = _radius2; 
                if (_angle <= Mathf.PI / 2) 
                { 
                    _angle = Mathf.PI + _angle; 
                    
                }
                CircleMovementFlip(); 
                if (_angle < Mathf.PI) 
                { 
                    _angle = Mathf.PI; 
                } 
                if (_angle > 3 * Mathf.PI / 2) 
                { 
                    _angle = 3 * Mathf.PI / 2; 
                } 
            } 
            fill.color = gtadient.Evaluate(slider.normalizedValue);
        } 
    }
}
