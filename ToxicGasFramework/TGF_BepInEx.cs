using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using FistVR;
using UnityEngine;

namespace ToxicGasFramework
{
#if !DEBUG
    [BepInPlugin("h3vr.andrew_ftw.TGF", "Toxic Gas Framework", "1.0.0")]
    public class TGF_BepInEx : BaseUnityPlugin
    {
        public ConfigEntry<bool> UseBreathingSFX;

        
        private GameObject _loadedPrefab;

        public TGF_BepInEx()
        {
            Logger.LogInfo("Toxic Gas Framework Loaded");

            //add config stuff here if I need any. 
            UseBreathingSFX = Config.Bind<bool>("Settings", "Does use breakthing sfx", true);

            GasMask.UseBreathSFX = UseBreathingSFX.Value;

        }

        public void Awake()
        {
           
            string pluginPath = Path.GetDirectoryName(Info.Location);
            AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(pluginPath, "tfgassets"));

            _loadedPrefab = bundle.LoadAsset<GameObject>("BlurSphere");
            PlayerBlindness.prefab = Instantiate(_loadedPrefab);


        }

        public void Update()
        {
            if(GM.CurrentPlayerBody != null && GM.CurrentPlayerBody.Head != null && PlayerBlindness.prefab == null)
            {
                PlayerBlindness.prefab = Instantiate(_loadedPrefab);
                Debug.Log("spawned the thing again cause the scene probably changed");
            }

        }


    }
#endif
}
