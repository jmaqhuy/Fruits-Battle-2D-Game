
using System;
using UnityEngine;

namespace Code_Battle_System.Character
{
    
    public class CheckCollider : MonoBehaviour
    {
        public Rigidbody2D _Rigidbody2D;
        public bool controller = false;

        private void Start()
        {
            _Rigidbody2D.gravityScale = 9.8f;
            _Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("dat"))
            {
                controller = true;
                _Rigidbody2D.gravityScale = 9.8f;
            }
        }
        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("dat"))
            {
                controller = false;
                _Rigidbody2D.gravityScale = 49f;
            }
        }
        

       
    }
}