using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NetworkThread;
using RoomEnum;

namespace Code_Battle_System.Character
{
    public class Unit : MonoBehaviour
    {
        private string unitName;
        private int HeathMax;
        private int HealthCurrent;
        private int Attack;
        private int Amor;
        private int Lucky;
        private bool isLeft;
        private Team playerTeam;
        public Slider healthSlider;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI healthText;
        public Gradient gradientColor;
        
        public void setAttack(int attack)
        {
            this.Attack = attack;
        }
        public int getAttack() { return this.Attack; }
        public void setArmor(int armor) 
        {
            this.Amor = armor;
        }
        public int getArmor() { return this.Amor; }
        public void setLucky(int Lucky)
        {
            this.Lucky = Lucky;
        }
        public int getLucky() { return this.Lucky; }
        public void TakeDamage(int damage)
        {
            this.HealthCurrent -= damage;
        }

        public void setHealthCurrent(int health)
        {
            
            this.HealthCurrent = health;
            float healthPercentage = (float)this.HealthCurrent/(float)this.HeathMax;
            healthSlider.value =  healthPercentage;
            healthText.text = this.HealthCurrent+"/"+this.HeathMax;
            healthSlider.fillRect.GetComponent<Image>().color = gradientColor.Evaluate(healthPercentage);

           
        }

        
        public void setHealthMax(int health)
        {
            this.HeathMax = health;

        }

        public void setUnitName(string unitName)
        {
            this.unitName = unitName;
        }

        public void SetDisplayName(string displayName, Color c)
        {
            nameText.text = displayName;
            nameText.color = c;
        }

        public int getHealthCurrent()
        {
            return this.HealthCurrent;
        }

        public int getHealthMax()
        {
            return this.HeathMax;
        }
        

        public string getUnitName()
        {
            return this.unitName;
        }
       

       
        public void setIsLest(bool isLest) 
        {
            this.isLeft = isLest;
        }
        public bool getIsLest() 
        {
            return this.isLeft;
        }
        public void setTeam(Team team)
        {
            playerTeam = team;
        }
        public Team getTeam()
        {
            return playerTeam;
        }
        public void Destroy()
        {
            Destroy(gameObject);
        }

    }
}