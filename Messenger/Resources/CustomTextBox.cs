namespace Messenger.Resources
{
    using System.Windows;
    using System.Windows.Controls;

    class CustomTextBox : TextBox
    {
        #region Fields

        public static readonly DependencyProperty CaretPositionProperty =
             DependencyProperty.Register("CaretPosition",
                                         typeof(int),
                                         typeof(CustomTextBox),
                                         new FrameworkPropertyMetadata(0,
                                                                       FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                       OnCaretPositionChanged));

        #endregion //Fields

        #region Properties

        public int CaretPosition
        {
            get { return (int)GetValue(CaretPositionProperty); }
            set { SetValue(CaretPositionProperty, value); }
        }

        #endregion //Properties

        #region Constructors

        public CustomTextBox()
        {
            SelectionChanged += (obj, e) => CaretPosition = CaretIndex;
        }

        #endregion //Constructors

        #region Methods

        private static void OnCaretPositionChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            (depObj as CustomTextBox).CaretIndex = (int)e.NewValue;
        }

        #endregion //Methods
    }
}
