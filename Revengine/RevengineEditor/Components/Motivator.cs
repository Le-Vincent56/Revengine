using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RevengineEditor.Components
{
    internal interface IMSMotivator { }

    [DataContract]
    internal abstract class Motivator : ViewModelBase
    {
        [DataMember]
        public Grievance Owner { get; private set; }

        public Motivator(Grievance owner)
        {
            Debug.Assert(owner != null);
            Owner = owner;
        }
        public abstract IMSMotivator GetMultiSelectionMotivator(MSObject msObject);
    }

    abstract class MSMotivator<T> : ViewModelBase, IMSMotivator where T : Motivator
    {
        bool _updatesEnabled = true;
        public List<T> SelectedMotivators { get; }
        public MSMotivator(MSObject msObject)
        {
            Debug.Assert(msObject?.SelectedGrievances?.Any() == true);
            SelectedMotivators = msObject.SelectedGrievances.Select(grievance => grievance.GetMotivator<T>()).ToList();
            PropertyChanged += (s, e) =>
            {
                if (_updatesEnabled)
                    UpdateMotivators(e.PropertyName);
            };
        }
        public void Refresh()
        {
            // Make sure that anything we gather from the components
            // doesn't propogate back to them
            _updatesEnabled = false;
            UpdateMSMotivator();
            _updatesEnabled = true;
        }

        protected abstract bool UpdateMotivators(string propertyName);
        protected abstract bool UpdateMSMotivator();
    }
}
