using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace ScheduleEditor.Behaviors
{
    public class WindowBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty CloseWindowCommandProperty =
            DependencyProperty.Register(nameof(CloseWindowCommand), typeof(System.Windows.Input.ICommand), typeof(WindowBehavior));

        public System.Windows.Input.ICommand CloseWindowCommand
        {
            get => (System.Windows.Input.ICommand)GetValue(CloseWindowCommandProperty);
            set => SetValue(CloseWindowCommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closing += OnWindowClosing;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Closing -= OnWindowClosing;
            base.OnDetaching();
        }

        private void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CloseWindowCommand?.CanExecute(null) == true)
            {
                CloseWindowCommand.Execute(null);
            }
        }
    }
}
