#if UNITY_EDITOR
#if CVR_CCK_EXISTS
using System.Collections.Generic;
using ABI.CCK.Components;
using ABI.CCK.Scripts;

public static class CVRCCKAnimations
{
    private static (bool, CVRAdvancedSettingsEntry) DoesAdvancedSettingExist(string name, ref List<CVRAdvancedSettingsEntry> entries)
    {
        bool ret = false;
        CVRAdvancedSettingsEntry ase = null;
        foreach (CVRAdvancedSettingsEntry cvrAdvancedSettingsEntry in entries)
        {
            if (cvrAdvancedSettingsEntry.name == name)
            {
                ret = true;
                ase = cvrAdvancedSettingsEntry;
            }
        }
        return (ret, ase);
    }
    
    public static void ApplySmallerAnimationsToCVRAvatar(CVRAvatar avatar, string hrName = "HR")
    {
        avatar.avatarUsesAdvancedSettings = true;
        (bool, CVRAdvancedSettingsEntry) dase = DoesAdvancedSettingExist(hrName, ref avatar.avatarSettings.settings);
        if (dase.Item1)
            return;
        
        List<CVRAdvancedSettingsDropDownEntry> dde = new List<CVRAdvancedSettingsDropDownEntry>();
        for (int i = 0; i < 256; i++)
        {
            CVRAdvancedSettingsDropDownEntry dropDownEntry = new CVRAdvancedSettingsDropDownEntry();
            dropDownEntry.name = i.ToString();
            dde.Add(dropDownEntry);
        }

        CVRAdvancedSettingsEntry hrase = new CVRAdvancedSettingsEntry
        {
            machineName = hrName,
            name = hrName,
            type = CVRAdvancedSettingsEntry.SettingsType.GameObjectDropdown,
            setting = new CVRAdvancesAvatarSettingGameObjectDropdown
            {
                defaultValue = 0,
                usedType = CVRAdvancesAvatarSettingBase.ParameterType.GenerateInt,
                options = dde
            }
        };
        
        avatar.avatarSettings.settings.Add(hrase);
    }
}
#endif
#endif