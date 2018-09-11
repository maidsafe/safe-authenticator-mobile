using System.Windows.Input;
using Xamarin.Forms;

namespace CommonUtils {
  public static class ItemTappedAttached {
    public static readonly BindableProperty CommandProperty = BindableProperty.CreateAttached(
      "Command",
      typeof(ICommand),
      typeof(ListView),
      null,
      BindingMode.OneWay,
      null,
      OnItemTappedChanged);

    public static ICommand GetItemTapped(BindableObject bindable) {
      return (ICommand)bindable.GetValue(CommandProperty);
    }

    public static void OnItemTapped(object sender, ItemTappedEventArgs e) {
      var control = sender as ListView;
      var command = GetItemTapped(control);

      if (command != null && command.CanExecute(e.Item)) {
        command.Execute(e.Item);
      }
    }

    private static void OnItemTappedChanged(BindableObject bindable, object oldValue, object newValue) {
      if (bindable is ListView control) {
        control.ItemTapped += OnItemTapped;
      }
    }

    public static void SetItemTapped(BindableObject bindable, ICommand value) {
      bindable.SetValue(CommandProperty, value);
    }
  }
}
