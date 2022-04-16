#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace HRtoVRChat.Scripts
{
    public static class AnimatorCreator
    {
        /// <summary>
        /// Essential enumeration for defining a placeholder value in a HeartRate
        /// </summary>
        public enum NumberSpots
        {
            Ones,
            Tens,
            Hundreds
        }
        
        /// <summary>
        /// Used in Loops to find the current HRObject child (ones, tens, hundreds GameObjects)
        /// </summary>
        /// <param name="hro">The HRObject to check against</param>
        /// <param name="loopControlVariable">The loop variable from the loop</param>
        /// <returns></returns>
        private static Transform HRObjectFromLoop(TargetHRObject hro, int loopControlVariable)
        {
            switch (loopControlVariable)
            {
                case 0:
                    return hro.OnesIcon.transform;
                case 1:
                    return hro.TensIcon.transform;
                case 2:
                    return hro.HundredsIcon.transform;
            }
            return null;
        }

        /// <summary>
        /// Used in loops to create an identifier for animations (not compatible with useSmallerHR)
        /// </summary>
        /// <param name="x">The ones, tens, hundreds represented as an int (0-2)</param>
        /// <param name="y">The current number being animated (0-9)</param>
        /// <returns></returns>
        private static string LoopToIdentifier(int x, int y)
        {
            string xname = String.Empty;
            switch (x)
            {
                case 0:
                    xname = "ones";
                    break;
                case 1:
                    xname = "tens";
                    break;
                case 2:
                    xname = "hundreds";
                    break;
            }
            string yname = String.Empty;
            switch (y)
            {
                case 0:
                    yname = "zero";
                    break;
                case 1:
                    yname = "one";
                    break;
                case 2:
                    yname = "two";
                    break;
                case 3:
                    yname = "three";
                    break;
                case 4:
                    yname = "four";
                    break;
                case 5:
                    yname = "five";
                    break;
                case 6:
                    yname = "six";
                    break;
                case 7:
                    yname = "seven";
                    break;
                case 8:
                    yname = "eight";
                    break;
                case 9:
                    yname = "nine";
                    break;
            }
            return xname + "-" + yname;
        }

        /// <summary>
        /// Converts a number (represented as a char) to a word.
        /// </summary>
        /// <param name="number">The number</param>
        /// <returns></returns>
        private static string NumberToWord(char number)
        {
            switch (number)
            {
                case '0':
                    return "zero";
                case '1':
                    return "one";
                case '2':
                    return "two";
                case '3':
                    return "three";
                case '4':
                    return "four";
                case '5':
                    return "five";
                case '6':
                    return "six";
                case '7':
                    return "seven";
                case '8':
                    return "eight";
                case '9':
                    return "nine";
            }
            return null;
        }
        
        /// <summary>
        /// Converts a word to a number (int) ranging from 0 - 9
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static int WordToNumber(string word)
        {
            switch (word)
            {
                case "zero":
                    return 0;
                case "one":
                    return 1;
                case "two":
                    return 2;
                case "three":
                    return 3;
                case "four":
                    return 4;
                case "five":
                    return 5;
                case "six":
                    return 6;
                case "seven":
                    return 7;
                case "eight":
                    return 8;
                case "nine":
                    return 9;
            }
            return 0;
        }
        
        /// <summary>
        /// Converts an int (x) into a easily parsable format.
        /// </summary>
        /// <param name="x">The int to parse</param>
        /// <returns></returns>
        private static string BetterX(int x = 0)
        {
            if (x <= 9)
                return "00" + x;
            if (x <= 99)
                return "0" + x;
            return x.ToString();   
        }

        /// <summary>
        /// Gets a NumberSpot from a Word
        /// </summary>
        /// <param name="numSpot"></param>
        /// <returns></returns>
        private static NumberSpots GetNumbersSpot(string numSpot)
        {
            switch (numSpot)
            {
                case "ones":
                    return NumberSpots.Ones;
                case "tens":
                    return NumberSpots.Tens;
                case "hundreds":
                    return NumberSpots.Hundreds;
            }

            return 0;
        }

        /// <summary>
        /// Gets a NumberSpot from an int
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static NumberSpots GetNumbersSpot(int x)
        {
            switch (x)
            {
                case 0:
                    return NumberSpots.Ones;
                case 1:
                    return NumberSpots.Tens;
                case 2:
                    return NumberSpots.Hundreds;
            }

            return 0;
        }

        /// <summary>
        /// Checks to see if a parameter in a list of AnimatorControllerParameters exist.
        /// This should be integrated in Unity; I should not have to write this.
        /// </summary>
        /// <param name="acps">List of Parameters</param>
        /// <param name="acpt">The Parameter Type to Check Against</param>
        /// <param name="parameterName">The Parameter Name to Check Against</param>
        /// <returns></returns>
        private static bool DoesParameterExist(AnimatorControllerParameter[] acps, AnimatorControllerParameterType acpt,
            string parameterName)
        {
            bool found = false;
            foreach (AnimatorControllerParameter acp in acps)
            {
                if (acp.name == parameterName && acp.type == acpt)
                    found = true;
            }

            return found;
        }

        /// <summary>
        /// Checks to see if a layer in a list of AnimatorControllerLayers exist. 
        /// Again, this should be integrated in Unity; I should not have to write this.
        /// </summary>
        /// <param name="acls">List of Layers</param>
        /// <param name="name">Name of the Layer</param>
        /// <returns></returns>
        private static bool DoesLayerExist(AnimatorControllerLayer[] acls, string name)
        {
            bool found = false;
            foreach (AnimatorControllerLayer animatorControllerLayer in acls)
            {
                if (animatorControllerLayer.name == name)
                    found = true;
            }

            return found;
        }
        
        /// <summary>
        /// Gets the index of a layer from a list of AnimatorControllerLayers.
        /// Unity only provides indexes of syncedLayers.
        /// </summary>
        /// <param name="acls">List of Layers</param>
        /// <param name="name">Name of the Layer</param>
        /// <returns></returns>
        private static int GetLayerIndexFromName(AnimatorControllerLayer[] acls, string name)
        {
            int found = -1;
            for (int x = 0; x < acls.Length; x++)
            {
                AnimatorControllerLayer animatorControllerLayer = acls[x];
                if (animatorControllerLayer.name == name)
                    found = x;
            }

            return found;
        }

        /// <summary>
        /// Properly Adds and Saves a Layer to an AnimatorController.
        /// Thank you azmidi for the hint
        /// </summary>
        /// <param name="animator">The AnimatorController to add a layer to</param>
        /// <param name="layer">The layer to be added</param>
        private static void AddLayerToAnimator(AnimatorController animator, AnimatorControllerLayer layer)
        {
            string assetPath = AssetDatabase.GetAssetPath(animator);
            if(assetPath != String.Empty)
                AssetDatabase.AddObjectToAsset(layer.stateMachine, assetPath);
            animator.AddLayer(layer);
        }

        public static void BeginProcess(GameObject Avatar, TargetHRObject HRObject, AnimatorController animator,
            string friendlyName = "HR", bool useSmallerHR = false, bool deleteComponentOnDone = false,
            bool overwriteLayers = false)

        {
            Debug.Log("Creating Animations...");
            Dictionary<string, AnimationClip> generatedAnimations = new Dictionary<string, AnimationClip>();
            // Create the Animation Clips
            if (useSmallerHR)
            {
                for (int x = 0; x < 256; x++)
                {
                    AnimationClip clip = new AnimationClip();
                    clip.frameRate = 1f;
                    for (int e = 0; e < 3; e++)
                    {
                        NumberSpots ns = 0;
                        Transform currentNumberTransform = null;
                        switch (e)
                        {
                            case 0:
                                ns = NumberSpots.Ones;
                                currentNumberTransform = HRObject.OnesIcon.transform;
                                break;
                            case 1:
                                ns = NumberSpots.Tens;
                                currentNumberTransform = HRObject.TensIcon.transform;
                                break;
                            case 2:
                                ns = NumberSpots.Hundreds;
                                currentNumberTransform = HRObject.HundredsIcon.transform;
                                break;
                        }

                        string path = AnimationUtility.CalculateTransformPath(currentNumberTransform, Avatar.transform);
                        EditorCurveBinding binding =
                            EditorCurveBinding.PPtrCurve(path, typeof(MeshRenderer), "m_Materials.Array.data[0]");
                        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[2];
                        HRMaterialCache hrmc = HRObject.HRMaterialCacheFromLoop(ns);
                        // why does this work?
                        int fixedE = 1;
                        switch (e)
                        {
                            case 0:
                                fixedE = 2;
                                break;
                            case 2:
                                fixedE = 0;
                                break;
                        }

                        int materialIndex = BetterX(x).ToCharArray()[fixedE] - '0';
                        Material targetMaterial = hrmc.GetNumberMaterialFromLoop(materialIndex);
                        if (targetMaterial == null)
                            throw new Exception();
                        // First Keyframe
                        keyFrames[0] = default;
                        keyFrames[0].time = 0f;
                        keyFrames[0].value = targetMaterial;
                        // Second Keyframe
                        keyFrames[1] = default;
                        keyFrames[1].time = 0.01f;
                        keyFrames[1].value = targetMaterial;
                        // Finalize
                        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyFrames);
                    }

                    string id = x.ToString();
                    clip.name = id;
                    string pa = "HRtoVRChat/output/" + friendlyName;
                    if (!Directory.Exists("Assets/" + pa))
                        Directory.CreateDirectory("Assets/" + pa);
                    AssetDatabase.CreateAsset(clip, "Assets/" + pa + "/" + id + ".anim");
                    generatedAnimations.Add(id, clip);
                }
            }
            else
            {
                for (int x = 0; x < 3; x++)
                {
                    Transform currentNumberTransform = HRObjectFromLoop(HRObject, x);
                    string path = AnimationUtility.CalculateTransformPath(currentNumberTransform, Avatar.transform);
                    for (int y = 0; y < 10; y++)
                    {
                        AnimationClip clip = new AnimationClip();
                        clip.frameRate = 1f;
                        EditorCurveBinding binding =
                            EditorCurveBinding.PPtrCurve(path, typeof(MeshRenderer), "m_Materials.Array.data[0]");
                        // Keyframes
                        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[2];
                        Material targetMaterial = HRObject.HRMaterialCacheFromLoop(GetNumbersSpot(x))
                            .GetNumberMaterialFromLoop(y);
                        // First Keyframe
                        keyFrames[0] = default;
                        keyFrames[0].time = 0f;
                        keyFrames[0].value = targetMaterial;
                        // Second Keyframe
                        keyFrames[1] = default;
                        keyFrames[1].time = 0.01f;
                        keyFrames[1].value = targetMaterial;
                        // Finalize and Save
                        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyFrames);
                        string id = LoopToIdentifier(x, y);
                        clip.name = id;
                        string pa = "HRtoVRChat/output/" + friendlyName;
                        if (!Directory.Exists("Assets/" + pa))
                            Directory.CreateDirectory("Assets/" + pa);
                        AssetDatabase.CreateAsset(clip, "Assets/" + pa + "/" + id + ".anim");
                        generatedAnimations.Add(id, clip);
                    }
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Created Animations!");
            // Edit the Animator Controller
            Debug.Log("Applying to AnimatorController...");
            if (useSmallerHR)
            {
                // Only one HR Parameter
                Debug.Log("-- Adding Parameter(s)");
                if (!DoesParameterExist(animator.parameters, AnimatorControllerParameterType.Int, "HR"))
                    animator.AddParameter("HR", AnimatorControllerParameterType.Int);
                // Setup Layers
                Debug.Log("-- Creating Layers");
                AnimatorControllerLayer masterLayer = new AnimatorControllerLayer
                {
                    name = friendlyName + "-master-hr",
                    defaultWeight = 1f,
                    stateMachine = new AnimatorStateMachine
                    {
                        name = friendlyName + "-master-hr",
                        hideFlags = HideFlags.HideInHierarchy
                    }
                };
                // Save Layers to the database
                AddLayerToAnimator(animator, masterLayer);
                // Manage Clips
                Debug.Log("-- Setting up Clips and Transitions");
                AnimatorState masterSM = masterLayer.stateMachine.AddState("empty");
                masterLayer.stateMachine.AddEntryTransition(masterSM);
                int x = 0;
                foreach (KeyValuePair<string, AnimationClip> animationClip in generatedAnimations)
                {
                    string[] split = animationClip.Key.Split('-');
                    NumberSpots ns = GetNumbersSpot(split[0]);
                    AnimatorState _as = masterLayer.stateMachine.AddState(animationClip.Key);
                    // Exit
                    _as.motion = animationClip.Value;
                    _as.writeDefaultValues = false;
                    AnimatorStateTransition exit = _as.AddExitTransition();
                    exit.hasExitTime = false;
                    exit.hasFixedDuration = false;
                    exit.duration = 0f;
                    // AST
                    AnimatorStateTransition ast = masterSM.AddTransition(_as);
                    ast.hasExitTime = false;
                    ast.hasFixedDuration = false;
                    ast.duration = 0f;
                    ast.AddCondition(AnimatorConditionMode.Equals, x, "HR");
                    exit.AddCondition(AnimatorConditionMode.NotEqual, x, "HR");
                    x++;
                }

                if(DoesLayerExist(animator.layers, masterLayer.name) && overwriteLayers)
                    animator.RemoveLayer(GetLayerIndexFromName(animator.layers, masterLayer.name));
            }
            else
            {
                // Only one HR Parameter
                Debug.Log("-- Adding Parameter(s)");
                if (!DoesParameterExist(animator.parameters, AnimatorControllerParameterType.Int, "onesHR"))
                    animator.AddParameter("onesHR", AnimatorControllerParameterType.Int);
                if (!DoesParameterExist(animator.parameters, AnimatorControllerParameterType.Int, "tensHR"))
                    animator.AddParameter("tensHR", AnimatorControllerParameterType.Int);
                if (!DoesParameterExist(animator.parameters, AnimatorControllerParameterType.Int, "hundredsHR"))
                    animator.AddParameter("hundredsHR", AnimatorControllerParameterType.Int);
                // Setup Layers
                Debug.Log("-- Creating Layers");
                AnimatorControllerLayer onesLayer = new AnimatorControllerLayer
                    {
                        name = friendlyName + "-ones-hr",
                        defaultWeight = 1f,
                        stateMachine = new AnimatorStateMachine
                        {
                            hideFlags = HideFlags.HideInHierarchy
                        }
                    },
                    tensLayer = new AnimatorControllerLayer
                    {
                        name = friendlyName + "-tens-hr",
                        defaultWeight = 1f,
                        stateMachine = new AnimatorStateMachine
                        {
                            hideFlags = HideFlags.HideInHierarchy
                        }
                    },
                    hundredsLayer = new AnimatorControllerLayer
                    {
                        name = friendlyName + "-hundreds-hr",
                        defaultWeight = 1f,
                        stateMachine = new AnimatorStateMachine
                        {
                            hideFlags = HideFlags.HideInHierarchy
                        }
                    };
                // Save Layers to the database
                AddLayerToAnimator(animator, onesLayer);
                AddLayerToAnimator(animator, tensLayer);
                AddLayerToAnimator(animator, hundredsLayer);
                // Manage Clips
                Debug.Log("-- Setting up Clips and Transitions");
                AnimatorState onesSM = onesLayer.stateMachine.AddState("empty");
                onesLayer.stateMachine.AddEntryTransition(onesSM);
                AnimatorState tensSM = tensLayer.stateMachine.AddState("empty");
                tensLayer.stateMachine.AddEntryTransition(tensSM);
                AnimatorState hundredsSM = hundredsLayer.stateMachine.AddState("empty");
                hundredsLayer.stateMachine.AddEntryTransition(hundredsSM);
                foreach (KeyValuePair<string, AnimationClip> animationClip in generatedAnimations)
                {
                    string[] split = animationClip.Key.Split('-');
                    NumberSpots ns = GetNumbersSpot(split[0]);
                    // This will not be null
                    AnimatorState sm = null;
                    AnimatorState _as = null;
                    switch (ns)
                    {
                        case NumberSpots.Ones:
                            sm = onesSM;
                            _as = onesLayer.stateMachine.AddState(animationClip.Key);
                            break;
                        case NumberSpots.Tens:
                            sm = tensSM;
                            _as = tensLayer.stateMachine.AddState(animationClip.Key);
                            break;
                        case NumberSpots.Hundreds:
                            sm = hundredsSM;
                            _as = hundredsLayer.stateMachine.AddState(animationClip.Key);
                            break;
                    }

                    _as.motion = animationClip.Value;
                    _as.writeDefaultValues = false;
                    AnimatorStateTransition exit = _as.AddExitTransition();
                    exit.hasExitTime = false;
                    exit.hasFixedDuration = false;
                    exit.duration = 0f;
                    AnimatorStateTransition ast = sm.AddTransition(_as);
                    ast.hasExitTime = false;
                    ast.hasFixedDuration = false;
                    ast.duration = 0f;
                    ast.AddCondition(AnimatorConditionMode.Equals, WordToNumber(split[1]), split[0] + "HR");
                    exit.AddCondition(AnimatorConditionMode.NotEqual, WordToNumber(split[1]), split[0] + "HR");
                }

                if(DoesLayerExist(animator.layers, onesLayer.name) && overwriteLayers)
                    animator.RemoveLayer(GetLayerIndexFromName(animator.layers, onesLayer.name));
                if(DoesLayerExist(animator.layers, tensLayer.name) && overwriteLayers)
                    animator.RemoveLayer(GetLayerIndexFromName(animator.layers, tensLayer.name));
                if(DoesLayerExist(animator.layers, hundredsLayer.name) && overwriteLayers)
                    animator.RemoveLayer(GetLayerIndexFromName(animator.layers, hundredsLayer.name));
            }

            if (deleteComponentOnDone)
                Object.DestroyImmediate(HRObject);
            Debug.Log("Done!");
            EditorUtility.DisplayDialog("HRtoVRChat_SDK", "Completed Operation!", "OK");
        }
    }
    
    public class AnimatorWindow : EditorWindow
    {
        private static AnimatorWindow _instance;
        private static void NewGUILine() => GUILayout.Label("", EditorStyles.largeLabel);
        private static Vector2 buttonsScrollPos;

        private TargetHRObject SelectedHRTarget;

        private string friendlyName = "HR";
        private bool useSmallerHR = true;
        private bool overwriteLayers;
        private bool deleteComponentOnDone;

        [MenuItem("Window/HRtoVRChat_SDK")]
        private static void ShowWindow()
        {
            _instance = GetWindow<AnimatorWindow>();
            _instance.titleContent = new GUIContent("HRtoVRChat Editor SDK");
        }

        private void Draw()
        {
            // BEGIN HEADER
            GUILayout.Label("HRtoVRChat Editor SDK", EditorStyles.centeredGreyMiniLabel);
            GUILayout.Label("HRtoVRChat by 200Tigersbloxed", EditorStyles.centeredGreyMiniLabel);
            NewGUILine();
            if (GUILayout.Button("Check us out on GitHub!"))
                Process.Start("https://github.com/200Tigersbloxed/HRtoVRChat_OSC");
            // END HEADER
            NewGUILine();
            // BEGIN BODY
            GUILayout.Label("Discovered TargetHRObjects", EditorStyles.centeredGreyMiniLabel);
            NewGUILine();
            List<TargetHRObject> targets = FindObjectsOfType<TargetHRObject>().ToList();
            if(targets.Count <= 0)
                GUILayout.Label("No TargetHRObject Components found in Scene!");
            else
            {
                buttonsScrollPos = GUILayout.BeginScrollView(buttonsScrollPos);
                foreach (TargetHRObject targetHrObject in targets)
                {
                    if (GUILayout.Button(targetHrObject.gameObject.name))
                        SelectedHRTarget = targetHrObject;
                }
                GUILayout.EndScrollView();
            }
            NewGUILine();
            if (SelectedHRTarget != null)
            {
                GUILayout.Label("SelectedHRTarget: " + SelectedHRTarget.gameObject.name);
                NewGUILine();
                if (GUILayout.Button("Create Animations!"))
                {
                    if (SelectedHRTarget.AvatarRoot != null && SelectedHRTarget.FXController != null)
                    {
                        try
                        {
                            AnimatorCreator.BeginProcess(SelectedHRTarget.AvatarRoot.gameObject, SelectedHRTarget,
                                SelectedHRTarget.FXController, useSmallerHR: useSmallerHR,
                                deleteComponentOnDone: deleteComponentOnDone, overwriteLayers: overwriteLayers,
                                friendlyName: friendlyName);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                            EditorUtility.DisplayDialog("HRtoVRChat_SDK",
                                "Failed to complete operation! See the Console for more information", "OK");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("HRtoVRChat_SDK",
                            "SelectedHRTarget cannot be null!\n" +
                            "If it is not null, then please make sure that you have set an Avatar " +
                            "and an FX Layer on the component", "OK");
                    }
                }
                NewGUILine();
            }
            GUILayout.Label("Options", EditorStyles.centeredGreyMiniLabel);
            NewGUILine();
            friendlyName = EditorGUILayout.TextField("friendlyName", friendlyName);
            GUILayout.Label("Sets a name for the HRObject when creating");
            GUILayout.Label("This should be changed for every creation on the same avatar", EditorStyles.miniLabel);
            useSmallerHR = GUILayout.Toggle(useSmallerHR, "Use one HR parameter");
            GUILayout.Label("Uses only one HR parameter instead of the conventional three parameters");
            deleteComponentOnDone = GUILayout.Toggle(deleteComponentOnDone, "Delete the TargetHRObject Component");
            GUILayout.Label("Deletes the TargetHRObject Component when finished");
            overwriteLayers = GUILayout.Toggle(overwriteLayers, "Overwrite existing layers");
            GUILayout.Label("Deletes layers if they already exist");
            // END BODY
        }

        private void OnGUI()
        {
            if (_instance != null)
            {
                Draw();
            }
        }
    }
}
#endif