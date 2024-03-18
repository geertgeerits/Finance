namespace Finance;

public partial class PageAbout : ContentPage
{
    public PageAbout()
	{
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlert("InitializeComponent", ex.Message, "OK");
            return;
        }

        // Put text in the chosen language in the controls
        lblVersion.Text = $"{FinLang.Version_Text} 3.0.66";
        lblCopyright.Text = $"{FinLang.Copyright_Text} � 1992-2024 Geert Geerits";
        lblEmail.Text = $"{FinLang.Email_Text} geertgeerits@gmail.com";
        lblWebsite.Text = $"{FinLang.Website_Text}: ../finance";
        lblPrivacyPolicy.Text = $"\n{FinLang.PrivacyPolicyTitle_Text} {FinLang.PrivacyPolicy_Text}";
        lblLicense.Text = $"\n{FinLang.LicenseTitle_Text}: {FinLang.License_Text}\n{FinLang.LicenseMit2_Text}";
        lblExplanation.Text = $"\n{FinLang.InfoExplanation_Text}";
    }

    // Open the e-mail program
    private async void OnBtnEmailLinkClicked(object sender, EventArgs e)
    {
        if (Email.Default.IsComposeSupported)
        {
            string subject = "Finance";
            string body = "";
            string[] recipients = ["geertgeerits@gmail.com"];

            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string>(recipients)
            };

            try
            {
                await Email.Default.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                await DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
            }
        }
    }

    // Open the page 'PageWebsite' to open the website in the WebView control
    // !!!BUG!!! in Android: the WebView control gives an error when opening a link to the Google Play Console
    private async void OnBtnWebsiteLinkClicked(object sender, EventArgs e)
    {
        try
        {
#if ANDROID
            Uri uri = new("https://geertgeerits.wixsite.com/geertgeerits/finance");
            BrowserLaunchOptions options = new()
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show
            };

            await Browser.Default.OpenAsync(uri, options);
#else
            await Navigation.PushAsync(new PageWebsite());
#endif
        }
        catch (Exception ex)
        {
            await DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
        }
    }
}