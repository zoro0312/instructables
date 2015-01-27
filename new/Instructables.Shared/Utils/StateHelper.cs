using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Instructables.Utils
{
    public class StateHelper : DependencyObject
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached(
            "State", typeof(String), typeof(StateHelper), new PropertyMetadata("Normal", StateChanged));

        internal static void StateChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            try
            {
                string newState = (string)args.NewValue;
                Debug.WriteLine(String.Format("Changing state from {0} to {1} on {2} ", args.OldValue, newState, target));
                if (newState != null)
                {
                    bool res = VisualStateManager.GoToState((Control)target, newState, true);
                    Debug.WriteLine("State Change result was: " + res);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in StateHelper: " + ex.Message);
            }
        }

        public static void SetState(DependencyObject obj, string value)
        {
            obj.SetValue(StateProperty, value);
        }

        public static string GetState(DependencyObject obj)
        {
            return (string)obj.GetValue(StateProperty);
        }
    }
}
