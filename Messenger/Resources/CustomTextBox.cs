namespace Messenger.Resources
{
    using System.Windows;
    using System.Windows.Controls;
    class CustomTextBox : TextBox
    {
        public static readonly DependencyProperty CaretPositionProperty =
             DependencyProperty.Register("CaretPosition",
                                         typeof(int),
                                         typeof(CustomTextBox),
                                         new FrameworkPropertyMetadata(0,
                                                                       FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                       OnCaretPositionChanged));

        public int CaretPosition
        {
            get { return (int)GetValue(CaretPositionProperty); }
            set { SetValue(CaretPositionProperty, value); }
        }

        public CustomTextBox()
        {
            SelectionChanged += (obj, e) => CaretPosition = CaretIndex;
        }

        private static void OnCaretPositionChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            (depObj as CustomTextBox).CaretIndex = (int)e.NewValue;
        }
    }
}
