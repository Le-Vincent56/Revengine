﻿using System;
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
        public GameEntity Owner { get; private set; }

        public Motivator(GameEntity owner)
        {
            Debug.Assert(owner != null);
            Owner = owner;
        }
    }

    abstract class MSMotivator<T> : ViewModelBase, IMSMotivator where T : Motivator
    {

    }
}