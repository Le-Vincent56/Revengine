using RevengineEditor.GameProject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RevengineEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    public class GameEntity : ViewModelBase
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

        [DataMember]
        public Scene ParentScene { get; private set; }
        [DataMember(Name = nameof(Motivators))]

        private readonly ObservableCollection<Motivator> _motivators = new ObservableCollection<Motivator>();
        public ReadOnlyObservableCollection<Motivator> Motivators { get; private set; }

        public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            _motivators.Add(new Transform(this));
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if(_motivators != null)
            {
                Motivators = new ReadOnlyObservableCollection<Motivator>(_motivators);
                OnPropertyChanged(nameof(Motivators));
            }
        }
    }
}
