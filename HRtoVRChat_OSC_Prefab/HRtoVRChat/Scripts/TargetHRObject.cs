#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
#if CVR_CCK_EXISTS
using ABI.CCK.Components;
#endif
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
#endif
using Object = UnityEngine.Object;

namespace HRtoVRChat.Scripts
{
    public enum HRObjectBackend
    {
        Universal,
        Shaders
    }
    
    [ExecuteInEditMode]
    public class TargetHRObject : MonoBehaviour
    {
        public HRObjectBackend Backend = HRObjectBackend.Universal;
        
        public GameObject OnesIcon;
        public GameObject TensIcon;
        public GameObject HundredsIcon;

        public HRMaterialCache OnesMaterials;
        public HRMaterialCache TensMaterials;
        public HRMaterialCache HundredsMaterials;

        public GameObject NumsIcon;

        public GameObject AvatarRoot;
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
    
    [CustomEditor(typeof(TargetHRObject))]
    public class HRObjectEditor : Editor
    {
        private int backend_index;

        private bool isOnesMaterialsOpen, isTensMaterialsOpen, isHundredsMaterialsOpen;

        public override void OnInspectorGUI()
        {
            TargetHRObject hrObject = target as TargetHRObject;
            bool previewMode = !isInstancedInScene(hrObject);
            if (previewMode)
                EditorGUILayout.HelpBox(
                    "You are currently viewing in Preview Mode. No changes can be made until the prefab is instanced in the scene.",
                    MessageType.Info);
            using (new EditorGUI.DisabledScope(previewMode))
            {
                hrObject.Backend =
                (HRObjectBackend) EditorGUILayout.Popup("Backend", (int) hrObject.Backend,
                    new[] {"Universal", "Shaders"});
            switch (hrObject.Backend)
            {
                case HRObjectBackend.Universal:
                    // Try and find target children
                    GameObject detectedOnes = AttemptGetUniversalChild(hrObject, 0);
                    GameObject detectedTens = AttemptGetUniversalChild(hrObject, 1);
                    GameObject detectedHundreds = AttemptGetUniversalChild(hrObject, 2);
                    hrObject.OnesIcon = (GameObject) EditorGUILayout.ObjectField(new GUIContent("OnesIcon"),
                        hrObject.OnesIcon, typeof(GameObject), true);
                    hrObject.TensIcon = (GameObject) EditorGUILayout.ObjectField(new GUIContent("TensIcon"),
                        hrObject.TensIcon, typeof(GameObject), true);
                    hrObject.HundredsIcon = (GameObject) EditorGUILayout.ObjectField(new GUIContent("HundredsIcon"),
                        hrObject.HundredsIcon, typeof(GameObject), true);
                    if (hrObject.OnesIcon == null && detectedOnes != null)
                        hrObject.OnesIcon = detectedOnes;
                    if (hrObject.TensIcon == null && detectedTens != null)
                        hrObject.TensIcon = detectedTens;
                    if (hrObject.HundredsIcon == null && detectedHundreds != null)
                        hrObject.HundredsIcon = detectedHundreds;
                    // If they're still null, prompt a warning
                    if (hrObject.OnesIcon == null)
                        EditorGUILayout.HelpBox("OnesIcon is null! This will cause issues in building.",
                            MessageType.Error);
                    if (hrObject.TensIcon == null)
                        EditorGUILayout.HelpBox("TensIcon is null! This will cause issues in building.",
                            MessageType.Error);
                    if (hrObject.HundredsIcon == null)
                        EditorGUILayout.HelpBox("HundredsIcon is null! This will cause issues in building.",
                            MessageType.Error);
                    // Create foldout for materials
                    CreateFoldout(ref isOnesMaterialsOpen, "OnesMaterials", ref hrObject.OnesMaterials);
                    CreateFoldout(ref isTensMaterialsOpen, "TensMaterials", ref hrObject.TensMaterials);
                    CreateFoldout(ref isHundredsMaterialsOpen, "HundredsMaterials", ref hrObject.HundredsMaterials);
                    if (!previewMode)
                    {
                        GUILayout.Label("Universal Tools", EditorStyles.centeredGreyMiniLabel);
                        if (hrObject.Backend == HRObjectBackend.Universal)
                        {
                            // Universal only tools
                            if (GUILayout.Button("Convert Materials To Quest"))
                            {
                                hrObject.OnesMaterials.SetMaterialsToQuest();
                                hrObject.TensMaterials.SetMaterialsToQuest();
                                hrObject.HundredsMaterials.SetMaterialsToQuest();
                                hrObject.SetMaterialToQuest();
                            }
                            if (GUILayout.Button("Convert Materials To Standard"))
                            {
                                hrObject.OnesMaterials.SetMaterialsToStandard();
                                hrObject.TensMaterials.SetMaterialsToStandard();
                                hrObject.HundredsMaterials.SetMaterialsToStandard();
                                hrObject.SetMaterialToStandard();
                            }
                        }
                    }
                    break;
                case HRObjectBackend.Shaders:
                    if (Shader.Find("RED_SIM/Simple Counter") != null)
                    {
                        EditorGUILayout.HelpBox(
                            "Using Shaders restricts use to PC Only! Please use Universal when possible, as it supports Quest too.",
                            MessageType.Warning);
                        GameObject detectedNums = AttemptGetShadersChild(hrObject, 0);
                        hrObject.NumsIcon = (GameObject) EditorGUILayout.ObjectField(new GUIContent("NumsIcon"),
                            hrObject.NumsIcon, typeof(GameObject), true);
                        if (hrObject.NumsIcon == null && detectedNums != null)
                            hrObject.NumsIcon = detectedNums;
                        if (hrObject.NumsIcon == null)
                            EditorGUILayout.HelpBox("NumsIcon is null! This will cause issues in building.",
                                MessageType.Error);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(
                            "RED_SIM's Simple Counter Shader was not found! Please download it and import it using the button below",
                            MessageType.Error);
                        if (GUILayout.Button("Download RED_SIM's Simple Counter Shader"))
                        {
                            Process.Start("https://www.patreon.com/posts/62864361");
                        }
                    }
                    break;
            }
            // Required regardless
            GUILayout.Label("Required", EditorStyles.centeredGreyMiniLabel);
            hrObject.AvatarRoot = (GameObject) EditorGUILayout.ObjectField(new GUIContent("AvatarRoot"),
                hrObject.AvatarRoot, typeof(GameObject), true);
            if (!previewMode)
            {
                if(hrObject.AvatarRoot == null)
                    EditorGUILayout.HelpBox("AvatarRoot is null! This is required.", MessageType.Error);
                else
                {
#if VRC_SDK_VRCSDK3
                    VRCAvatarDescriptor vrcAvatarDescriptor = hrObject.AvatarRoot.GetComponent<VRCAvatarDescriptor>();
                    if (vrcAvatarDescriptor != null)
                    {
                        if (vrcAvatarDescriptor.expressionParameters == null)
                            EditorGUILayout.HelpBox(
                                "No ExpressionParameters are attached to the VRCAvatarDescriptor! " +
                                "ExpressionParameters will not be added automatically, and you will be responsible for " +
                                "adding them manually.",
                                MessageType.Warning);
                        if (hrObject.FXController != null)
                        {
                            foreach (VRCAvatarDescriptor.CustomAnimLayer specialAnimationLayer in vrcAvatarDescriptor
                                         .baseAnimationLayers)
                            {
                                if (specialAnimationLayer.type == VRCAvatarDescriptor.AnimLayerType.FX)
                                {
                                    if (specialAnimationLayer.animatorController != hrObject.FXController)
                                        EditorGUILayout.HelpBox(
                                            "The FX Controller on your VRCAvatarDescriptor is not the same as the one " +
                                            "provided below. Was this intended?",
                                            MessageType.Warning);
                                }
                            }
                        }
                    }
                    else
                        EditorGUILayout.HelpBox("No VRCAvatarDescriptor was found! This may cause issues during building.",
                            MessageType.Error);
#endif
#if CVR_CCK_EXISTS
                    CVRAvatar cvrAvatar = hrObject.AvatarRoot.GetComponent<CVRAvatar>();
                    if (cvrAvatar != null)
                    {
                        if (cvrAvatar.avatarSettings.baseController != hrObject.FXController)
                        {
                            EditorGUILayout.HelpBox(
                                "The AnimatorController on your CVRAvatar is not the same as the one " +
                                "provided below. Was this intended?",
                                MessageType.Warning);
                        }
                    }
                    else
                        EditorGUILayout.HelpBox("No CVRAvatar was found! This may cause issues during building.",
                            MessageType.Error);
#endif
#if CVR_CCK_EXISTS && VRC_SDK_VRCSDK3 
#else
                    EditorGUILayout.HelpBox("No valid SDK/CCK detected! This may cause issues during building.",
                            MessageType.Error);
#endif
                }
            }
            hrObject.FXController = (AnimatorController) EditorGUILayout.ObjectField(new GUIContent("FXController"),
                hrObject.FXController, typeof(AnimatorController), true);
            if(hrObject.FXController == null && !previewMode)
                EditorGUILayout.HelpBox("FXController is null! This is required.", MessageType.Error);
            }
        }

