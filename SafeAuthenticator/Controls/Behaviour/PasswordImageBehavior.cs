using System;
using Xamarin.Forms;

namespace SafeAuthenticator.Controls.Behaviour
{
  public class PasswordImageBehavior : Behavior<Image> {
    public static readonly BindableProperty EntryProperty =
      BindableProperty.Create("InputEntry", typeof(PaddedEntry), typeof(PasswordImageBehavior));

    private Image _image;
    private bool _isTextMasked = true;
    private TapGestureRecognizer _tap;

    public PaddedEntry InputEntry { get => (PaddedEntry)GetValue(EntryProperty); set => SetValue(EntryProperty, value); }

    protected override void OnAttachedTo(Image image) {
      _image = image;
      _tap = new TapGestureRecognizer {NumberOfTapsRequired = 1};
      _tap.Tapped += OnTapped;
      _image.GestureRecognizers.Insert(0, _tap);
      _image.Source = "show_pass.png";
      InputEntry.IsPassword = _isTextMasked;
      base.OnAttachedTo(image);
    }

    protected override void OnDetachingFrom(Image image) {
      _image.GestureRecognizers.RemoveAt(0);
      _tap.Tapped -= OnTapped;
      base.OnDetachingFrom(image);
    }

    private void OnTapped(object sender, EventArgs eventArgs) {
      _isTextMasked = !_isTextMasked;
      if (_isTextMasked) {
        _image.Source = "show_pass.png";
        InputEntry.IsPassword = true;
      } else {
        _image.Source = "hide_pass.png";
        InputEntry.IsPassword = false;
      }
    }
  }
}
