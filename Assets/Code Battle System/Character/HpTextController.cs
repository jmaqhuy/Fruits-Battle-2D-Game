using UnityEngine;

namespace Code_Battle_System.Character
{
    public class HpTextController:MonoBehaviour
    {
        private float time = 0f;
        private float maxTimeFly = 2f;
        private float flyUpSpeed = 1f;
        void Update()
        {
            time += Time.deltaTime;
            if (time > maxTimeFly)
            {
                Destroy(gameObject);
            }
            transform.Translate(Vector2.up * flyUpSpeed * Time.deltaTime);
        }
    }
}