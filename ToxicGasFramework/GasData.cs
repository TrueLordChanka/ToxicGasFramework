using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ToxicGasFramework
{
    [Serializable]
    public class GasData
    {
        public float Damage;
        public float SosigBlindness;
        public float SosigStun = 0;
        public float PlayerBlindness;
        public float PlayerMaxBlindness = 1;
        [Tooltip("Tick is in seconds")]
        public float EffectTick;
        
    }
}
