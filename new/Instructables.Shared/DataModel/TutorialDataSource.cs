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
    public abstract class TutorialDataCommon : BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public TutorialDataCommon(String title, String picture, String discription)
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
                return new Uri(TutorialDataCommon._baseUri, this._picture);
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
    public class TutorialDataItem : TutorialDataCommon
    {
        public TutorialDataItem(String title, String type, String picture)
            : base(title, type, picture)
        {
        }

    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class TutorialDataSource
    {
        private ObservableCollection<object> _items = new ObservableCollection<object>();
        public ObservableCollection<object> Items
        {
            get { return this._items; }
        }

        /*public TutorialDataSource()
        {
            Items.Add(new TutorialDataItem("Create_Help1",
                    "Assets/create_help1.jpg",
                    "Choose a title, category and channel for your Instructable."
                    ));
            Items.Add(new TutorialDataItem("Create_Help2",
                    "Assets/create_help2.jpg",
                    "Start by adding images and description to your introduction."
                    ));
            Items.Add(new TutorialDataItem("Create_Help3",
                    "Assets/create_help3.jpg",
                    "Quick and easy way to crop, re-size, fix, draw and add text to your photos on the go! Enhance, add style and personalize any image with Pixlr Express. "
                    ));
            Items.Add(new TutorialDataItem("Create_Help4",
                    "Assets/create_help4.jpg",
                    "Create new steps by tapping add step button."
                    ));
            Items.Add(new TutorialDataItem("Create_Help5",
                    "Assets/create_help5.jpg",
                    "You can reorder images by selecting and holding them. You can also add more or delete the images within each step."
                    ));
            Items.Add(new TutorialDataItem("Create_Help6",
                   "Assets/create_help6.jpg",
                   "Tap the overview button to view all your steps. "
                   ));
            Items.Add(new TutorialDataItem("Create_Help7",
                    "Assets/create_help7.jpg",
                    "You can swipe back and forth between steps. "
                    ));
            Items.Add(new TutorialDataItem("Create_Help8",
                    "Assets/create_help8.jpg",
                    "When you are ready to publish your Instructable, press Publish. Once live, it can always be updated."
                    ));
        }*/

        public TutorialDataSource()
        {
            Items.Add(new TutorialDataItem("Create_Help1",
                    "Assets/Screen_1.jpg",
                    "Choose a title, category, and language for your Instructable."
                    ));
            Items.Add(new TutorialDataItem("Create_Help2",
                    "Assets/Screen_2.jpg",
                    "Add photos and descriptions to your steps."
                    ));
            Items.Add(new TutorialDataItem("Create_Help3",
                    "Assets/Screen_3.jpg",
                    "Create new steps by tapping the add step button."
                    ));
            Items.Add(new TutorialDataItem("Create_Help4",
                    "Assets/Screen_4.jpg",
                    "Tap the overview button to view all of your steps."
                    ));
            Items.Add(new TutorialDataItem("Create_Help5",
                    "Assets/Screen_5.jpg",
                    "You can swipe back and forth between steps. "
                    ));
            Items.Add(new TutorialDataItem("Create_Help6",
                   "Assets/Screen_6.jpg",
                   "When finished, publish your Instructable! Once live, it can always be updated."
                   ));
        }
    }
}
