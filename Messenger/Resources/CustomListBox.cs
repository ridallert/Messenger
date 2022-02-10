namespace Messenger.Resources
{
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    public class CustomListBox : ListBox
    {
        #region Fields

        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.Register("AutoScroll",
                                        typeof(bool),
                                        typeof(CustomListBox),
                                        new FrameworkPropertyMetadata(true,
                                                                      FrameworkPropertyMetadataOptions.AffectsArrange |
                                                                      FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                      AutoScrollPropertyChanged));

        #endregion //Fields

        #region Properties

        [Category("Common")]
        public bool AutoScroll
        {
            get { return (bool)GetValue(AutoScrollProperty); }
            set { SetValue(AutoScrollProperty, value); }
        }

        #endregion //Properties

        #region Methods

        public CustomListBox()
        {
            SubscribeToAutoScrollItemsCollectionChanged(this, (bool)AutoScrollProperty.DefaultMetadata.DefaultValue);
        }

        private static void AutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SubscribeToAutoScrollItemsCollectionChanged((CustomListBox)d, (bool)e.NewValue);
        }

        private static void SubscribeToAutoScrollItemsCollectionChanged(CustomListBox listBox, bool subscribe)
        {
            INotifyCollectionChanged notifyCollection = listBox.Items.SourceCollection as INotifyCollectionChanged;

            if (notifyCollection != null)
            {
                if (subscribe)
                {
                    notifyCollection.CollectionChanged += listBox.AutoScrollItemsCollectionChanged;
                }
                else
                {
                    notifyCollection.CollectionChanged -= listBox.AutoScrollItemsCollectionChanged;
                }
            }
        }

        private void AutoScrollItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int count = Items.Count;
                ScrollIntoView(Items[count - 1]);
            }
        }

        #endregion //Methods
    }
}
