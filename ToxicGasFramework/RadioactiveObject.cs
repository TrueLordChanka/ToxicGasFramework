using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Unity;
using FistVR;

namespace ToxicGasFramework
{
    public class RadioactiveObject : MonoBehaviour
    {
        public float StrengthAt1m = 10f;
        public float MinEffectDist = 0.2f;
        public float MaxEffectDist = 20f;
        public float BaseDmgProbability = 0.001f;
        public float MaxProximityProbability = 0.8f;

        private float StrengthAtSource;
        public static List<RadioactiveObject> radioactiveObjects = new List<RadioactiveObject>();
        private Transform TorsoPt;
        private FVRPlayerBody playerBody;
        public void Awake()
        {
            StrengthAtSource = 4f * 3.14f * StrengthAt1m;
            playerBody = GM.CurrentPlayerBody;
            radioactiveObjects.Add(this);
            TorsoPt = playerBody.transform.Find("Torso");

        }

        public void Update()
        {
            if(!Physics.Linecast(transform.position, TorsoPt.position, 19))
            {
                float distFromSource = Vector3.Distance(TorsoPt.position, transform.position);
                float I = StrengthAtSource / (4f * 3.14f * distFromSource);
                float proxFactor = Mathf.Clamp01((MaxEffectDist - distFromSource) / (MaxEffectDist - MinEffectDist));
                float adjustedProbability = Mathf.Lerp(BaseDmgProbability, MaxProximityProbability, proxFactor);
                float randomNum = UnityEngine.Random.value;

                if (randomNum <= adjustedProbability && I > 1f)
                {
                    Debug.Log(I);
                    playerBody.Health -= I;
                    playerBody.HarmPercent(0);
                    


                }
            }
            
        }



    }
}
