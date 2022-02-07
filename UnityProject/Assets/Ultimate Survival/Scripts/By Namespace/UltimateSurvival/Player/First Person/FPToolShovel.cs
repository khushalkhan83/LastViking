using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
    public class FPToolShovel : FPTool
    {
        public Message Dig { get { return m_Dig; } }

        private Message m_Dig = new Message();


        public override bool TryAttackOnce(Camera camera) 
        {
            if (Time.time < m_NextUseTime)
            {
                return false;
            }

            bool objectIsClose = false;
            var raycastData = Player.RaycastData.Value;
            if (raycastData)
            {
                objectIsClose = raycastData.HitInfo.distance < m_MaxReach;
            }

            if (objectIsClose && raycastData.HitInfo.collider.GetComponent<MineableObjectDiggingPlace>() != null)
            {
                Dig.Send();
            }
            else
            {
                MeleeAttack.Send(objectIsClose);
            }

            StartCoroutine(AttackSound());
            // Send the regular attack message.
            Attack.Send();

            m_NextUseTime = Time.time + m_TimeBetweenAttacks;

            return true;
        }
    }

}