using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Instructables.ViewModels
{
    public class ViewModelLocator
    {
        public static ViewModelLocator Instance = new ViewModelLocator();

        private static InstructableDetailViewModel _detailViewModel;
        public InstructableDetailViewModel DetailVM
        {
            get
            {
                if (_detailViewModel == null)
                {
                    _detailViewModel = new InstructableDetailViewModel();
                }
                return _detailViewModel;
            }
        }

        private static AboutViewModel _aboutViewModel;
        public AboutViewModel AboutVM
        {
            get 
            { 
                if(_aboutViewModel == null)
                {
                    _aboutViewModel = new AboutViewModel();
                }
                return _aboutViewModel;
            }
        }

        private static LicenseAgreementViewModel _licenseAgreementViewModel;
        public LicenseAgreementViewModel LicenseVM
        {
            get
            {
                if (_licenseAgreementViewModel == null)
                {
                    _licenseAgreementViewModel = new LicenseAgreementViewModel();
                }
                return _licenseAgreementViewModel;
            }
        }

        private static LandingViewModel _landingViewModel;
        public LandingViewModel LandingVM
        {
            get
            {
                if (_landingViewModel == null)
                {
                    _landingViewModel = new LandingViewModel();
                }
                return _landingViewModel;
            }
        }

        private static ExploreViewModel _exploreViewModel;
        public ExploreViewModel ExploreVM
        {
            get
            {
                if (_exploreViewModel == null)
                {
                    _exploreViewModel = new ExploreViewModel();
                }
                return _exploreViewModel;
            }
        }

        private static ContestViewModel _contestViewModel;
        public ContestViewModel ContestVM
        {
            get
            {
                if (_contestViewModel == null)
                {
                    _contestViewModel = new ContestViewModel();
                }
                return _contestViewModel;
            }
        }

        private static UserProfileViewModel _userProfileViewModel;
        public UserProfileViewModel UserProfileVM
        {
            get
            {
                if (_userProfileViewModel == null)
                {
                    _userProfileViewModel = new UserProfileViewModel();
                }
                return _userProfileViewModel;
            }
        }

        private static CreateViewModel _createViewModel;
        public CreateViewModel CreateVM
        {
            get
            {
                if (_createViewModel == null)
                {
                    _createViewModel = new CreateViewModel();
                }
                return _createViewModel;
            }
        }
    }
}
