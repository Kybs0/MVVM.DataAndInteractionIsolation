using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MVVM.DataAndInteractionIsolation.Annotations;

namespace MVVM.DataAndInteractionIsolation
{
    /// <summary>
    /// UI交互处理-提供可调用UI交互的操作
    /// </summary>
    public class UIDelegateOperation : INotifyPropertyChanged, IUIDelegateAction
    {
        private UIDelegateProgress _delegateProgress;

        public UIDelegateProgress DelegateProgress
        {
            get => _delegateProgress;
            private set
            {
                _delegateProgress = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void Execute()
        {
            var delegateProgress = new UIDelegateProgress();
            delegateProgress.ProgressCompleted += () =>
            {
                _delegateProgress = null;
            };
            DelegateProgress = delegateProgress;
        }

        /// <summary>
        /// 异步执行
        /// 交互处理完成并回调
        /// </summary>
        public async Task ExecuteAsync()
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            var delegateProgress = new UIDelegateProgress();
            delegateProgress.ProgressCompleted += () =>
            {
                _delegateProgress = null;

                autoResetEvent.Set();
            };
            DelegateProgress = delegateProgress;
            await Task.Run(() => { autoResetEvent.WaitOne(); });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// UI交互处理-提供可同步调用UI交互的操作
    /// </summary>
    /// <typeparam name="T">输入/输出类型</typeparam>
    public class UIDelegateAction<T> : INotifyPropertyChanged, IUIDelegateAction<T>
    {
        private UIDelegateProgress<T> _delegateProgress;

        public UIDelegateProgress<T> DelegateProgress
        {
            get => _delegateProgress;
            private set
            {
                _delegateProgress = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        public void Execute(T parameter)
        {
            var delegateProgress = new UIDelegateProgress<T>(parameter);
            delegateProgress.ProgressCompleted += () =>
            {
                _delegateProgress = null;
            };
            DelegateProgress = delegateProgress;
        }
        /// <summary>
        /// 异步执行
        /// 交互处理完成并回调
        /// </summary>
        public async Task ExecuteAsync(T parameter)
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            var delegateProgress = new UIDelegateProgress<T>(parameter);
            delegateProgress.ProgressCompleted += () =>
            {
                _delegateProgress = null;

                autoResetEvent.Set();
            };
            DelegateProgress = delegateProgress;

            await Task.Run(() => { autoResetEvent.WaitOne(); });
        }

        /// <summary>
        /// 异步执行并返回结果
        /// </summary>
        public async Task<T> ExecuteWithResultAsync()
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            var delegateProgress = new UIDelegateProgress<T>();
            delegateProgress.ProgressCompleted += () =>
            {
                _delegateProgress = null;

                autoResetEvent.Set();
            };
            DelegateProgress = delegateProgress;

            await Task.Run(() => { autoResetEvent.WaitOne(); });

            return delegateProgress.Result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// UI交互处理-提供可同步调用UI交互的操作
    /// </summary>
    /// <typeparam name="TInput">输入类型</typeparam>
    /// <typeparam name="TOut">输出类型</typeparam>
    public class UIDelegateAction<TInput, TOut> : INotifyPropertyChanged, IUIDelegateAction<TInput, TOut>
    {
        private UIDelegateProgress<TInput, TOut> _delegateProgress;

        public UIDelegateProgress<TInput, TOut> DelegateProgress
        {
            get => _delegateProgress;
            private set
            {
                _delegateProgress = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        public void Execute(TInput parameter)
        {
            var delegateProgress = new UIDelegateProgress<TInput, TOut>(parameter);
            delegateProgress.ProgressCompleted += () =>
            {
                _delegateProgress = null;
            };
            DelegateProgress = delegateProgress;
        }

        /// <summary>
        /// 执行并返回结果
        /// </summary>
        public TOut ExecuteWithResult(TInput parameter)
        {
            var delegateProgress = new UIDelegateProgress<TInput, TOut>(parameter);
            delegateProgress.ProgressCompleted += () =>
            {
                _delegateProgress = null;
            };
            DelegateProgress = delegateProgress;
            return delegateProgress.Result;
        }

        /// <summary>
        /// 异步执行并返回结果
        /// </summary>
        public async Task<TOut> ExecuteWithResultAsync(TInput parameter)
        {
            var delegateProgress = new UIDelegateProgress<TInput, TOut>(parameter);
            await SetDelegateProgress(delegateProgress);
            return delegateProgress.Result;
        }
        private async Task SetDelegateProgress(UIDelegateProgress<TInput, TOut> delegateProgress)
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            delegateProgress.ProgressCompleted += () =>
            {
                _delegateProgress = null;
                autoResetEvent.Set();
            };
            DelegateProgress = delegateProgress;
            await Task.Run(() => { autoResetEvent.WaitOne(); });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// UI交互处理接口
    /// </summary>
    public interface IUIDelegateAction
    {

        UIDelegateProgress DelegateProgress { get; }

        /// <summary>
        /// 执行
        /// </summary>
        void Execute();

        /// <summary>
        /// 异步执行
        /// </summary>
        Task ExecuteAsync();
    }

    /// <summary>
    /// UI交互处理接口
    /// </summary>
    /// <typeparam name="T">输入/输出类型</typeparam>
    public interface IUIDelegateAction<T>
    {
        UIDelegateProgress<T> DelegateProgress { get; }

        /// <summary>
        /// 执行
        /// </summary>
        void Execute(T parameter);

        /// <summary>
        /// 异步执行
        /// </summary>
        Task ExecuteAsync(T parameter);

        /// <summary>
        /// 异步执行并返回结果
        /// </summary>
        Task<T> ExecuteWithResultAsync();
    }

    /// <summary>
    /// UI交互处理接口
    /// </summary>
    /// <typeparam name="TInput">输入类型</typeparam>
    /// <typeparam name="TOut">输出类型</typeparam>
    public interface IUIDelegateAction<TInput, TOut>
    {
        UIDelegateProgress<TInput, TOut> DelegateProgress { get; }

        /// <summary>
        /// 执行
        /// </summary>
        void Execute(TInput parameter);

        /// <summary>
        /// 执行并返回结果
        /// </summary>
        TOut ExecuteWithResult(TInput parameter);

        /// <summary>
        /// 异步执行并返回结果
        /// </summary>
        Task<TOut> ExecuteWithResultAsync(TInput parameter);
    }
}