        private GameObject AttemptGetUniversalChild(TargetHRObject hrObject, int index)
        {
            GameObject found = null;
            try
            {
                found = hrObject.transform.GetChild(index).gameObject;
            }
            catch(Exception){}

            if (found != null)
            {
                switch (index)
                {
                    case 0:
                        if (found.name != "ones")
                            found = null;
                        break;
                    case 1:
                        if (found.name != "tens")
                            found = null;
                        break;
                    case 2:
                        if (found.name != "hundreds")
                            found = null;
                        break;
                }
            }
            return found;
        }
        
        private GameObject AttemptGetShadersChild(TargetHRObject hrObject, int index)
        {
            GameObject found = null;
            try
            {
                found = hrObject.transform.GetChild(index).gameObject;
            }
            catch(Exception){}

            switch (index)
            {
                case 0:
                    if (found.name != "nums")
                        found = null;
                    break;
            }
            return found;
        }

        private void CreateFoldout<T>(ref bool open, string label, ref T objects)
        {
            open = EditorGUILayout.Foldout(open, label);
            if (open)
            {
                foreach (FieldInfo fieldInfo in objects.GetType().GetFields())
                {
                    Object objValue = (Object) Convert.ChangeType(fieldInfo.GetValue(objects), fieldInfo.FieldType);
                    fieldInfo.SetValue(objects, Convert.ChangeType(
                        EditorGUILayout.ObjectField(new GUIContent(fieldInfo.Name), objValue, fieldInfo.FieldType, true),
                        fieldInfo.FieldType));
                }
            }
        }
        
        private static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }

        private static bool isInstancedInScene(TargetHRObject hrObject)
        {
            string path = GetGameObjectPath(hrObject.gameObject);
            if (path != "/" + hrObject.name)
                return true;
            foreach (GameObject rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (rootGameObject == hrObject.gameObject)
                    return true;
            }
            return false;
        }
    }
}
#endif