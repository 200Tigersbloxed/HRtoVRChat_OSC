/*
 * This file is now archived
 * https://github.com/200Tigersbloxed/UnityMods/issues/6
 */

/*
using GenericHRLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnhollowerBaseLib;

namespace HRtoVRChat.HRManagers
{
    public class WinBLEGATTManager : HRManager
    {
        private Assembly generichr_assembly = null;
        private Type generichr_type = null;
        private static object global_generichr_instance = null;

        private Thread _thread = null;

        public static int HR { get; private set; } = 0;
        public static bool isConnected { get; private set; } = false;

        // https://stackoverflow.com/a/194223/12968919
        private static string ProgramFilesx86()
        {
            if( 8 == IntPtr.Size 
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }
        
        private void load_generichr_resource()
        {
            // load UniversalApiContract
            string references = Path.Combine(new string[] {ProgramFilesx86(), "Windows Kits", "10", "References"});
            bool didLoad = false;
            foreach (string directory in Directory.GetDirectories(references))
            {
                LogHelper.Debug("g", directory);
                string file = Path.Combine(new string[]
                {
                    directory, "Windows.Foundation.UniversalApiContract", "10.0.0.0",
                    "Windows.Foundation.UniversalApiContract.winmd"
                });
                LogHelper.Debug("g", file);
                if (!string.IsNullOrEmpty(file) && File.Exists(file))
                {
                    LogHelper.Debug("WinBLEGATTManager", $"Found UniversalApiContract at {file}");
                    Assembly.LoadFile(file);
                    didLoad = true;
                }
            }
            if (!didLoad)
            {
                LogHelper.Error("WinBLEGATTManager", "Failed to find UniversalApiContract");
                return;
            }
            // load GenericHRLib
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("HRtoVRChat.Libs.GenericHRLib.dll"))
            {
                if (stream != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        generichr_assembly = Assembly.Load(ms.ToArray());
                        LogHelper.Debug("WinBLEGATTManager", "Loaded GenericHRLib Assembly!");
                    }
                }
                else
                    LogHelper.Error("WinBLEGATTManager", "stream is null! Does the ManifestResource exist?");
            }
        }

        private object create_generichr_instance()
        {
            if(generichr_assembly != null)
            {
                generichr_type = generichr_assembly.GetType("GenericHRLib.GenericHRDevice");
                object newInstance = Activator.CreateInstance(generichr_type);
                global_generichr_instance = newInstance;
                LogHelper.Debug("WinBLEGATTManager", "GenericHRDevice created!");
                return newInstance;
            }
            LogHelper.Error("WinBLEGATTManager", "generichr_assembly is null!");
            global_generichr_instance = null;
            return null;
        }

        private bool generichrdevice_findandconnect(object instance)
        {
            MethodInfo method = generichr_type.GetMethod("FindAndConnect");
            if (method != null)
            {
                try
                {
                    method.Invoke(instance, null);
                    return generichrdevice_getboolproperty(generichrdevice_boolproperties.IsAlive, instance);
                }
                catch (Exception e)
                {
                    LogHelper.Error("WinBLEGATTManager", $"Failed to FindAndConnect! Exception: {e}");
                    return false;
                }
            }
            LogHelper.Error("WinBLEGATTManager", "Failed to find FindAndConnect!");
            return false;
        }

        private enum generichrdevice_boolproperties
        {
            NeedsUpdate,
            IsAlive
        }
        
        private bool generichrdevice_getboolproperty(generichrdevice_boolproperties boolproperties, object instance)
        {
            switch (boolproperties)
            {
                case generichrdevice_boolproperties.IsAlive:
                    PropertyInfo isalive_property = generichr_type.GetProperty("IsAlive");
                    if (isalive_property != null)
                    {
                        bool isalive_value =
                            (bool) Convert.ChangeType(isalive_property.GetValue(instance), typeof(bool));
                        return isalive_value;
                    }
                    LogHelper.Error("WinBLEGATTManager", "Failed to get IsAlive Property!");
                    break;
                case generichrdevice_boolproperties.NeedsUpdate:
                    PropertyInfo needsupdate_property = generichr_type.GetProperty("NeedsUpdate");
                    if (needsupdate_property != null)
                    {
                        bool needsupdate_value =
                            (bool) Convert.ChangeType(needsupdate_property.GetValue(instance), typeof(bool));
                        return needsupdate_value;
                    }
                    LogHelper.Error("WinBLEGATTManager", "Failed to get NeedsUpdate Property!");
                    break;
            }
            LogHelper.Warn("WinBLEGATTManager", "Unknown issue in generichrdevice_getboolproperty");
            return false;
        }

        private object generichrdevice_getheartratereading(object instance)
        {
            MethodInfo method = generichr_type.GetMethod("GetHeartRateReading");
            if (method != null)
            {
                object result;
                result = method.Invoke(instance, null);
                return result;
            }
            LogHelper.Error("WinBLEGATTManager", "Failed to GetHeartRateReading!");
            return null;
        }

        private int generichrdevice_getheartrate(object instance)
        {
            PropertyInfo property = generichr_type.GetProperty("HeartRate");
            if (property != null)
            {
                object result = property.GetValue(instance);
                return (int) Convert.ChangeType(result, typeof(int));
            }
            LogHelper.Error("WinBLEGATTManager", "Failed to find HeartRate!");
            return -1;
        }

        private void generichrdevice_dispose(object instance)
        {
            MethodInfo disposeMethod = generichr_type.GetMethod("Dispose");
            if(disposeMethod != null)
                disposeMethod.Invoke(instance, null);
            else
                LogHelper.Error("WinBLEGATTManager", "Failed to Dispose!");
        }
        
        private void VerifyClosedDevice()
        {
            if (global_generichr_instance != null)
                generichrdevice_dispose(global_generichr_instance);
            global_generichr_instance = null;
        }

        private void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
                _thread = null;
            }
        }

        public bool Init(string empty)
        {
            object device = null;
            bool didConnect = false;
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                if (generichr_assembly == null)
                    load_generichr_resource();
                if (isConnected)
                    VerifyClosedDevice();
                if (generichr_assembly != null)
                {
                    device = create_generichr_instance();
                    didConnect = generichrdevice_findandconnect(device);
                }
                else
                    LogHelper.Error("WinBLEGATTManager", "GenericHR Assembly is null!");
                while (isConnected)
                {
                    // Get Values and Properties
                    isConnected = generichrdevice_getboolproperty(generichrdevice_boolproperties.IsAlive, device);
                    HR = generichrdevice_getheartrate(device);
                    Thread.Sleep(10);
                }
            });
            _thread.Start();
            return didConnect;
        }

        public bool IsOpen() => isConnected;
        public bool IsActive() => isConnected;
        public int GetHR() => HR;

        public void Stop()
        {
            VerifyClosedDevice();
            VerifyClosedThread();
        }
    }

    public class WinBLEGATTManager : HRManager
    {
        private Thread _thread = null;
        private GenericHRDevice ghrd = null;

        public int HR { get; private set; } = 0;
        public bool isDeviceConnected { get; private set; } = false;

        private bool LoadedAssemblies = false;
        
        // https://stackoverflow.com/a/194223/12968919
        private static string ProgramFilesx86()
        {
            if( 8 == IntPtr.Size 
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }
        
        public void LoadWindowsContracts()
        {
            string programFilesPath = String.Empty;
            string nugetPath = String.Empty;
            if(Directory.Exists(Path.Combine(new string[] {ProgramFilesx86(), "Windows Kits", "10", "References"})))
                programFilesPath = Path.Combine(new string[] {ProgramFilesx86(), "Windows Kits", "10", "References"});
            if (Directory.Exists(Environment.ExpandEnvironmentVariables("%USERPROFILE%\\.nuget\\packages\\microsoft.windows.sdk.contracts")))
                nugetPath = Environment.ExpandEnvironmentVariables("%USERPROFILE%\\.nuget\\packages\\microsoft.windows.sdk.contracts");

            Assembly.LoadFile(Path.Combine(new[]
                {ProgramFilesx86(), "Windows Kits", "10", "UnionMetadata", "10.0.19041.0", "Windows.winmd"}));
            
            if (!string.IsNullOrEmpty(programFilesPath) && !LoadedAssemblies)
            {
                foreach (string sdkVersion in Directory.GetDirectories(programFilesPath))
                {
                    if (File.Exists($"{sdkVersion}\\Windows.Foundation.FoundationContract\\4.0.0.0\\Windows.Foundation.FoundationContract.winmd"))
                        Assembly.LoadFrom($"{sdkVersion}\\Windows.Foundation.FoundationContract\\4.0.0.0\\Windows.Foundation.FoundationContract.winmd");
                    else
                        LogHelper.Error("WinBLEGATTManager", "Failed to Find Windows.Foundation.FoundationContract Assembly! Prepare to crash!");

                    if (File.Exists($"{sdkVersion}\\Windows.Foundation.UniversalApiContract\\10.0.0.0\\Windows.Foundation.UniversalApiContract.winmd"))
                        Assembly.LoadFrom($"{sdkVersion}\\Windows.Foundation.UniversalApiContract\\10.0.0.0\\Windows.Foundation.UniversalApiContract.winmd");
                    else
                        LogHelper.Error("WinBLEGATTManager", "Failed to Find Windows.Foundation.UniversalApiContract Assembly! Prepare to crash!");
                }
            }
            else if (!string.IsNullOrEmpty(nugetPath) && !LoadedAssemblies)
            {
                foreach (string sdkVersion in Directory.GetDirectories(nugetPath))
                {
                    if (File.Exists($"{sdkVersion}\\ref\\netstandard2.0\\Windows.Foundation.FoundationContract.winmd"))
                        Assembly.LoadFrom($"{sdkVersion}\\ref\\netstandard2.0\\Windows.Foundation.FoundationContract.winmd");
                    else
                        LogHelper.Error("WinBLEGATTManager", "Failed to Find Windows.Foundation.FoundationContract Assembly! Prepare to crash!");

                    if (File.Exists($"{sdkVersion}\\ref\\netstandard2.0\\Windows.Foundation.UniversalApiContract.winmd"))
                        Assembly.LoadFrom($"{sdkVersion}\\ref\\netstandard2.0\\Windows.Foundation.UniversalApiContract.winmd");
                    else
                        LogHelper.Error("WinBLEGATTManager", "Failed to Find Windows.Foundation.UniversalApiContract Assembly! Prepare to crash!");
                }
            }
            else if (LoadedAssemblies)
                LogHelper.Debug("WinBLEGATTManager", "Already loaded Contract Assemblies, not loading again.");
            else
                LogHelper.Error("WinBLEGATTManager", "Failed to find contract locations, do you have the Windows SDK?");
        }

        private void LoadFoundation()
        {
            if (Directory.Exists("C:\\Windows\\System32\\WinMetadata"))
                if (File.Exists("C:\\Windows\\System32\\WinMetadata\\Windows.Foundation.winmd"))
                    Assembly.LoadFrom("C:\\Windows\\System32\\WinMetadata\\Windows.Foundation.winmd");
                else
                    LogHelper.Error("WinBLEGATTManager", "Failed to find Windows.Foundation.winmd! Are you on Windows 10?");
            else
                LogHelper.Error("WinBLEGATTManager", "Failed to find WinMetadata!");
        }

        private void LoadServices()
        {
            if (Directory.Exists("C:\\Windows\\System32\\WinMetadata"))
                if (File.Exists("C:\\Windows\\System32\\WinMetadata\\Windows.Services.winmd"))
                    Assembly.LoadFrom("C:\\Windows\\System32\\WinMetadata\\Windows.Services.winmd");
                else
                    LogHelper.Error("WinBLEGATTManager", "Failed to find Windows.Services.winmd! Are you on Windows 10?");
            else
                LogHelper.Error("WinBLEGATTManager", "Failed to find WinMetadata!");
        }

        public bool Init(string bruh)
        {
            if (!LoadedAssemblies)
            {
                LoadWindowsContracts();
                LoadFoundation();
                LoadServices();
                LoadedAssemblies = true;
            }
            StartThread();
            return isDeviceConnected;
        }

        void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
        }

        void VerifyClosedDevice()
        {
            if (ghrd != null)
                ghrd.Dispose();
            ghrd = null;
        }

        void StartThread()
        {
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                VerifyClosedDevice();
                ghrd = new GenericHRDevice();
                try { ghrd.FindAndConnect(); isDeviceConnected = true; } catch (HRDeviceException e) { LogHelper.Error("HRManagers/WinBLEGATTManager.cs", 
                    "Failed to FindAndConnect() to HR Device! Exception: " + e); isDeviceConnected = false; }
                if (isDeviceConnected)
                {
                    ghrd.HeartRateUpdated += ghrd_HeartRateUpdated;
                    ghrd.HeartRateDisconnected += ghrd_HeartRateDisconnected;
                }
                while (isDeviceConnected)
                {
                    // Events handle everything, just need to keep this thread alive to avoid GC errors
                    Thread.Sleep(10);
                }
            });
            _thread.Start();
        }

        private void ghrd_HeartRateDisconnected() => isDeviceConnected = false;
        private void ghrd_HeartRateUpdated(HeartRateReading reading) => HR = reading.BeatsPerMinute;

        public bool IsOpen() => isDeviceConnected;
        public bool IsActive() => isDeviceConnected;
        public int GetHR() => HR;

        public void Stop()
        {
            VerifyClosedDevice();
            VerifyClosedThread();
            HR = 0;
            isDeviceConnected = false;
        }
    }
}
*/