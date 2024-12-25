using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code_Battle_System.Items
{
    public enum Effect_type
    {
        Heal,
        Damage,
        Invisibility,
        Toxic
    }

    public enum Target_type
    {
        Self,
        Teammate,
        Enemy,
        All
    }

    public class MainItem : MonoBehaviour
    {
        public int IdItem;
        public String ItemName;
        public Effect_type effect_type;
        public int ValueItem;
        public int Duration;
        public Target_type target_type;
        

       

        public void Invisible()
        {
            if (effect_type == Effect_type.Invisibility)
            {
                if (target_type == Target_type.Self)
                {
                    // Gửi thông tin rằng nhân vật tàng hình lên server tại đây.
                }

                if (target_type == Target_type.Teammate)
                {
                    // Gửi thông tin rằng nhân vật và đồng đội tàng hình lên server tại đây.
                }
            }
        }

        public void Healling()
        {
            if (effect_type== Effect_type.Heal)
            {
                if (target_type == Target_type.Self)
                {
                    // Gửi thông tin rằng nhân vật hồi máu lên server tại đây.
                }

                if (target_type == Target_type.Teammate)
                {
                    // Gửi thông tin rằng nhân vật và đồng đội hồi máu lên server tại đây.
                }
                
            }
        }

        public void ToxicOn()
        {
            if (effect_type== Effect_type.Toxic)
            {
                // Chưa rõ ý tưởng này có độc toàn bộ người chơi hay không? Nhưng gờ chỉ nghĩ đến việc độc đối thủ đã.
                if (target_type== Target_type.Enemy)
                {
                    // Chuyển sang loại đạn có độc và ném vào đối thủ, độc khiến cho đối phương mất máu theo từng lượt.
                    
                }
                
            }
        }

        public void DamageOn()
        {
            if (effect_type == Effect_type.Damage)
            {
                // Chắc chắn rằng cái này chỉ tấn công đối thủ rồi.
                if (target_type== Target_type.Enemy)
                {
                    // Cũng sẽ chuyển sang loại đạn có damage to hơn.
                }
            }
        }
            
    }
}
