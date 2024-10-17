using System;

namespace Assets.CodeBase.Utility
{ 
    public interface IReactiveGetter<T> {
        event Action<T> OnChanged;

        T Value { get; }
    }

    public class ReactiveProperty<T> : IReactiveGetter<T>
    {
        public event Action<T> OnChanged;

        protected T _value;

        public T Value {
            get => _value;
            set => SetValue(value);
        }

        protected virtual void SetValue(T value) {
            _value = value;
            OnChanged?.Invoke(_value);
        }
    }

    public class ReactivePropertyWithPassOnEquality<T> : ReactiveProperty<T>
    {
        protected override void SetValue(T value) {
            if (_value.Equals(value))
                return;

            base.SetValue(value);
        }
    }
}