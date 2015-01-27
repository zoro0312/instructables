using Instructables.DataModel;
using Instructables.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Instructables.Controls
{
    class LandingPageFollowedGridView : GridView
    {
        protected override void PrepareContainerForItemOverride(Windows.UI.Xaml.DependencyObject element, object item)
        {
            try
            {
                var sampleItem = item as Instructable;
                if (sampleItem != null && sampleItem.Group.Layout == DataGroup.LayoutType.MainFeature)
                {
                    if (LandingPageViewModel.CurrentScreenMetrics.Name == "768")
                    {
                        switch (sampleItem.GroupOrdinal)
                        {
                            case 0:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 12);
                                break;
                            case 1:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 8);
                                break;
                            default:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 4);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 4);
                                break;
                        }
                    }
                    else if (LandingPageViewModel.CurrentScreenMetrics.Name == "1080")
                    {
                        switch (sampleItem.GroupOrdinal)
                        {
                            case 0:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 12);
                                break;
                            case 3:
                            case 4:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 8);
                                break;
                            default:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 4);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 4);
                                break;
                        }
                    }
                    else if (LandingPageViewModel.CurrentScreenMetrics.Name == "1440" || LandingPageViewModel.CurrentScreenMetrics.Name == "Portrait1440")
                    {
                        switch (sampleItem.GroupOrdinal)
                        {
                            case 0:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 12);
                                break;
                            case 2:
                            case 6:
                            case 8:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 8);
                                break;
                            default:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 4);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 4);
                                break;
                        }
                    }
                }
                else if (sampleItem != null && sampleItem.Group.Layout == DataGroup.LayoutType.DualFeature)
                {
                    //                    if (sampleItem.GroupOrdinal == 0 || sampleItem.GroupOrdinal == 5)
                    /*if (sampleItem.GroupOrdinal <= 1)
                    {
                        element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                        element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 6);
                    }
                    else if (sampleItem.GroupOrdinal == 7 && LandingPageViewModel.CurrentScreenMetrics.Name == "1440")
                    {
                        element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                        element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 8);
                    }
                    else
                    {
                        element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 5);
                        element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 5);
                    }*/
                    if (LandingPageViewModel.CurrentScreenMetrics.Name == "1080")
                    {
                        switch (sampleItem.GroupOrdinal)
                        {
                            case 0:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 16);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 38);
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 7:
                            case 8:
                            case 9:
                            case 10:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 19);
                                break;
                            case 6:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 38);
                                break;
                            case 11:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 13);
                                break;
                        }
                    }
                    else
                    {
                        switch (sampleItem.GroupOrdinal % 8)
                        {
                            case 0:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 16);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 38);
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 5:
                            case 6:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 19);
                                break;
                            case 4:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 38);
                                break;
                            case 7:
                                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 13);
                                break;
                        }
                    }
                }
                else if (sampleItem != null && sampleItem.Group.Layout == DataGroup.LayoutType.SingleFeature)
                {
                    if (sampleItem.GroupOrdinal == 0 || (LandingPageViewModel.CurrentScreenMetrics.Name == "1440" && sampleItem.GroupOrdinal == 4) || (LandingPageViewModel.CurrentScreenMetrics.Name == "1440" && sampleItem.GroupOrdinal == 6))
                    {
                        element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                        element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 8);
                    }
                    else if (sampleItem.GroupOrdinal == 5 && LandingPageViewModel.CurrentScreenMetrics.Name == "1080")
                    {
                        element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 8);
                        element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 8);
                    }
                    else
                    {
                        element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 4);
                        element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 4);
                    }
                }
                else if (sampleItem != null && sampleItem.Group.Layout == DataGroup.LayoutType.EBook)
                {
                    element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 4);
                    element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 6);
                }
                else
                {
                    element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 4);
                    element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 4);
                }
            }
            finally
            {
                base.PrepareContainerForItemOverride(element, item);
            }
        }
    }
}
