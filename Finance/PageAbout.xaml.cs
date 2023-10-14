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

        // Put text in the chosen language in the controls.
        lblVersion.Text = $"{FinLang.Version_Text} 3.0.63";
        lblCopyright.Text = $"{FinLang.Copyright_Text} © 1992-2023 Geert Geerits";
        lblEmail.Text = $"{FinLang.Email_Text} geertgeerits@gmail.com";
        lblWebsite.Text = $"{FinLang.Website_Text}: ../finance";
        lblPrivacyPolicy.Text = $"\n{FinLang.PrivacyPolicyTitle_Text} {FinLang.PrivacyPolicy_Text}";
        lblLicense.Text = $"\n{FinLang.LicenseTitle_Text}: {FinLang.License_Text}\n{FinLang.LicenseMit2_Text}";
        lblExplanation.Text = $"\n{FinLang.InfoExplanation_Text}";
    }

    // Open the e-mail program.
    private async void OnbtnEmailLinkClicked(object sender, EventArgs e)
    {
#if (IOS || MACCATALYST)
        string cAddress = "geertgeerits@gmail.com";

        try
        {
            await Launcher.OpenAsync(new Uri($"mailto:{cAddress}"));
        }
        catch (Exception ex)
        {
            await DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
        }
#else
        if (Email.Default.IsComposeSupported)
        {
            string subject = "Finance";
            string body = "";
            string[] recipients = new[] { "geertgeerits@gmail.com" };

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
#endif
    }

    // Open the page 'PageWebsite' to open the website in the WebView control.
    private async void OnbtnWebsiteLinkClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PageWebsite());
    }
}