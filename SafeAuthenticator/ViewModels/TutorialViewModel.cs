// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

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
            Application.Current.Properties[Constants.IsTutorialComplete] = true;
            MessagingCenter.Send(
                this,
                CarouselPagePosition < 2 ? MessengerConstants.NavLoginPage : MessengerConstants.NavCreateAcctPage);
        }

        private void OnPrimaryButton()
        {
            if (CarouselPagePosition < 2)
            {
                CarouselPagePosition += 1;
            }
            else
            {
                Application.Current.Properties[Constants.IsTutorialComplete] = true;
                MessagingCenter.Send(this, MessengerConstants.NavLoginPage);
            }
        }
    }
}
