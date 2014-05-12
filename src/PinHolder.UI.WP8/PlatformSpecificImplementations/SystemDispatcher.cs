using System;
using System.ComponentModel;
using System.Windows.Threading;
using Curacao.Mvvm.Abstractions.Services;
using PinHolder.Annotations;

namespace PinHolder.PlatformSpecificImplementations
{
    internal class SystemDispatcher : ISystemDispatcher
    {
        private Dispatcher _instance;

        private bool? _designer;

        public void InvokeOnUIifNeeded(Action action)
        {
            BeginInvoke(action);
        }

        public void Initialize([NotNull] Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            _instance = dispatcher;

            if (_designer == null)
            {
                _designer = DesignerProperties.IsInDesignTool;
            }
        }

        private void BeginInvoke(Action a)
        {
            if (_instance.CheckAccess() || _designer == true)
            {
                a();
            }
            else
            {
                _instance.BeginInvoke(a);
            }
        }
    }
}