using Instructables.DataModel;
using Instructables.DataServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Data;

namespace Instructables.Common
{
    /// <summary>
    /// Value converter that translates true to false and vice versa.
    /// </summary>
    public sealed class SessionManager
    {
        static private SessionInfo _sessionInfo = null;
        static public SessionInfo sessionInfo
        {
            get
            {
                if (_sessionInfo == null)
                    _sessionInfo = new SessionInfo();
                return _sessionInfo;
            }
        }

        static public async Task ReadSession()
        {
            try
            {
                StorageFolder sessionFolder = null;
                sessionFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("SessionFolder", CreationCollisionOption.OpenIfExists);
                if (sessionFolder != null)
                {
                    StorageFile file = await sessionFolder.CreateFileAsync("SessionInfo", CreationCollisionOption.OpenIfExists);
                    if (file != null)
                    {
                        string fileContent = await FileIO.ReadTextAsync(file);
                        if(fileContent != String.Empty)
                        {
                            _sessionInfo = SerializationHelper.Deserialize<SessionInfo>(fileContent);
                        }
                        else 
                        {
                            if (_sessionInfo == null)
                                _sessionInfo = new SessionInfo();
                        }
                       
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
            }
        }

        static public async Task WriteSession()
        {
            if (_sessionInfo == null)
                return;
            StorageFolder sessionFolder = null;
            try
            {
                sessionFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("SessionFolder", CreationCollisionOption.OpenIfExists);
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
            }

            if (sessionFolder != null)
            {
                try
                {
                    StorageFile file = await sessionFolder.CreateFileAsync("SessionInfo", CreationCollisionOption.ReplaceExisting);
                    var sessionString = SerializationHelper.Serialize<SessionInfo>(_sessionInfo);
                    await FileIO.WriteTextAsync(file, sessionString);
                }
                catch (FileNotFoundException ex)
                {
                    Debug.WriteLine(String.Format("Error Retrieving Data: {0}", ex.Message));
                }
            }
        }

        static public async Task DeleteSession()
        {
            StorageFolder sessionFolder = null;
            sessionFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("SessionFolder", CreationCollisionOption.OpenIfExists);
            if (sessionFolder != null)
            {
                StorageFile file = await sessionFolder.CreateFileAsync("SessionInfo", CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    await file.DeleteAsync();
                }
            }
        }

        static public void UpdateLoginInfo(string userName, string passWord, string APPSERVER, string authy, string JSESSIONID)
        {
            if (_sessionInfo == null)
                return;
            _sessionInfo.loginSuccess = true;
            _sessionInfo.userName = userName;
            _sessionInfo.passWord = passWord;
            _sessionInfo.APPSERVER = APPSERVER;
            _sessionInfo.authy = authy;
            _sessionInfo.JSESSIONID = JSESSIONID;
        }

        static public void ResetLoginInfo()
        {
            if (_sessionInfo == null)
                return;
            _sessionInfo.loginSuccess = false;
            _sessionInfo.userName = String.Empty;
            _sessionInfo.passWord = String.Empty;
            _sessionInfo.APPSERVER = String.Empty;
            _sessionInfo.authy = String.Empty;
            _sessionInfo.JSESSIONID = String.Empty;
            _sessionInfo.ShowedCreateTutorial = false;
        }

        static public bool IsLoginSuccess()
        {
            if (_sessionInfo == null)
                return false;
            return _sessionInfo.loginSuccess;
        }

        static public string GetLoginUserName()
        {
            if (_sessionInfo == null)
                return String.Empty;
            return _sessionInfo.userName;
        }

        static public bool IsFirstTime()
        {
            if (_sessionInfo == null)
                return true;
            return _sessionInfo.firstTime;
        }

        static public bool IsShowLicenseAgreement()
        {
            if (_sessionInfo == null)
                return true;
            return _sessionInfo.licenseAgreement;
        }

        static public void NotFirstTime()
        {
            if (_sessionInfo == null)
                return;
            _sessionInfo.firstTime = false;
        }

        static public void NeverShowLicenseAgreement()
        {
            if (_sessionInfo == null)
                return;
            _sessionInfo.licenseAgreement = false;
        }

        static public void ResetFirstTime()
        {
            if (_sessionInfo == null)
                return;
            _sessionInfo.firstTime = true;
        }

        static public bool IsGAEnable()
        {
            if (_sessionInfo == null)
                return true;
            return _sessionInfo.dataCollection;
        }

        static public void SetGAEnable(bool enable)
        {
            if (_sessionInfo == null)
                return;
            _sessionInfo.dataCollection = enable;
        }

        static public bool ShowedCreateTutorial()
        {
            if (_sessionInfo == null)
                return false;
            return _sessionInfo.ShowedCreateTutorial;
        }

        static public void SetShowedCreateTutorial(bool show)
        {
            if (_sessionInfo == null)
                return;
            _sessionInfo.ShowedCreateTutorial = show;
        }
    }
}
