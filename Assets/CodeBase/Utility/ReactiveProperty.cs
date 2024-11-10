using System;

namespace Assets.CodeBase.Utility
{
	public interface IReactiveGetter<T>
	{
		event Action<T> OnChanged;

		T Value { get; }
	}

	public class ReactiveProperty<T> : IReactiveGetter<T>
	{
		public event Action<T> OnChanged;

		protected T _value;

		private readonly Action<T> _setterAction;
		
		public ReactiveProperty(bool passOnEquality = false) {
			_setterAction = passOnEquality
				? SetValueWithPassOnEquality
				: SetValue;
		}
		
		public T Value {
			get => _value;
			set => _setterAction.Invoke(value);
		}

		private void SetValue(T value) {
			_value = value;
			OnChanged?.Invoke(_value);
		}

		private void SetValueWithPassOnEquality(T value) {
			if (_value.Equals(value))
				return;
			
			_value = value;
			OnChanged?.Invoke(_value);
		}
	}
}