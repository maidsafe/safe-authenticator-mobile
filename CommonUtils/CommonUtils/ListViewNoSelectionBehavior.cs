using Xamarin.Forms;

namespace CommonUtils {
  public class ListViewNoSelectionBehavior : Behavior<ListView> {
    protected override void OnAttachedTo(ListView listview) {
      listview.ItemSelected += OnItemSelected;
      base.OnAttachedTo(listview);
    }

    protected override void OnDetachingFrom(ListView listview) {
      listview.ItemSelected -= OnItemSelected;
      base.OnDetachingFrom(listview);
    }

    private static void OnItemSelected(object sender, SelectedItemChangedEventArgs args) {
      ((ListView)sender).SelectedItem = null;
    }
  }
}
