// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace SafeAuthenticator.Controls.Converters
{
    public class HtmlLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;

            var formatted = new FormattedString();

            foreach (var item in ProcessString((string)value))
                formatted.Spans.Add(CreateSpan(item));

            return formatted;
        }

        private Span CreateSpan(StringSection section)
        {
            var span = new Span()
            {
                Text = section.Text
            };

            if (!string.IsNullOrEmpty(section.Link))
            {
                span.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = _navigationCommand,
                    CommandParameter = section.Link
                });
                span.TextColor = Color.Blue;
                span.TextDecorations = TextDecorations.Underline;
            }

            return span;
        }

        public IList<StringSection> ProcessString(string rawText)
        {
            const string spanPattern = @"(<a.*?>.*?</a>)";

            MatchCollection collection = Regex.Matches(rawText, spanPattern, RegexOptions.Singleline);

            var sections = new List<StringSection>();

            var lastIndex = 0;

            foreach (Match item in collection)
            {
                var foundText = item.Value;
                sections.Add(new StringSection() { Text = rawText.Substring(lastIndex, item.Index) });
                lastIndex += item.Index + item.Length;

                // Get HTML href
                var html = new StringSection()
                {
                    Link = Regex.Match(item.Value, "(?<=href=\\\")[\\S]+(?=\\\")").Value,
                    Text = Regex.Replace(item.Value, "<.*?>", string.Empty)
                };

                sections.Add(html);
            }

            sections.Add(new StringSection() { Text = rawText.Substring(lastIndex) });

            return sections;
        }

        public class StringSection
        {
            public string Text { get; set; }

            public string Link { get; set; }
        }

        private ICommand _navigationCommand = new Command<string>((url) =>
        {
            DependencyService.Get<INativeBrowserService>().LaunchNativeEmbeddedBrowser(url);
        });

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
