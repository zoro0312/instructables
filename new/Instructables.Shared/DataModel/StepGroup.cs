using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instructables.DataModel
{
    public class StepGroup
    {
        private ObservableCollection<Step> _steps = new ObservableCollection<Step>();
        public ObservableCollection<Step> Steps
        {
            get { return this._steps; }
        }

        public bool ShowTitle
        {
            get
            {
                if (Steps != null && Steps[0] != null)
                {
                    if (Steps[0].stepIndex == 0)
                        return false;
                }
                return true;
            }
        }

        public string StepName { get; set; }
        private string _stepTitle = String.Empty;
        public string StepTitle
        {
            get
            {
                if (!String.IsNullOrEmpty(StepName))
                {
                    String StepNameModified = StepName;
                    if (_stepTitle == null)
                        _stepTitle = String.Empty;
                    return String.Format("{0}" + " " + "{1}", StepName, _stepTitle.ToUpper());
                }
                    
                else
                    return _stepTitle.ToUpper();
            }
            set
            {
                _stepTitle = value;
            }
        }

        public string StepImage
        {
            get
            {
                if (Steps.Count > 0)
                {
                    var step = Steps[0];
                    if (step.VideoList != null && step.VideoList.Count > 0)
                        return step.VideoList[0].ThumbnailURI;
                    else if (step.files.Count > 0)
                        return step.files[0].tinyUrl.Replace("TINY", "SMALL");
                    else
                        return @"http://www.instructables.com/static/defaultIMG/default.RECTANGLE1.png";
                }
                else
                    return String.Empty;
            }
        }
    }
}
