#if UNITY_EDITOR
#if VRC_SDK_VRCSDK3
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

public static class VRCSDKAnimations
{
    /// <summary>
    /// Adds parameters to an AvatarDescriptor
    /// </summary>
    /// <param name="avatarDescriptor">The target Avatar Descriptor</param>
    /// <param name="parameter">The parameter to add</param>
    public static bool AddVRCParameter(VRCAvatarDescriptor avatarDescriptor, VRCExpressionParameters.Parameter parameter)
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
}
#endif
#endif