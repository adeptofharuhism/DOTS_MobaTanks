using System;
using Zenject;

namespace Assets.CodeBase.Utility.MVVM
{
    public abstract class ViewModel : IInitializable, IDisposable
    {
        public virtual void Initialize() {
            GetModelValues();
            SubscribeToModel();
        }

        public virtual void Dispose() {
            UnsubscribeFromModel();
        }

        protected abstract void GetModelValues();

        protected abstract void SubscribeToModel();

        protected abstract void UnsubscribeFromModel();
    }
}
