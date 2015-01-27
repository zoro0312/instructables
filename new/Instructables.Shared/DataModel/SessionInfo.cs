using Instructables.Common;
using System;
using System.Collections.Generic;

namespace Instructables.DataModel
{
    public class SessionInfo : BindableBase
    {
        public bool _loginSuccess = false;
        public bool loginSuccess
        {
            get
            {
                return _loginSuccess;
            }

            set
            {
                _loginSuccess = value;
                OnPropertyChanged();
            }
        }

        private string _userName = String.Empty;
        public string userName
        {
            get 
            {
                return _userName;
            }

            set 
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private string _passWord = String.Empty;
        public string passWord
        {
            get
            {
                return _passWord;
            }

            set 
            {
                _passWord = value;
                OnPropertyChanged();
            }
        }

        private string _APPSERVER = String.Empty;
        public string APPSERVER
        {
            get
            {
                return _APPSERVER;
            }

            set
            {
                _APPSERVER = value;
                OnPropertyChanged();
            }
        }

        private string _authy = String.Empty;
        public string authy
        {
            get
            {
                return _authy;
            }

            set
            {
                _authy = value;
                OnPropertyChanged();
            }
        }

        private string _JSESSIONID = String.Empty;
        public string JSESSIONID
        {
            get
            {
                return _JSESSIONID;
            }

            set
            {
                _JSESSIONID = value;
                OnPropertyChanged();
            }
        }

        private bool _licenseAgreement = true;
        public bool licenseAgreement
        {
            get
            {
                return _licenseAgreement;
            }

            set
            {
                _licenseAgreement = value;
                OnPropertyChanged();
            }
        }

        private bool _firstTime = true;
        public bool firstTime
        {
            get
            {
                return _firstTime;
            }

            set
            {
                _firstTime = value;
                OnPropertyChanged();
            }
        }

        private bool _dataCollection = true;
        public bool dataCollection
        {
            get
            {
                return _dataCollection;
            }

            set
            {
                _dataCollection = value;
                OnPropertyChanged();
            }
        }

        private bool _showedCreateTutorial = false;
        public bool ShowedCreateTutorial
        {
            get
            {
                return _showedCreateTutorial;
            }

            set
            {
                _showedCreateTutorial = value;
                OnPropertyChanged();
            }
        }
    }
}
