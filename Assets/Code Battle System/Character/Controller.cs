using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Code_Battle_System.BatlleSystem;
using NetworkThread;
using UnityEngine;
using UnityEngine.UI;
using Input = UnityEngine.Input;

namespace Code_Battle_System.Character
{
    public class Controller : MonoBehaviour
    {
        //Controller Character moving
        private Rigidbody2D _Rigidbody2D;
        public bool controllermoving = true;

        //For Power Image
        private Image powerImage;
        private GameObject powerEffect;
        private Button powerButton;

        //For Player can moving
        public float speedMovement = 5f;
        private Vector2 _movement;
        private Animator animator;
        private bool animationCompleted = false;

        //For target point can move around Player.
        private Transform pointObject;
        private Transform centerObject;
        public float radius = 2f;
        private float radius1;
        private float _radius2;
        public float speed = 1f;
        private float _angle;
        private Vector3 _previousPoint;




        // private CalculateAngle cal;

        //For Warking
        private Image manaFill;
        public float speedOfMana = 1f;
        private bool controllerWarking = true;
        private bool playerAlive = true;
        private Vector3 previousPosition;
        // Start is called before the first frame update
        void Start()
        {
            _Rigidbody2D = GetComponent<Rigidbody2D>();
            _Rigidbody2D.gravityScale = 9.8f;
            _Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            previousPosition = transform.position;
            //Set powerImage = power.GetComponent<Image>();
            GameObject imageObjectPower = GameObject.Find("PowImage");
            if (imageObjectPower != null)
            {
                powerImage = imageObjectPower.GetComponent<Image>();
                if (powerImage == null)
                {
                    Debug.LogError("Image component not found on PowImage");
                }
                Debug.Log("Đã lấy data ");
            }
            else
            {
                Debug.Log("No Image Object Selected");
            }


            // powerImage = imageObjectPower.GetComponent<Image>();
            powerEffect = GameObject.Find("Pow Effect");
            if (powerEffect != null)
            {
                Debug.Log(powerEffect.name + " is selected");
            }
            else
            {
                Debug.Log("No Pow Effect Selected");
            }

            GameObject Button = GameObject.Find("Pow");
            powerButton = Button.GetComponent<Button>();
            resetPowerFillAmount();

            animator = GetComponent<Animator>();

            GameObject point = GameObject.Find("target");
            if (point != null)
            {
                pointObject = point.transform;
            }
            else
            {
                Debug.Log("No Point Object Selected");
            }

            Debug.Log(pointObject.name);
            centerObject = gameObject.transform;



            // cal = GetComponent<CalculateAngle>();

            GameObject manaImage = GameObject.Find("ForceBarManaWalking");
            manaFill = manaImage.GetComponent<Image>();
            manaFill.fillAmount = 1;
            radius1 = radius;
            _radius2 = -radius;
        }


        // Update is called once per frame
        void Update()
        {
            if (powerImage.fillAmount >= 1.0F)
            {
                powerEffect.SetActive(true);
                powerButton.onClick.AddListener(OnPowerButtonClicked);
            }
            //When character in the land, it can move and do other thing.
            if (controllermoving)
            {
                speedMovement = 5f;
                ManaWarking();
                if (controllerWarking)
                {
                    speedMovement = 5f;
                }
                else
                {
                    speedMovement = 0f;
                }
                movement();

            }
            //make speedMovement =0 for character can fall down.
            else if (controllermoving == false)
            {
                speedMovement = 0;
            }

            if (gameObject.transform.position.y < 230 && playerAlive)
            {
                playerUnderMap();
                playerAlive = false;
            }
        }

        private void playerUnderMap()
        {
            Unit script = gameObject.GetComponent<Unit>();
            if (script != null)
            {
                if (script.getUnitName() == NetworkStaticManager.ClientHandle.GetUsername())
                {
                    
                    NetworkStaticManager.ClientHandle.SendPlayerDie(NetworkStaticManager.ClientHandle.GetUsername());
                    NetworkStaticManager.ClientHandle.SendEndTurn(NetworkStaticManager.ClientHandle.GetUsername());
                    Destroy(gameObject);
                }
            }
            
        }
        //Player can moving
        private void FixedUpdate()
        {
            _Rigidbody2D.MovePosition(_Rigidbody2D.position + _movement * speedMovement * Time.fixedDeltaTime);

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
            float verticalInput = Input.GetAxisRaw("Vertical");
            _angle += verticalInput * speed * Time.deltaTime;
            _angle = Mathf.Clamp(_angle, 0f, 3 * Mathf.PI / 2);
            _movement.x = Input.GetAxisRaw("Horizontal");
            if (_movement.x < 0)
            {
                animator.SetBool("moving", true);
                gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                manaFill.fillAmount -= speedOfMana * Time.deltaTime;

            }
            else if (_movement.x > 0)
            {
                animator.SetBool("moving", true);
                gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                manaFill.fillAmount -= speedOfMana * Time.deltaTime;

            }
            else
            {
                animator.SetBool("moving", false);
            }
            if (Mathf.Approximately(gameObject.transform.rotation.eulerAngles.y, 0))
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
            else if (Mathf.Approximately(gameObject.transform.rotation.eulerAngles.y, 180))
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
            if (transform.position != previousPosition)
            {
                // Send position update packet if the position has changed
                NetworkStaticManager.ClientHandle.SendPositionPacket(NetworkStaticManager.ClientHandle.GetUsername(), transform.position.x, transform.position.y);

                // Update the previous position to the current one
                previousPosition = transform.position;
                
            }

        }



        //Controll mana for warking.
        void ManaWarking()
        {
            if (manaFill.fillAmount > 0)
            {
                controllerWarking = true;
            }
            else if (manaFill.fillAmount == 0)
            {
                manaFill.fillAmount = 0;
                controllerWarking = false;
            }

        }


        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("dat"))
            {
                controllermoving = true;
                _Rigidbody2D.gravityScale = 9.8f;
            }
        }
        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("dat"))
            {
                controllermoving = false;
                _Rigidbody2D.gravityScale = 49f;
            }
        }
    }
}
