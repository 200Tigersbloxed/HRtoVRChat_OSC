#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
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

        /// <summary>
        /// Adds parameters to an AvatarDescriptor
        /// </summary>
        /// <param name="avatarDescriptor">The target Avatar Descriptor</param>
        /// <param name="parameter">The parameter to add</param>
        private static bool AddVRCParameter(VRCAvatarDescriptor avatarDescriptor, VRCExpressionParameters.Parameter parameter)
        {
            // Make sure Parameters aren't null
            if (avatarDescriptor.expressionParameters == null)
                return false;
            // Instantiate and Save to Database
            VRCExpressionParameters newParameters = avatarDescriptor.expressionParameters;
            string assetPath = AssetDatabase.GetAssetPath(avatarDescriptor.expressionParameters);
            if (assetPath != String.Empty)
            {
                AssetDatabase.RemoveObjectFromAsset(avatarDescriptor.expressionParameters);
                AssetDatabase.CreateAsset(newParameters, assetPath);
                avatarDescriptor.expressionParameters = newParameters;
            }
            // Make sure the parameter doesn't already exist
            VRCExpressionParameters.Parameter foundParameter = newParameters.FindParameter(parameter.name);
            if (foundParameter == null || foundParameter.valueType != parameter.valueType)
            {
                // Add the parameter
                List<VRCExpressionParameters.Parameter> betterParametersBecauseItsAListInstead =
                    newParameters.parameters.ToList();
                betterParametersBecauseItsAListInstead.Add(parameter);
                newParameters.parameters = betterParametersBecauseItsAListInstead.ToArray();
            }
            return true;
        }

        public static string HR_paramname = "HR";
        public static string onesHR_paramname = "onesHR";
        public static string tensHR_paramname = "tensHR";
        public static string hundredsHR_paramname = "hundredsHR";
        
        public static void BeginProcess(GameObject Avatar, TargetHRObject HRObject, AnimatorController animator,
            string friendlyName = "HR", bool useSmallerHR = false, bool deleteComponentOnDone = false,
            bool overwriteLayers = false, bool writeDefaults = false, bool writeToParameters = false)

        {
            VRCAvatarDescriptor descriptor = Avatar.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null)
                throw new Exception("Failed to get Avatar Descriptor!");
            Debug.Log("Creating Animations...");
            Dictionary<string, AnimationClip> generatedAnimations = new Dictionary<string, AnimationClip>();
            // Create the Animation Clips
            if (HRObject.Backend == HRObjectBackend.Universal)
            {
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
            }
            else if (HRObject.Backend == HRObjectBackend.Shaders)
            {
                for (int x = 0; x < 256; x++)
                {
                    AnimationClip clip = new AnimationClip();
                    clip.frameRate = 1f;
                    string path = AnimationUtility.CalculateTransformPath(HRObject.NumsIcon.transform, Avatar.transform);
                    EditorCurveBinding binding =
                        EditorCurveBinding.PPtrCurve(path, typeof(MeshRenderer), "material._Value");
                    AnimationCurve ac = new AnimationCurve();
                    // First Keyframe
                    ac.AddKey(0f, x);
                    // Second Keyframe
                    ac.AddKey(0.01f, x);
                    // Finalize
                    AnimationUtility.SetEditorCurve(clip, binding, ac);
                    string id = x.ToString();
                    clip.name = id;
                    string pa = "HRtoVRChat/output/" + friendlyName;
                    if (!Directory.Exists("Assets/" + pa))
                        Directory.CreateDirectory("Assets/" + pa);
                    AssetDatabase.CreateAsset(clip, "Assets/" + pa + "/" + id + ".anim");
                    generatedAnimations.Add(id, clip);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Created Animations!");
            // Edit the Animator Controller
            Debug.Log("Applying to AnimatorController...");
            if (useSmallerHR || HRObject.Backend == HRObjectBackend.Shaders)
            {
                // Only one HR Parameter
                Debug.Log("-- Adding Parameter(s)");
                if (!DoesParameterExist(animator.parameters, AnimatorControllerParameterType.Int, HR_paramname))
                    animator.AddParameter(HR_paramname, AnimatorControllerParameterType.Int);
                // Setup Layers
                if(overwriteLayers)
                    Debug.Log("Removing Duplicate Layers");
                if(DoesLayerExist(animator.layers, friendlyName + "-master-hr") && overwriteLayers)
                    animator.RemoveLayer(GetLayerIndexFromName(animator.layers, friendlyName + "-master-hr"));
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
                    _as.writeDefaultValues = writeDefaults;
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
                // Apply Parameters
                if (writeToParameters)
                {
                    Debug.Log("Adding Parameters");
                    AddVRCParameter(descriptor, new VRCExpressionParameters.Parameter
                    {
                        name = HR_paramname,
                        defaultValue = 0,
                        saved = false,
                        valueType = VRCExpressionParameters.ValueType.Int
                    });
                }
            }
            else
            {
                // Only one HR Parameter
                Debug.Log("-- Adding Parameter(s)");
                if (!DoesParameterExist(animator.parameters, AnimatorControllerParameterType.Int, onesHR_paramname))
                    animator.AddParameter(onesHR_paramname, AnimatorControllerParameterType.Int);
                if (!DoesParameterExist(animator.parameters, AnimatorControllerParameterType.Int, tensHR_paramname))
                    animator.AddParameter(tensHR_paramname, AnimatorControllerParameterType.Int);
                if (!DoesParameterExist(animator.parameters, AnimatorControllerParameterType.Int, hundredsHR_paramname))
                    animator.AddParameter(hundredsHR_paramname, AnimatorControllerParameterType.Int);
                // Setup Layers
                if(overwriteLayers)
                    Debug.Log("Removing Duplicate Layers");
                if(DoesLayerExist(animator.layers, friendlyName + "-ones-hr") && overwriteLayers)
                    animator.RemoveLayer(GetLayerIndexFromName(animator.layers, friendlyName + "-ones-hr"));
                if(DoesLayerExist(animator.layers, friendlyName + "-tens-hr") && overwriteLayers)
                    animator.RemoveLayer(GetLayerIndexFromName(animator.layers, friendlyName + "-tens-hr"));
                if(DoesLayerExist(animator.layers, friendlyName + "-hundreds-hr") && overwriteLayers)
                    animator.RemoveLayer(GetLayerIndexFromName(animator.layers, friendlyName + "-hundreds-hr"));
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
                    _as.writeDefaultValues = writeDefaults;
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
                // Apply Parameters
                if (writeToParameters)
                {
                    Debug.Log("Adding Parameters");
                    AddVRCParameter(descriptor, new VRCExpressionParameters.Parameter
                    {
                        name = onesHR_paramname,
                        defaultValue = 0,
                        saved = false,
                        valueType = VRCExpressionParameters.ValueType.Int
                    });
                    AddVRCParameter(descriptor, new VRCExpressionParameters.Parameter
                    {
                        name = tensHR_paramname,
                        defaultValue = 0,
                        saved = false,
                        valueType = VRCExpressionParameters.ValueType.Int
                    });
                    AddVRCParameter(descriptor, new VRCExpressionParameters.Parameter
                    {
                        name = hundredsHR_paramname,
                        defaultValue = 0,
                        saved = false,
                        valueType = VRCExpressionParameters.ValueType.Int
                    });
                }
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
        private static Vector2 optionsScrollPos;

        private TargetHRObject SelectedHRTarget;

        private string friendlyName = "HR";
        private bool useSmallerHR = true;
        private bool overwriteLayers;
        private bool deleteComponentOnDone;
        private bool writeDefaults;
        private bool writeToParameters = true;

        [MenuItem("Window/HRtoVRChat_SDK")]
        private static void ShowWindow()
        {
            _instance = GetWindow<AnimatorWindow>();
            _instance.titleContent = new GUIContent("HRtoVRChat Editor SDK");
        }
        
        private void CreateBuildButton()
        {
            if (GUILayout.Button("Create Animations!"))
            {
                if (SelectedHRTarget.AvatarRoot != null && SelectedHRTarget.FXController != null)
                {
                    try
                    {
                        AnimatorCreator.BeginProcess(SelectedHRTarget.AvatarRoot.gameObject, SelectedHRTarget,
                            SelectedHRTarget.FXController, useSmallerHR: useSmallerHR,
                            deleteComponentOnDone: deleteComponentOnDone, overwriteLayers: overwriteLayers,
                            friendlyName: friendlyName, writeDefaults: writeDefaults,
                            writeToParameters: writeToParameters);
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
                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.Android:
                        if (SelectedHRTarget.Backend == HRObjectBackend.Universal)
                            CreateBuildButton();
                        else
                            EditorGUILayout.HelpBox("Universal Backend required for building on Quest!",
                                MessageType.Error);
                        break;
                     case BuildTarget.StandaloneWindows:
                         CreateBuildButton();
                         break;
                     case BuildTarget.StandaloneWindows64:
                         CreateBuildButton();
                         break;
                }
                NewGUILine();
            }
            GUILayout.Label("Options", EditorStyles.centeredGreyMiniLabel);
            NewGUILine();
            optionsScrollPos = GUILayout.BeginScrollView(optionsScrollPos);
            friendlyName = EditorGUILayout.TextField("friendlyName", friendlyName);
            GUILayout.Label("Sets a name for the HRObject when creating");
            GUILayout.Label("This should be changed for every creation in the same Unity Project", EditorStyles.miniLabel);
            NewGUILine();
            if (SelectedHRTarget != null)
            {
                using (new EditorGUI.DisabledScope(SelectedHRTarget.Backend == HRObjectBackend.Shaders))
                    useSmallerHR = GUILayout.Toggle(useSmallerHR, "Use one HR parameter");
                if (SelectedHRTarget.Backend == HRObjectBackend.Shaders)
                {
                    EditorGUILayout.HelpBox("useSmallerHR is required for a Shader Backend", MessageType.Info);
                    useSmallerHR = true;
                }
            }
            else
                GUILayout.Label("Please select an HR target to edit the setting: useSmallerHR");
            GUILayout.Label("Uses only one HR parameter instead of the conventional three parameters");
            NewGUILine();
            writeToParameters = GUILayout.Toggle(writeToParameters, "Write to Expression Parameters");
            GUILayout.Label("Will write all expression parameters to the AvatarDescriptor's\nexpression parameters");
            NewGUILine();
            deleteComponentOnDone = GUILayout.Toggle(deleteComponentOnDone, "Delete the TargetHRObject Component");
            GUILayout.Label("Deletes the TargetHRObject Component when finished");
            NewGUILine();
            overwriteLayers = GUILayout.Toggle(overwriteLayers, "Overwrite existing layers");
            GUILayout.Label("Deletes layers if they already exist");
            NewGUILine();
            writeDefaults = GUILayout.Toggle(writeDefaults, "Write Defaults");
            GUILayout.Label("Whether or not the AnimatorStates writes back the default values\nfor properties that are not animated by its Motion.");
            GUILayout.Label("This is recommended off by VRChat", EditorStyles.boldLabel);
            NewGUILine();
            AnimatorCreator.HR_paramname = EditorGUILayout.TextField("HR Parameter Name", AnimatorCreator.HR_paramname);
            GUILayout.Label("Parameter name to be used for the parameter HR");
            NewGUILine();
            AnimatorCreator.onesHR_paramname =
                EditorGUILayout.TextField("onesHR Parameter Name", AnimatorCreator.onesHR_paramname);
            GUILayout.Label("Parameter name to be used for the parameter onesHR");
            NewGUILine();
            AnimatorCreator.tensHR_paramname =
                EditorGUILayout.TextField("tensHR Parameter Name", AnimatorCreator.tensHR_paramname);
            GUILayout.Label("Parameter name to be used for the parameter tensHR");
            NewGUILine();
            AnimatorCreator.hundredsHR_paramname =
                EditorGUILayout.TextField("hundredsHR Parameter Name", AnimatorCreator.hundredsHR_paramname);
            GUILayout.Label("Parameter name to be used for the parameter hundredsHR");
            GUILayout.EndScrollView();
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