using System.Collections;
using System.Collections.Generic;
using Code_Battle_System.BatlleSystem;
using UnityEngine;
using UnityEngine.UI;

public class CharacterControllerScript : MonoBehaviour
{
    public Image powerImage;
    public GameObject powerEffect;
    public Button powerButton;

    //For Player can moving
    public float speedMovement = 5f;
    public Rigidbody2D rb;
    private Vector2 _movement;
    public Animator animator;

    //For target point can move around Player.
    public Transform pointObject;
    public Transform centerObject;
    public float radius = 2f;
    private float radius1;
    private float _radius2;
    public float speed = 1f;
    private float _angle;
    private Vector3 _previousPoint;

    //For Force bar can work.
    public float speedFill = 1f;
    private bool filling = false;
    private bool fillingReverse = true;
    public Image fill;
    public float froce;

    //Connect to other scripts.
    public GameObject bulletPerfab;
    public CalculateAngle cal;
    public GameObject Player;

    //Setup Player before battle start.
    void Start()
    {
        resetPowerFillAmount();
        radius1 = radius;
        _radius2 = -radius;
    }

    void Update()
    {
        if (powerImage.fillAmount >= 1.0F)
        {
            powerEffect.SetActive(true);
            powerButton.onClick.AddListener(OnPowerButtonClicked);
        }
        movement();
        if (Input.GetButtonDown("Jump"))
        {
            filling = true;
        }
        if (Input.GetButtonUp("Jump"))
        {
            animator.SetBool("shooting", true);
            filling = false;
            froce = fill.fillAmount;
            Shoot();
            animator.SetBool("shooting", true);
        }
        Strong();
    }

    //Player can moving
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + _movement * speedMovement * Time.fixedDeltaTime);
    }

    void OnPowerButtonClicked()
    {
        resetPowerFillAmount();
        powerEffect.SetActive(false);
    }

    void resetPowerFillAmount()
    {
        powerImage.fillAmount = 0;
    }

    //Create Bullet
    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPerfab, pointObject.position, pointObject.rotation);
    }
    
    //For target point movement
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
    
    
    void movement()
    {
        // timeTurn -= Time.deltaTime; 
        // fixTimeTurn = Mathf.CeilToInt(timeTurn); 
        // timeText.text = fixTimeTurn.ToString(); 
        pointObject.gameObject.SetActive(true); 
        // cal.controll = true; 
        float verticalInput = Input.GetAxisRaw("Vertical"); 
        _angle += verticalInput * speed * Time.deltaTime; 
        _angle = Mathf.Clamp(_angle, 0f, 3 * Mathf.PI / 2); 
        _movement.x = Input.GetAxisRaw("Horizontal"); 
        if (_movement.x < 0) 
        { 
            animator.SetBool("moving", true);
            Player.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (_movement.x > 0) 
        { 
            animator.SetBool("moving", true);
            Player.transform.rotation = Quaternion.Euler(0f, 0f, 0f); 
        }
        else
        {
            animator.SetBool("moving", false);
        }
        if (Mathf.Approximately(Player.transform.rotation.eulerAngles.y, 0)) 
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
        else if (Mathf.Approximately(Player.transform.rotation.eulerAngles.y, 180)) 
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
        // fill.color = gtadient.Evaluate(slider.normalizedValue);
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

}


