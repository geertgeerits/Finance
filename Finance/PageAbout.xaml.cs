namespace Finance
{
    public sealed partial class PageAbout : ContentPage
    {
        public PageAbout()
    	{
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                DisplayAlertAsync("InitializeComponent", ex.Message, "OK");
                return;
            }
#if WINDOWS
            // Set the margin of the title for windows
            lblTitlePage.Margin = new Thickness(80, 15, 0, 0);
#endif
            //// Put text in the chosen language in the controls
            lblVersion.Text = $"{FinLang.Version_Text} 3.0.70";
            lblCopyright.Text = $"{FinLang.Copyright_Text} © 1992-2026 Geert Geerits";
            lblPrivacyPolicy.Text = $"\n{FinLang.PrivacyPolicyTitle_Text} {FinLang.PrivacyPolicy_Text}";
            lblLicense.Text = $"\n{FinLang.LicenseTitle_Text}: {FinLang.License_Text}\n{FinLang.LicenseMit2_Text}";
            lblExplanation.Text = $"\n{FinLang.InfoExplanation_Text}";
            lblTrademarks.Text = $"\n{FinLang.Trademarks_Text}";
        }
    }

    /// <summary>
    /// Open e-mail app and open webpage (reusable hyperlink class)
    /// </summary>
    public sealed partial class HyperlinkSpan : Span
    {
        public static readonly BindableProperty UrlProperty =
            BindableProperty.Create(nameof(Url), typeof(string), typeof(HyperlinkSpan), null);

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        public HyperlinkSpan()
        {
            FontFamily = "OpenSansRegular";
            FontAttributes = FontAttributes.Bold;
            FontSize = 16;
            TextDecorations = TextDecorations.Underline;

            GestureRecognizers.Add(new TapGestureRecognizer
            {
                // Launcher.OpenAsync is provided by Essentials
                //Command = new Command(async () => await Launcher.OpenAsync(Url))
                Command = new Command(async () => await OpenHyperlink(Url))
            });
        }

        /// <summary>
        /// Open the e-mail program or the website link
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task OpenHyperlink(string url)
        {
            if (url.StartsWith("mailto:"))
            {
                await OpenEmailLink(url[7..]);
            }
            else
            {
                await OpenWebsiteLink(url);
            }
        }

        /// <summary>
        /// Open the e-mail program
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task OpenEmailLink(string url)
        {
            if (Email.Default.IsComposeSupported)
            {
                string subject = "Finance";
                string body = "";
                string[] recipients = [url];

                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = [.. recipients]
                };

                try
                {
                    await Email.Default.ComposeAsync(message);
                }
                catch (Exception ex)
                {
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
                }
            }
        }

        /// <summary>
        /// Open the website link in the default browser
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task OpenWebsiteLink(string url)
        {
            try
            {
                Uri uri = new(url);
                BrowserLaunchOptions options = new()
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show
                };

                await Browser.Default.OpenAsync(uri, options);
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
            }
        }
    }
}