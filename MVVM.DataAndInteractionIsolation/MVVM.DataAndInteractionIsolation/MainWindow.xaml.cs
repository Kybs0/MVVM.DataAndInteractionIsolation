using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MVVM.DataAndInteractionIsolation.Annotations;

namespace MVVM.DataAndInteractionIsolation
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ViewModel();
        }

        #region 删除确认窗口

        /// <summary>
        /// 删除窗口
        /// </summary>
        public static readonly DependencyProperty ShowDeleteWaringProperty = DependencyProperty.Register(
            "ShowDeleteWaring", typeof(UIDelegateProgress<List<DocumentData>, MessageBoxResult>), typeof(MainWindow), new PropertyMetadata(default(UIDelegateProgress<List<DocumentData>, MessageBoxResult>),
                (d, e) => ((UIDelegateProgress<List<DocumentData>, MessageBoxResult>)e.NewValue)?.Start(((MainWindow)d).ShowDeleteWaringShow)));

        private MessageBoxResult ShowDeleteWaringShow(List<DocumentData> items)
        {
            var result = MessageBox.Show($"是否删除此{items.Count}份数据？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.None);
            return result;
        }

        public UIDelegateProgress<List<DocumentData>, MessageBoxResult> ShowDeleteWaring
        {
            get => (UIDelegateProgress<List<DocumentData>, MessageBoxResult>)GetValue(ShowDeleteWaringProperty);
            set => SetValue(ShowDeleteWaringProperty, value);
        }

        #endregion

        #region 删除动画

        /// <summary>
        /// 删除窗口
        /// </summary>
        public static readonly DependencyProperty DeleteDatasAnimationProperty = DependencyProperty.Register(
            "DeleteDatasAnimation", typeof(UIDelegateProgress<List<DocumentData>>), typeof(MainWindow), new PropertyMetadata(default(UIDelegateProgress<List<DocumentData>>),
                (d, e) => ((UIDelegateProgress<List<DocumentData>>)e.NewValue)?.StartAsync(((MainWindow)d).DeleteDatasn)));

        private async Task DeleteDatasn(List<DocumentData> items)
        {
            List<Storyboard> storyboards = new List<Storyboard>();
            foreach (var courseware in items)
            {
                var listBoxItem = DocumentListBox.ItemContainerGenerator.ContainerFromItem(courseware) as ListBoxItem;
                var border = listBoxItem?.VisualDescendant<Border>();
                var storyboard = (Storyboard)border?.Resources["ItemRemovedStoryboard"];
                if (storyboard == null)
                {
                    //如果找不到storyBoard，则中断动画的执行。
                    return;
                }
                storyboards.Add(storyboard);
            }
            await ExecuteStoryboradAsync(storyboards);
        }

        public UIDelegateProgress<List<DocumentData>, MessageBoxResult> DeleteDatasAnimation
        {
            get => (UIDelegateProgress<List<DocumentData>, MessageBoxResult>)GetValue(DeleteDatasAnimationProperty);
            set => SetValue(DeleteDatasAnimationProperty, value);
        }

        /// <summary>
        /// 执行多个动画
        /// </summary>
        /// <param name="storyboards"></param>
        /// <returns></returns>
        public async Task ExecuteStoryboradAsync(List<Storyboard> storyboards)
        {
            if (storyboards == null) throw new ArgumentNullException(nameof(storyboards));

            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            DispatcherQueue dispatcherQueue = new DispatcherQueue(() =>
            {
                autoResetEvent.Set();
            }, DispatcherPriority.Normal);

            int count = storyboards.Count;
            for (int i = 0; i < count; i++)
            {
                var storybaord = storyboards[i];
                storybaord.Completed += OnStoryboardCompleted;
                storybaord.Begin();

                void OnStoryboardCompleted(object sender, EventArgs e)
                {
                    storybaord.Completed -= OnStoryboardCompleted;
                    dispatcherQueue.Require();
                }
            }

            await Task.Run(() => { autoResetEvent.WaitOne(); });
        }

        #endregion
    }

    public static class VisualHelper
    {
        public static T VisualDescendant<T>(this Visual element) where T : class
        {
            if (element == null)
            {
                return default(T);
            }
            if (element.GetType() == typeof(T))
            {
                return element as T;
            }
            T foundElement = null;
            (element as FrameworkElement)?.ApplyTemplate();
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = visual.VisualDescendant<T>();
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }
    }
}
