namespace HRtoVRChat_OSC
{
    public static class ParamsManager
    {
        public static List<HRParameter> Parameters = new List<HRParameter>();

        public static void InitParams()
        {
            Parameters.Add(new IntParameter(hro => hro.ones, ConfigManager.LoadedConfig.ParameterNames["onesHR"]));
            Parameters.Add(new IntParameter(hro => hro.tens, ConfigManager.LoadedConfig.ParameterNames["tensHR"]));
            Parameters.Add(new IntParameter(hro => hro.hundreds, ConfigManager.LoadedConfig.ParameterNames["hundredsHR"]));
            Parameters.Add(new IntParameter((hro) =>
            {
                string HRstring = $"{hro.hundreds}{hro.tens}{hro.ones}";
                int HR = 0;
                try
                {
                    HR = Convert.ToInt32(HRstring);
                }
                catch (Exception)
                {
                }

                if (HR > 255)
                    HR = 255;
                if (HR < 0)
                    HR = 0;
                return HR;
            }, ConfigManager.LoadedConfig.ParameterNames["HR"]));
            Parameters.Add(new FloatParameter((hro) =>
            {
                float targetFloat = 0f;
                float maxhr = (float) ConfigManager.LoadedConfig.MaxHR;
                float minhr = (float) ConfigManager.LoadedConfig.MinHR;
                float HR = (float) hro.HR;
                if (HR > maxhr)
                    targetFloat = 1;
                else if (HR < minhr)
                    targetFloat = 0;
                else
                    targetFloat = (HR - minhr) / (maxhr - minhr);
                return targetFloat;
            }, ConfigManager.LoadedConfig.ParameterNames["HRPercent"]));
            Parameters.Add(new BoolParameter(hro => hro.isActive, ConfigManager.LoadedConfig.ParameterNames["isHRActive"]));
            Parameters.Add(new BoolParameter(hro => hro.isConnected, ConfigManager.LoadedConfig.ParameterNames["isHRConnected"]));
            Parameters.Add(new BoolParameter(BoolCheckType.HeartBeat, ConfigManager.LoadedConfig.ParameterNames["isHRBeat"]));
        }

        public static void ResetParams()
        {
            int paramcount = Parameters.Count;
            foreach (HRParameter hrParameter in Parameters)
                hrParameter.UpdateParameter(true);
            Parameters.Clear();
            LogHelper.Debug($"Cleared {paramcount} parameters!");
        }

        public class IntParameter : HRParameter
        {
            public IntParameter(Func<HROutput, int> getVal, string parameterName)
            {
                ParameterName = parameterName;
                LogHelper.Debug($"IntParameter with ParameterName: {parameterName}, has been created!");
                Program.OnHRValuesUpdated += (ones, tens, hundreds, HR, isConnected, isActive) =>
                {
                    HROutput hro = new HROutput()
                    {
                        ones = ones,
                        tens = tens,
                        hundreds = hundreds,
                        isConnected = isConnected
                    };
                    int valueToSet = getVal.Invoke(hro);
                    ParamValue = valueToSet.ToString();
                    UpdateParameter();
                };
            }
            
            public string ParameterName { get; set; }
            public string ParamValue { get; set; }
            public string DefaultValue => "0";
            public void UpdateParameter(bool fromReset = false)
            {
                string val = ParamValue;
                if (fromReset)
                    val = DefaultValue;
                OSCManager.SendMessage("/avatar/parameters/" + ParameterName, Convert.ToInt32(val));
            }
        }

        public class BoolParameter : HRParameter
        {
            public BoolParameter(Func<HROutput, bool> getVal, string parameterName)
            {
                ParameterName = parameterName;
                LogHelper.Debug($"BoolParameter with ParameterName: {parameterName}, has been created!");
                Program.OnHRValuesUpdated += (ones, tens, hundreds, HR, isConnected, isActive) =>
                {
                    HROutput hro = new HROutput()
                    {
                        ones = ones,
                        tens = tens,
                        hundreds = hundreds,
                        HR = HR,
                        isConnected = isConnected,
                        isActive = isActive
                    };
                    bool valueToSet = getVal.Invoke(hro);
                    ParamValue = valueToSet.ToString();
                    UpdateParameter();
                };
            }

            public BoolParameter(BoolCheckType bct, string parameterName)
            {
                ParameterName = parameterName;
                LogHelper.Debug($"BoolParameter with ParameterName: {parameterName} and BoolCheckType of: {bct}, has been created!");
                Program.OnHeartBeatUpdate += (isHeartBeat, shouldRestart) =>
                {
                    switch (bct)
                    {
                        case BoolCheckType.HeartBeat:
                            ParamValue = isHeartBeat.ToString();
                            UpdateParameter();
                            break;
                    }
                };
            }
            
            public string ParameterName { get; set; }
            public string ParamValue { get; set; }
            public string DefaultValue => "false";
            public void UpdateParameter(bool fromReset = false)
            {
                string val = ParamValue;
                if (fromReset)
                    val = DefaultValue;
                OSCManager.SendMessage("/avatar/parameters/" + ParameterName, Convert.ToBoolean(val));
            }
        }

        public class FloatParameter : HRParameter
        {
            public FloatParameter(Func<HROutput, float> getVal, string parameterName)
            {
                ParameterName = parameterName;
                LogHelper.Debug($"FloatParameter with ParameterName: {parameterName} has been created!");
                Program.OnHRValuesUpdated += (ones, tens, hundreds, HR, isConnected, isActive) =>
                {
                    HROutput hro = new HROutput()
                    {
                        ones = ones,
                        tens = tens,
                        hundreds = hundreds,
                        HR = HR,
                        isConnected = isConnected,
                        isActive = isActive
                    };
                    float targetValue = getVal.Invoke(hro);
                    ParamValue = targetValue.ToString();
                    UpdateParameter();
                };
            }

            public string ParameterName { get; set; }
            public string ParamValue { get; set; }
            public string DefaultValue => "0";
            public void UpdateParameter(bool fromReset = false)
            {
                string val = ParamValue;
                if (fromReset)
                    val = DefaultValue;
                OSCManager.SendMessage("/avatar/parameters/" + ParameterName, (float) Convert.ToDouble(val));
            }
        }

        public class HROutput
        {
            public int ones;
            public int tens;
            public int hundreds;
            public int HR;
            public bool isConnected;
            public bool isActive;
        }

        public interface HRParameter
        {
            public string ParameterName { get; set; }
            public string ParamValue { get; set; }
            public string DefaultValue { get; }
            public void UpdateParameter(bool fromReset = false);
        }

        public enum BoolCheckType
        {
            HeartBeat
        }
    }
}