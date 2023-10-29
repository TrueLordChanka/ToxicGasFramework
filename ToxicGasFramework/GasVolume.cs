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

        private Dictionary<GameObject, Coroutine> coroutines = new Dictionary<GameObject, Coroutine>();
        private string[] sosigMaskList = { "SosigAccessory_Tactical_GasMask1(Clone)", "SosigAccessory_Tactical_GasMask2(Clone)", "SosigAccessory_Tactical_GasMask3(Clone)", "Sosigaccesory_MF2_Red_PY_mask(Clone)", "Sosigaccesory_MF2_Blue_PY_mask(Clone)",
            "MG_Ninja_Helmet(Clone)", "MG_Telekine_Head(Clone)", "Sosig_HL_UncivilErection_Helmet(Clone)", "Sosig_HalfLife_OverwatchElite_Helmet(Clone)", "Sosig_HalfLife_OverwatchSoldier_Helmet(Clone)", "MG_Chemwar_Head(Clone)", "Operator_Gasmask_Black(Clone)"
        };

        public bool ignoresMasks;   //Should the GasVolume ignore gas masks and damage things anyway?

        private List<GameObject> parentsBeingDamaged = new List<GameObject>();
        public List<GameObject> debug_DamagedEntities;

        void Update()
        {
            debug_DamagedEntities = parentsBeingDamaged;
        }

        void OnTriggerEnter(Collider collider) //when somthing enters the collider
        {
            if (!(collider.gameObject.layer == LayerMask.NameToLayer("PlayerHead") || collider.GetComponent<SosigLink>() != null)) return; //Exclude all collisions outside of PlayerBody and AgentBody

            //Debug.Log("Collided with " + collider.gameObject.name);

            GameObject entity = GetSosigOrPlayer(collider);
            if (entity != null) //Script should have acquired a player or sosig
            {
                if (!parentsBeingDamaged.Contains(entity))
                {
                    bool isPlayer = entity.GetComponent<FVRPlayerBody>() != null ? true : false;    //detects whether it is a player or sosig
                    parentsBeingDamaged.Add(entity);
                    Coroutine coroutine = StartCoroutine(DamageTick(collider, isPlayer));
                    coroutines.Add(entity, coroutine);
                }
            }
        }

        void OnTriggerExit(Collider collider)
        {
            //if (!(collider.tag == "PlayerHead" || collider.tag == "AgentBody")) return;   //Exclude all collisions outside of PlayerBody and AgentBody
            //Debug.Log("Collision Exit");

            GameObject entity = GetSosigOrPlayer(collider);
            Coroutine coroutineToKill;

            if (parentsBeingDamaged.Contains(entity))
            {
                if (coroutines.TryGetValue(entity, out coroutineToKill))
                {
                    parentsBeingDamaged.Remove(entity);
                    StopCoroutine(coroutineToKill);
                    coroutines.Remove(entity);
                }
            }
        }

        IEnumerator DamageTick(Collider collider, bool _isPlayer)
        {

            while (true)
            {
                //Debug.Log("volume tick dealing " + VolumeGasData.Damage + " Damage, " + VolumeGasData.PlayerBlindness + " Player Blindness, with a "+ VolumeGasData.EffectTick + " Tick"); ;

                //determine if its a sosig or player
                if (_isPlayer)
                {
                    //Debug.Log("UwU");
                    //its a player
                    if (collider.GetComponent<FVRPlayerHitbox>() != null)
                    {
                        //Debug.Log("Hitbox is active");
                        FVRPlayerHitbox hitbox = collider.GetComponent<FVRPlayerHitbox>();
                        //Debug.Log($"PlayerBlindness._isAMaskEquipped = {PlayerBlindness._isAMaskEquipped}, ignoresMasks = {ignoresMasks}");
                        if (!PlayerBlindness._isAMaskEquipped || ignoresMasks) //if a mask is not equipped or ignoresMasks is enabled
                        {
                            //Debug.Log("AAAAA");
                            //do damage and blindess
                            hitbox.Damage((int)(VolumeGasData.Damage + 1));
                            //Debug.Log("Took " + VolumeGasData.Damage + " Damage");

                            if (PlayerBlindness.BlindnessPercent < VolumeGasData.PlayerMaxBlindness)
                            {
                                PlayerBlindness.BlindnessPercent += VolumeGasData.PlayerBlindness;
                            }
                        }
                        else //a mask is equipped
                        {
                            hitbox.Damage((int)((VolumeGasData.Damage + 1) * PlayerBlindness._DamBlindVals.x));
                            if (PlayerBlindness.BlindnessPercent < VolumeGasData.PlayerMaxBlindness)
                            {
                                PlayerBlindness.BlindnessPercent += VolumeGasData.PlayerBlindness * PlayerBlindness._DamBlindVals.y;
                            }
                        }
                    }
                }
                else
                {
                    //It's a sosig
                    //Debug.Log("UwU2");

                    if (collider != null)
                    {
                        Sosig targetedSosig = collider.GetComponent<SosigLink>().S;
                        if (targetedSosig.BodyState != Sosig.SosigBodyState.Dead)
                        {
                            if (!IsSosigMasked(targetedSosig))
                            {
                                foreach (SosigLink link in targetedSosig.Links)
                                {
                                    if (link != null)
                                    {
                                        //Debug.Log(link.name + " Sosig link hopefully");
                                        link.Damage(new Damage //apply both blinding and blunt damage to the sosig based on the damage
                                        {
                                            Dam_Blinding = VolumeGasData.SosigBlindness,
                                            Dam_Blunt = VolumeGasData.Damage,
                                            Dam_Stunning = VolumeGasData.SosigStun
                                        });
                                        if (link == null)
                                        {
                                            yield return null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(VolumeGasData.EffectTick); //wait for tick
            }
        }

        private GameObject GetSosigOrPlayer(Collider collider)
        {
            GameObject other = null;

            if (collider.gameObject.GetComponentInParent<FVRPlayerBody>()) //Is a player
            {
                other = collider.transform.root.gameObject;
            }
            else if (collider.gameObject.GetComponent<SosigLink>() != null) //Is a sosig
            {
                other = collider.gameObject.GetComponent<SosigLink>().S.gameObject;
            }
            return other;
        }

        private bool IsSosigMasked(Sosig _sosig)
        {
            if (ignoresMasks) return false;
            for (int i = 0; i < _sosig.Links.Count; i++)    //for loop probably unnecessary, but just in case
            {
                for (int j = 0; j < sosigMaskList.Length; j++)
                {
                    //Debug.Log(j + "index");
                    if (_sosig.Links[i].transform.Find(sosigMaskList[j]) != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}