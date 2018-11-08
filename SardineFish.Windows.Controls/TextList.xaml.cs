using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace SardineFish.Windows.Controls
{
    /// <summary>
    /// TextList.xaml 的交互逻辑
    /// </summary>
    public partial class List : UserControl
    {
        public List()
        {
            InitializeComponent();
        }

        public static DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IList), typeof(List));

        public IList ItemsSource
        {
            get
            {
                return (IList)GetValue(ItemsSourceProperty);
            }
            set
            {
                if (ItemsSource != null && typeof(INotifyCollectionChanged).IsAssignableFrom(ItemsSource.GetType()))
                    ((INotifyCollectionChanged)ItemsSource).CollectionChanged -= Value_CollectionChanged;
                if (typeof(INotifyCollectionChanged).IsAssignableFrom(value.GetType()))
                    ((INotifyCollectionChanged)value).CollectionChanged += Value_CollectionChanged;
                SetValue(ItemsSourceProperty, value);
                ReDraw();
            }
        }

        private void Value_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add )
            {
                foreach (var obj in e.NewItems )
                {
                    ContentPresenter cp = new ContentPresenter();
                    cp.Content = obj;
                    cp.ContentTemplate = ItemsTemplate;
                    stackPanel.Children.Add(cp);
                }
            }
            else if(e.Action ==  System.Collections.Specialized.NotifyCollectionChangedAction.Remove )
            {
                stackPanel.Children.RemoveAt(e.OldStartingIndex);
            }
            else if(e.Action ==  System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                throw new Exception("WTF?!");
            }
            else if (e.Action ==  System.Collections.Specialized.NotifyCollectionChangedAction.Replace )
            {
                for(var i=0;i<e.OldItems.Count;i++)
                {
                    ContentPresenter cp = new ContentPresenter();
                    cp.ContentTemplate = ItemsTemplate;
                    cp.Content = e.NewItems[i];
                    stackPanel.Children[i] = cp;
                }
            }
        }

        public static DependencyProperty ItemsTemplateProperty = DependencyProperty.Register("ItemsTemplate", typeof(DataTemplate), typeof(List));
        public DataTemplate ItemsTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemsTemplateProperty);
            }
            set
            {
                SetValue(ItemsTemplateProperty, value);
                ReDraw();
            }
        }

        private void ReDraw()
        {
            stackPanel.Children.Clear();
            foreach (var obj in ItemsSource )
            {
                
                ContentPresenter cp = new ContentPresenter();
                cp.Content = obj;
                cp.ContentTemplate = ItemsTemplate;
                stackPanel.Children.Add(cp);
            }
        }

        private void stackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
