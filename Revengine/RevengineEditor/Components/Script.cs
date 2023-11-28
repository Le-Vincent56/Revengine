using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RevengineEditor.Components
{
    [DataContract]
    internal class Script : Motivator
    {
        private string _name;

        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public Script(Grievance owner) : base(owner) { }

        public override IMSMotivator GetMultiSelectionMotivator(MSObject msObject) => new MSScript(msObject);
    }

    sealed class MSScript : MSMotivator<Script>
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public MSScript(MSObject mSGrievance) : base(mSGrievance) 
        {
            Refresh();
        }

        protected override bool UpdateMotivators(string propertyName)
        {
            if(propertyName == nameof(Name))
            {
                SelectedMotivators.ForEach(m => m.Name = _name);
                return true;
            }

            return false;
        }

        protected override bool UpdateMSMotivator()
        {
            Name = MSObject.GetMixedValue(SelectedMotivators, new Func<Script, string>(x => x.Name));
            return true;
        }
    }
}
