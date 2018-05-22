using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MVVM.DataAndInteractionIsolation.Annotations;

namespace MVVM.DataAndInteractionIsolation
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            DeleteCommand = new DelegateCommand(DeleteItems_OnExecute);
        }

        private ObservableCollection<DocumentData> _itemsSource = new ObservableCollection<DocumentData>()
        {
            new DocumentData() {Name = "数据1"},
            new DocumentData() {Name = "数据2"},
            new DocumentData() {Name = "数据3"},
            new DocumentData() {Name = "数据4"},
            new DocumentData() {Name = "数据5"}
        };

        public ObservableCollection<DocumentData> ItemsSource
        {
            get => _itemsSource;
            set
            {
                _itemsSource = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteCommand { get; }

        /// <summary>
        /// 弹出删除确认窗口 
        /// </summary>
        public IUIDelegateAction<List<DocumentData>, MessageBoxResult> ShowDeleteWaring { get; set; } =
            new UIDelegateAction<List<DocumentData>, MessageBoxResult>();

        /// <summary>
        /// 删除动画 
        /// </summary>
        public IUIDelegateAction<List<DocumentData>> DeleteDatasAnimation { get; set; } =
            new UIDelegateAction<List<DocumentData>>();

        private async void DeleteItems_OnExecute()
        {
            if (ItemsSource.Any(i => i.IsSelected))
            {
                var selectedItems = ItemsSource.Where(i => i.IsSelected).ToList();
                var messageResult = await ShowDeleteWaring.ExecuteWithResultAsync(selectedItems);

                if (messageResult == MessageBoxResult.OK)
                {

                    /*Delete data in DB!*/

                    await DeleteDatasAnimation.ExecuteAsync(selectedItems);

                    selectedItems.ForEach(item => ItemsSource.Remove(item));

                    /*Other bussiness code!*/

                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    };

    public class DocumentData : INotifyPropertyChanged
    {
        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        private string _name = string.Empty;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
