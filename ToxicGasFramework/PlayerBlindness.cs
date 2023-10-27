using System;
using System.Collections;
using System.Collections.Generic;
using OpenScripts2;
using FistVR;
using UnityEngine;


namespace ToxicGasFramework
{
    public class PlayerBlindness : MonoBehaviour
    {

        public static float BlindnessPercent = 0f;
        public Vector3 HeadOffsetPosition = new Vector3(0, 0.015f, -0.05f);
        public float BlindLerpOn = 0.5f;
        public float BlindDegradationPerSecond = 2f;
        public AnimationCurve AnimationCurve;
        [HideInInspector]
        public static bool _isAMaskEquipped; //determines whether a mask is equiped or not
        [HideInInspector]
        public static Vector2 _DamBlindVals = new Vector2(0, 0);

        private Color BlindColor;
        private MaterialPropertyBlock PropertyBlock = new MaterialPropertyBlock();


        public static GameObject prefab;
        public List<Renderer> Renderers = new List<Renderer>();


        public void Start()
        {
            //transform.SetParent(GM.CurrentPlayerBody.Head, false); Do somthing like this to the prefab
            //transform.localPosition += this.HeadOffsetPosition;
            //I need to spawn the blindness sphere and move it
            //Debug.Log("pineapple");
            prefab.transform.SetParent(GM.CurrentPlayerBody.Head, false);
            prefab.transform.localPosition = new Vector3 (0,0,0);
            prefab.transform.localPosition += HeadOffsetPosition;

            BlindColor = new Color(0,0,0,0);
            //color shit
            //Debug.Log(BlindColor.a + " Blindcolor a");
            


        }


        
        public void Update()
        {
            //do the mask stuff
            if (GasMask.gasMasks.Count != 0)
            {
                bool b = false;
                foreach (GasMask m in GasMask.gasMasks)
                {
                    if (m.Mask.QuickbeltSlot != null && m.Mask.QuickbeltSlot.GetComponent<HeadQBSlot>() != null) //If its in a qb slot, and the slot is a player (not a mask carrier)
                    {
                        b = true;
                        _DamBlindVals = new Vector2(m.MaskDamageMultiplier, m.MaskBlindnessMultiplier);
                    }

                }
                if (b)
                {
                    _isAMaskEquipped = true;
                    
                }
                else
                {
                    _isAMaskEquipped = false;
                }
            }
            else
            {
                GasMask.gasMasks.Clear();
                _isAMaskEquipped = false;
            }
            //Debug.Log(_isAMaskEquipped);

            //do blindness stuff
            if (BlindnessPercent > 0f)
            {
                //Debug.Log("Blind % > 0, DoingColorChange");
                BlindnessPercent -= BlindDegradationPerSecond * Time.deltaTime; //do blindness Degradation

                //update the color
                //ensure that the value is between 0 and 1
                if (BlindnessPercent > 1.0f)
                {
                    BlindnessPercent = 1.0f;
                }

                BlindColor.a = AnimationCurve.Evaluate(BlindnessPercent);
                PropertyBlock.SetColor("_Color", BlindColor);
                foreach (Renderer r in Renderers) //apply all the renderers to the property block
                {
                    r.SetPropertyBlock(PropertyBlock);
                }
            } else if (BlindnessPercent <= 0f && BlindColor.a != 0f)
            {
                BlindColor.a = 0f;
                PropertyBlock.SetColor("_Color", BlindColor);
                BlindnessPercent = 0f;
                foreach (Renderer r in Renderers) //apply all the renderers to the property block
                {
                    r.SetPropertyBlock(PropertyBlock);
                }
            }


        }
        

    }
}
