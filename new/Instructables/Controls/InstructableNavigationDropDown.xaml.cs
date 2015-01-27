using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Instructables.Controls
{
    public sealed partial class InstructableNavigationDropDown : UserControl
    {
        public event EventHandler StepSelected;

        public InstructableNavigationDropDown()
        {
            this.InitializeComponent();
            Loaded += (sender, args) =>
                {
                    Debug.WriteLine(StepListBox.SelectedIndex);
                };
        }

        public void SetTopOffset(double offset)
        {
            topOffset.Height = offset;
        }

        public void SetSelectedIndex(int index)
        {
            StepListBox.SelectedIndex = index;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StepSelected != null)
                StepSelected(this, new EventArgs());
        }
    }
}
