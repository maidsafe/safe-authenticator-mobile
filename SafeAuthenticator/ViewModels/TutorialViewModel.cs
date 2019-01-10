using System.Windows.Input;
using SafeAuthenticator.Helpers;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    class TutorialViewModel : BaseViewModel
    {
        private int _carouselPagePosition;
        private string _primaryButtonText;
        private string _secondaryButtonText;

        public string PrimaryButtonText
        {
            get => _primaryButtonText;
            set => SetProperty(ref _primaryButtonText, value);
        }

        public string SecondaryButtonText
        {
            get => _secondaryButtonText;
            set => SetProperty(ref _secondaryButtonText, value);
        }

        public int CarouselPagePosition
        {
            get => _carouselPagePosition;
            set => SetProperty(ref _carouselPagePosition, value);
        }

        public ICommand SecondaryButtonCommand { get; }

        public ICommand PrimaryButtonCommand { get; }

        public ICommand CarouselPageChangeCommand { get; }

        public TutorialViewModel()
        {
            CarouselPageChangeCommand = new Command(OnCarouselPageChange);
            SecondaryButtonCommand = new Command(OnSecondaryButton);
            PrimaryButtonCommand = new Command(OnPrimaryButton);
            SecondaryButtonText = "LOGIN";
            PrimaryButtonText = "GET STARTED";
        }

        private void OnCarouselPageChange()
        {
            switch (CarouselPagePosition)
            {
                case 0:
                    SecondaryButtonText = "LOGIN";
                    PrimaryButtonText = "GET STARTED";
                    break;
                case 1:
                    SecondaryButtonText = "SKIP";
                    PrimaryButtonText = "CONTINUE";
                    break;
                case 2:
                    SecondaryButtonText = "CREATE ACCOUNT";
                    PrimaryButtonText = "LOGIN";
                    break;
            }
        }

        private void OnSecondaryButton()
        {
            if (CarouselPagePosition < 2)
                MessagingCenter.Send(this, MessengerConstants.NavLoginPage);
            else
                MessagingCenter.Send(this, MessengerConstants.NavCreateAcctPage);
        }

        private void OnPrimaryButton()
        {
            if (CarouselPagePosition < 2)
                CarouselPagePosition += 1;
            else
                MessagingCenter.Send(this, MessengerConstants.NavLoginPage);
        }
    }
}
