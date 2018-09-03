using System;
using Xamarin.Forms;

namespace CommonUtils {
  public class PasswordShowHideBehavior : Behavior<Entry> {
    private Entry _entry;
    private TapGestureRecognizer _tap;

    protected override void OnAttachedTo(Entry entry) {
      _entry = entry;
      _entry.IsPassword = true;
      _tap = new TapGestureRecognizer {NumberOfTapsRequired = 3};
      _tap.Tapped += OnTapped;
      _entry.GestureRecognizers.Insert(0, _tap);
      base.OnAttachedTo(entry);
    }

    protected override void OnDetachingFrom(Entry entry) {
      _entry.GestureRecognizers.RemoveAt(0);
      _tap.Tapped -= OnTapped;
      base.OnDetachingFrom(entry);
    }

    private void OnTapped(object sender, EventArgs eventArgs) {
      _entry.IsPassword = !_entry.IsPassword;
    }
  }
}
