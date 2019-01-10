namespace SafeAuthenticator.Models
{
    public class StrengthIndicator
    {
        public string Strength { get; set; }

        public double Percentage { get; set; }

        public double Guesses { get; set; }

        public StrengthIndicator()
        {
            Strength = string.Empty;
            Percentage = 0;
            Guesses = 0;
        }
    }
}
