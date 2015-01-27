using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Instructables.Common;
using Windows.UI.Xaml;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace Instructables.DataModel
{
    /// <summary>
    /// Base class for <see cref="TutorialDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class HeroPageDataCommon : BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public HeroPageDataCommon(String title, String picture, String discription)
        {
            this._title = title;
            this._picture = picture;
            this._discription = discription;
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private Uri _image = null;
        private String _picture = null;
        public Uri Image
        {
            get
            {
                return new Uri(HeroPageDataCommon._baseUri, this._picture);
            }

            set
            {
                this._picture = null;
                this.SetProperty(ref this._image, value);
            }
        }

        private string _discription = string.Empty;
        public string Discription
        {
            get { return this._discription; }
            set { this.SetProperty(ref this._discription, value); }
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class HeroPageDataItem : HeroPageDataCommon
    {
        public HeroPageDataItem(String title, String type, String picture)
            : base(title, type, picture)
        {
        }

    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class HeroPageDataSource : BindableBase
    {
        private ObservableCollection<object> _items = new ObservableCollection<object>();
        public ObservableCollection<object> Items
        {
            get { return this._items; }
        }

        private string _currentItemTitle = String.Empty;
        public string CurrentItemTitle
        {
            get
            {
                return _currentItemTitle;
            }

            set
            {
                _currentItemTitle = value;
                OnPropertyChanged();
            }
        }

        public int fitWidth
        {
            get
            {
                return (int)Window.Current.Bounds.Width;
            }
        }

        public int fitSize
        {
            get
            {
                return (int)(fitWidth * 9 / 10);
            }
        }


        public HeroPageDataSource()
        {
            Items.Add(new HeroPageDataItem("engines",
                    "Assets/land-image1.jpg",
                    "Choose a title, category, and language for your Instructable."
                    ));
            Items.Add(new HeroPageDataItem("cloths",
                    "Assets/land-image2.jpg",
                    "Add photos and descriptions to your steps."
                    ));
            Items.Add(new HeroPageDataItem("furniture",
                    "Assets/land-image3.jpg",
                    "Create new steps by tapping the add step button."
                    ));
            Items.Add(new HeroPageDataItem("costumes",  //"costums",
                    "Assets/land-image4.jpg",
                    "Tap the overview button to view all of your steps."
                    ));
            Items.Add(new HeroPageDataItem("dinner",
                    "Assets/land-image5.jpg",
                    "You can swipe back and forth between steps. "
                    ));
            Items.Add(new HeroPageDataItem("music",
                   "Assets/land-image6.jpg",
                   "When finished, publish your Instructable! Once live, it can always be updated."
                   ));
        }
    }
}
