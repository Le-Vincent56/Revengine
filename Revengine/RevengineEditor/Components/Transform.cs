using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RevengineEditor.Utilities;

namespace RevengineEditor.Components
{
    [DataContract]
    internal class Transform : Motivator
    {
        private Vector3 _position;
        [DataMember]
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                if(_position != value)
                {
                    _position = value;
                }
                OnPropertyChanged(nameof(Position));
            }
        }

        private Vector3 _rotation;
        [DataMember]
        public Vector3 Rotation
        {
            get { return _rotation; }
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                }
                OnPropertyChanged(nameof(Rotation));
            }
        }

        private Vector3 _scale;
        [DataMember]
        public Vector3 Scale
        {
            get { return _scale; }
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                }
                OnPropertyChanged(nameof(Scale));
            }
        }

        public Transform(Grievance owner) : base(owner)
        {

        }

        public override IMSMotivator GetMultiSelectionMotivator(MSObject msObject)
        {
            return new MSTransform(msObject);
        }
    }

    sealed class MSTransform : MSMotivator<Transform>
    {
        private float? _posX;
        public float? PosX
        {
            get { return _posX; }
            set
            {
                if(!_posX.IsTheSameAs(value))
                {
                    _posX = value;
                    OnPropertyChanged(nameof(PosX));
                }
            }
        }

        private float? _posY;
        public float? PosY
        {
            get { return _posY; }
            set
            {
                if (!_posY.IsTheSameAs(value))
                {
                    _posY = value;
                    OnPropertyChanged(nameof(PosY));
                }
            }
        }

        private float? _posZ;
        public float? PosZ
        {
            get { return _posZ; }
            set
            {
                if (!_posZ.IsTheSameAs(value))
                {
                    _posZ = value;
                    OnPropertyChanged(nameof(PosZ));
                }
            }
        }

        private float? _rotX;
        public float? RotX
        {
            get { return _rotX; }
            set
            {
                if (!_rotX.IsTheSameAs(value))
                {
                    _rotX = value;
                    OnPropertyChanged(nameof(RotX));
                }
            }
        }

        private float? _rotY;
        public float? RotY
        {
            get { return _rotY; }
            set
            {
                if (!_rotY.IsTheSameAs(value))
                {
                    _rotY = value;
                    OnPropertyChanged(nameof(RotY));
                }
            }
        }

        private float? _rotZ;
        public float? RotZ
        {
            get { return _rotZ; }
            set
            {
                if (!_rotZ.IsTheSameAs(value))
                {
                    _rotZ = value;
                    OnPropertyChanged(nameof(RotZ));
                }
            }
        }

        private float? _scaleX;
        public float? ScaleX
        {
            get { return _scaleX; }
            set
            {
                if (!_scaleX.IsTheSameAs(value))
                {
                    _scaleX = value;
                    OnPropertyChanged(nameof(ScaleX));
                }
            }
        }

        private float? _scaleY;
        public float? ScaleY
        {
            get { return _scaleY; }
            set
            {
                if (!_scaleY.IsTheSameAs(value))
                {
                    _scaleY = value;
                    OnPropertyChanged(nameof(ScaleY));
                }
            }
        }

        private float? _scaleZ;
        public float? ScaleZ
        {
            get { return _scaleZ; }
            set
            {
                if (!_scaleZ.IsTheSameAs(value))
                {
                    _scaleZ = value;
                    OnPropertyChanged(nameof(ScaleZ));
                }
            }
        }

        public MSTransform(MSObject msObject) : base(msObject)
        {
            Refresh();
        }

        /// <summary>
        /// Update the Transform Motivator
        /// </summary>
        /// <param name="propertyName">The Transform property to update</param>
        /// <returns>True if the update succeeded, false if not</returns>
        protected override bool UpdateMotivators(string propertyName)
        {
            switch(propertyName)
            {
                case nameof(PosX):
                case nameof(PosY):
                case nameof(PosZ):
                    SelectedMotivators.ForEach(m => m.Position = new Vector3(
                        _posX ?? m.Position.X, _posY ?? m.Position.Y, _posZ ?? m.Position.Z));
                    return true;

                case nameof(RotX):
                case nameof(RotY):
                case nameof(RotZ):
                    SelectedMotivators.ForEach(m => m.Rotation = new Vector3(
                        _rotX ?? m.Rotation.X, _rotY ?? m.Rotation.Y, _rotZ ?? m.Rotation.Z));
                    return true;

                case nameof(ScaleX):
                case nameof(ScaleY):
                case nameof(ScaleZ):
                    SelectedMotivators.ForEach(m => m.Scale = new Vector3(
                        _scaleX ?? m.Scale.X, _scaleY ?? m.Scale.Y, _scaleZ ?? m.Scale.Z));
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Update a Multiselection of Motivators
        /// </summary>
        /// <returns>True once update completes</returns>
        protected override bool UpdateMSMotivator()
        {
            PosX = MSObject.GetMixedValue(SelectedMotivators, new Func<Transform, float>(x => x.Position.X));
            PosY = MSObject.GetMixedValue(SelectedMotivators, new Func<Transform, float>(x => x.Position.Y));
            PosZ = MSObject.GetMixedValue(SelectedMotivators, new Func<Transform, float>(x => x.Position.Z));

            RotX = MSObject.GetMixedValue(SelectedMotivators, new Func<Transform, float>(x => x.Rotation.X));
            RotY = MSObject.GetMixedValue(SelectedMotivators, new Func<Transform, float>(x => x.Rotation.Y));
            RotZ = MSObject.GetMixedValue(SelectedMotivators, new Func<Transform, float>(x => x.Rotation.Z));

            ScaleX = MSObject.GetMixedValue(SelectedMotivators, new Func<Transform, float>(x => x.Scale.X));
            ScaleY = MSObject.GetMixedValue(SelectedMotivators, new Func<Transform, float>(x => x.Scale.Y));
            ScaleZ = MSObject.GetMixedValue(SelectedMotivators, new Func<Transform, float>(x => x.Scale.Z));

            return true;
        }
    }
}
