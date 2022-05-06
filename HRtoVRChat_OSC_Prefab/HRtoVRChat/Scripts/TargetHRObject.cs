#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace HRtoVRChat.Scripts
{
    [ExecuteInEditMode]
    public class TargetHRObject : MonoBehaviour
    {
        public GameObject OnesIcon;
        public GameObject TensIcon;
        public GameObject HundredsIcon;

        public HRMaterialCache OnesMaterials;
        public HRMaterialCache TensMaterials;
        public HRMaterialCache HundredsMaterials;

        public VRCAvatarDescriptor AvatarRoot;
        public AnimatorController FXController;

        public HRMaterialCache HRMaterialCacheFromLoop(AnimatorCreator.NumberSpots loopControlVariable)
        {
            switch (loopControlVariable)
            {
                case AnimatorCreator.NumberSpots.Ones:
                    return OnesMaterials;
                case AnimatorCreator.NumberSpots.Tens:
                    return TensMaterials;
                case AnimatorCreator.NumberSpots.Hundreds:
                    return HundredsMaterials;
            }
            return null;
        }

        public void SetMaterialToQuest()
        {
            Renderer thisRenderer = gameObject.GetComponent<Renderer>();
            Material newMaterial = thisRenderer.sharedMaterial;
            newMaterial.shader = Shader.Find("VRChat/Mobile/Standard Lite");
            newMaterial.color = new Color(1f, 1f, 1f);
            if(File.Exists("Assets/HRtoVRChat/hr-quest.png"))
                newMaterial.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/HRtoVRChat/hr-quest.png");
            string assetPath = AssetDatabase.GetAssetPath(newMaterial);
            if (assetPath != String.Empty)
            {
                AssetDatabase.RemoveObjectFromAsset(newMaterial);
                AssetDatabase.CreateAsset(newMaterial, assetPath);
                thisRenderer.sharedMaterial = newMaterial;
            }
        }
        
        public void SetMaterialToStandard()
        {
            Renderer thisRenderer = gameObject.GetComponent<Renderer>();
            Material newMaterial = thisRenderer.sharedMaterial;
            newMaterial.shader = Shader.Find("Standard");
            newMaterial.color = new Color(1f, 0.3443396f, 0.4872888f);
            if(File.Exists("Assets/HRtoVRChat/hr.png"))
                newMaterial.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/HRtoVRChat/hr.png");
            string assetPath = AssetDatabase.GetAssetPath(newMaterial);
            if (assetPath != String.Empty)
            {
                AssetDatabase.RemoveObjectFromAsset(newMaterial);
                AssetDatabase.CreateAsset(newMaterial, assetPath);
                thisRenderer.sharedMaterial = newMaterial;
            }
        }
    }

    [Serializable]
    public class HRMaterialCache
    {
        public Material one;
        public Material two;
        public Material three;
        public Material four;
        public Material five;
        public Material six;
        public Material seven;
        public Material eight;
        public Material nine;
        public Material zero;

        public Material GetNumberMaterialFromLoop(int loopControlVariable)
        {
            switch (loopControlVariable)
            {
                case 0:
                    return zero;
                case 1:
                    return one;
                case 2:
                    return two;
                case 3:
                    return three;
                case 4:
                    return four;
                case 5:
                    return five;
                case 6:
                    return six;
                case 7:
                    return seven;
                case 8:
                    return eight;
                case 9:
                    return nine;
            }
            return null;
        }

        public void SetMaterialsToQuest()
        {
            foreach (FieldInfo fieldInfo in GetType().GetFields())
            {
                Material targetMaterial = (Material) Convert.ChangeType(fieldInfo.GetValue(this), typeof(Material));
                targetMaterial.shader = Shader.Find("VRChat/Mobile/Standard Lite");
                string assetPath = AssetDatabase.GetAssetPath(targetMaterial);
                if (assetPath != String.Empty)
                {
                    AssetDatabase.RemoveObjectFromAsset(targetMaterial);
                    AssetDatabase.CreateAsset(targetMaterial, assetPath);
                    fieldInfo.SetValue(this, targetMaterial);
                }
            }
        }
        
        public void SetMaterialsToStandard()
        {
            foreach (FieldInfo fieldInfo in GetType().GetFields())
            {
                Material targetMaterial = (Material) Convert.ChangeType(fieldInfo.GetValue(this), typeof(Material));
                targetMaterial.shader = Shader.Find("Standard");
                string assetPath = AssetDatabase.GetAssetPath(targetMaterial);
                if (assetPath != String.Empty)
                {
                    AssetDatabase.RemoveObjectFromAsset(targetMaterial);
                    AssetDatabase.CreateAsset(targetMaterial, assetPath);
                    fieldInfo.SetValue(this, targetMaterial);
                }
            }
        }
    }
}
#endif