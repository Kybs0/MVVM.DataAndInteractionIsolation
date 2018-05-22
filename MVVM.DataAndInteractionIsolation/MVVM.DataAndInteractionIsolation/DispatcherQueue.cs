using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MVVM.DataAndInteractionIsolation
{
    public class DispatcherQueue: DispatcherObject
    {
        private readonly DispatcherPriority _priority = DispatcherPriority.Normal;
        private bool _isTaskRequired;
        private readonly Action _action;

        public DispatcherQueue(Action action, DispatcherPriority priority)
        {
            this._action = action;
            this._priority = priority;
        }

        public void Require()
        {
            if (this._isTaskRequired)
                return;
            this._isTaskRequired = true;
            this.Dispatcher.InvokeAsync(new Action(this.InvokeAction), this._priority);
        }

        public void Invoke(bool withRequire = false)
        {
            this._isTaskRequired |= withRequire;
            this.InvokeAction();
        }

        private void InvokeAction()
        {
            if (!this._isTaskRequired)
                return;
            try
            {
                this._action();
            }
            finally
            {
                this._isTaskRequired = false;
            }
        }
    }
}
