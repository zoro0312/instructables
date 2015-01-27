using Instructables.DataModel;
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

        private static HeroPageDataSource _heroVM;
        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public HeroPageDataSource HeroVM
        {
            get
            {
                if (_heroVM == null)
                {
                    _heroVM = new HeroPageDataSource();
                    //_mainVM.Initialize();

                }
                return _heroVM;
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

        private static LandingPageViewModel _landingVM;
        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public LandingPageViewModel LandingVM
        {
            get
            {
                if (_landingVM == null)
                {
                    _landingVM = new LandingPageViewModel();
                    //_mainVM.Initialize();

                }
                return _landingVM;
            }
        }

    
        /*private static MainViewModel _mainVM;
        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public MainViewModel MainVM
        {
            get
            {
                if (_mainVM == null)
                {
                    _mainVM = new MainViewModel();
                    //_mainVM.Initialize();

                }
                return _mainVM;
            }
        }*/

        private static ExploreViewModel _exploreVM;
        public ExploreViewModel ExploreVM
        {
            get
            {
                if (_exploreVM == null)
                {
                    _exploreVM = new ExploreViewModel();
                }
                return _exploreVM;
            }
        }

        /*private static GroupDetailViewModel _groupDetailVM;
        public GroupDetailViewModel GroupDetailVM
        {
            get
            {
                if (_groupDetailVM == null)
                {
                    _groupDetailVM = new GroupDetailViewModel();
                }
                return _groupDetailVM;
            }
        }*/

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

        /*private static InstructableDetailViewModelOld _detailViewModelOld;
        public InstructableDetailViewModelOld DetailOldVM
        {
            get
            {
                if (_detailViewModelOld == null)
                {
                    _detailViewModelOld = new InstructableDetailViewModelOld();
                }
                return _detailViewModelOld;
            }
        }*/

        private static UserProfileViewModel _userProfileViewModel;
        public UserProfileViewModel UserProfileVM
        {
            get
            {
                if(_userProfileViewModel == null)
                {
                    _userProfileViewModel = new UserProfileViewModel();
                }
                return _userProfileViewModel;
            }
        }

        private static ContestsListViewModel _contestsListViewModel;
        public ContestsListViewModel ContestsListVM
        {
            get
            {
                if(_contestsListViewModel == null)
                {
                    _contestsListViewModel = new ContestsListViewModel();
                }
                return _contestsListViewModel;
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

        private static ContestEntriesViewModel _contestEntriesViewModel;
        public ContestEntriesViewModel ContestEntriesVM
        {
            get
            {
                if (_contestEntriesViewModel == null)
                {
                    _contestEntriesViewModel = new ContestEntriesViewModel();
                }
                return _contestEntriesViewModel;
            }
        }
    }  
}
