using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace ToxicGasFramework
{
    public class GasMask: MonoBehaviour
    {

        public FVRPhysicalObject Mask;
        public float MaskDamageMultiplier = 0;
        public float MaskBlindnessMultiplier = 0;
        public GameObject Audio;

        public static bool UseBreathSFX = true;

        public static List<GasMask> gasMasks = new List<GasMask>();

#if !(UNITY_EDITOR || UNITY_5)


        public void Awake()
        {
            gasMasks.Add(this);
            if (Audio != null && !UseBreathSFX)
            {
                Audio.SetActive(false);
            }

            //Debug.Log("Adding mask to list");
        }


        public void OnDestroy()
        {
            gasMasks.Remove(this);
        }



#endif
    }
}
