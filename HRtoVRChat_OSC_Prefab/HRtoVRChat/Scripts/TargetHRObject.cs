#if UNITY_EDITOR
using System;
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
    }
}
#endif