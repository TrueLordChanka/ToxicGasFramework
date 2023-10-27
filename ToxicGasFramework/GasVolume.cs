using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using System.Collections.Generic;

namespace ToxicGasFramework
{
    [RequireComponent(typeof(Collider))] //each volume needs a collider

    public class GasVolume : MonoBehaviour
    {
        public GasData VolumeGasData;

        private Dictionary<Collider, Coroutine> coroutines = new Dictionary<Collider, Coroutine>();
        private string[] sosigMaskList = { "SosigAccessory_Tactical_GasMask1(Clone)", "SosigAccessory_Tactical_GasMask2(Clone)", "SosigAccessory_Tactical_GasMask3(Clone)", "Sosigaccesory_MF2_Red_PY_mask(Clone)", "Sosigaccesory_MF2_Blue_PY_mask(Clone)", 
            "MG_Ninja_Helmet(Clone)", "MG_Telekine_Head(Clone)", "Sosig_HL_UncivilErection_Helmet(Clone)", "Sosig_HalfLife_OverwatchElite_Helmet(Clone)", "Sosig_HalfLife_OverwatchSoldier_Helmet(Clone)", "MG_Chemwar_Head(Clone)", "Operator_Gasmask_Black(Clone)"
        };

#if !(UNITY_EDITOR || UNITY_5)


        void OnTriggerEnter(Collider collider) //when somthing enters the collider
        {
            //Debug.Log("Collision detected");
            LayerMask layer;
            
            layer = collider.gameObject.layer;
            //Debug.Log("b "+ layer.ToString()); //this next check isnt worken for sosigs
            if (layer == LayerMask.NameToLayer("PlayerHead") || (layer == LayerMask.NameToLayer("AgentBody") && collider.attachedRigidbody.gameObject.GetComponent<SosigLink>() != null && collider.attachedRigidbody.gameObject.GetComponent<SosigLink>().BodyPart == SosigLink.SosigBodyPart.Head))//collider.gameObject.GetComponent<SosigLink>() != null&& collider.gameObject.GetComponent<SosigLink>().BodyPart == SosigLink.SosigBodyPart.Head
            {
                //Debug.Log("ValidTarget");
                Coroutine coroutine = StartCoroutine(DamageTick(collider));
                coroutines.Add(collider, coroutine);
            }
        }
        
        void OnTriggerExit(Collider collider)
        {
            //Debug.Log("Collision Exit");
            Coroutine coroutineToKill;
            if(coroutines.TryGetValue(collider, out coroutineToKill))
            {
                StopCoroutine(coroutineToKill);
                coroutines.Remove(collider);
            }
        }
        
        IEnumerator DamageTick(Collider collider)
        {
            while (true)
            {
                //Debug.Log("volume tick dealing " + VolumeGasData.Damage + "Damage, " + VolumeGasData.Blindness + " Blindness, with a "+ VolumeGasData.EffectTick + " Tick"); ;

                //determine if its a sosig or player
                if (collider.GetComponent<FVRPlayerHitbox>() != null && collider.GetComponent<FVRPlayerHitbox>().Type == FVRPlayerHitbox.PlayerHitBoxType.Head)
                {
                    //Debug.Log("UwU");
                    //its a player
                    if (collider.GetComponent<FVRPlayerHitbox>() != null)
                    {
                        FVRPlayerHitbox hitbox = collider.GetComponent<FVRPlayerHitbox>();
                        if (!PlayerBlindness._isAMaskEquipped) //if a mask is not equipped 
                        {
                            //Debug.Log("AAAAA");
                            //do damage and blindess
                            hitbox.Damage(VolumeGasData.Damage);
                            if (PlayerBlindness.BlindnessPercent < VolumeGasData.PlayerMaxBlindness)
                            {
                                PlayerBlindness.BlindnessPercent += VolumeGasData.PlayerBlindness;
                            }

                        }
                        else //a mask is equipped
                        {
                            hitbox.Damage(VolumeGasData.Damage * PlayerBlindness._DamBlindVals.x);
                            if (PlayerBlindness.BlindnessPercent < VolumeGasData.PlayerMaxBlindness)
                            {
                                PlayerBlindness.BlindnessPercent += VolumeGasData.PlayerBlindness * PlayerBlindness._DamBlindVals.y;
                            }
                        }
                    }
                }else if (collider.gameObject.layer == LayerMask.NameToLayer("AgentBody") && collider.attachedRigidbody.gameObject.GetComponent<SosigLink>().BodyPart == SosigLink.SosigBodyPart.Head) //its a sosig, is it their head? //&& collider.GetComponent<SosigLink>().BodyPart == SosigLink.SosigBodyPart.Head
                {
                    //Debug.Log("UwU2");
                    SosigLink component = collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
                    
                    
                    if(IsSosigMasked(component) == null)
                    {
                        //Debug.Log(component.name + " Sosig link hopefully");
                        component.Damage(new Damage //apply both blinding and blunt damage to the sosig based on the damage
                        {
                            Dam_Blinding = VolumeGasData.SosigBlindness,
                            Dam_Blunt = VolumeGasData.Damage,
                            Dam_Stunning = VolumeGasData.SosigStun
                        });
                    }

                }
                yield return new WaitForSeconds(VolumeGasData.EffectTick); //wait for tick
            }
        }

        private Transform IsSosigMasked(SosigLink component)
        {
            for (int i = 0; i < sosigMaskList.Length; i++)
            {
                //Debug.Log(i + "index");
                if (component.transform.Find(sosigMaskList[i]) != null)
                {
                    return component.transform.Find(sosigMaskList[i]);
                }
                
            }
            return null;


        }


#endif
    }
}
