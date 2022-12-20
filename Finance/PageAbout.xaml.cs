using Finance.Resources.Languages;

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
            DisplayAlert("InitializeComponent", ex.Message, MainPage.cButtonCloseText);
            return;
        }

        // Put text in the chosen language in the controls.
        lblTitle.Text = FinLang.About_Text;

        lblNameProgram.Text = FinLang.NameProgram_Text;
        lblDescription.Text = FinLang.Description_Text;
        lblVersion.Text = FinLang.Version_Text + " 3.0.53";
        lblCopyright.Text = FinLang.Copyright_Text + " © 1992-2022 Geert Geerits";
        lblEmail.Text = FinLang.Email_Text + " " + lblEmail.Text;
        lblWebsite.Text = FinLang.Website_Text + " " + lblWebsite.Text;
        lblPrivacyPolicy.Text = FinLang.PrivacyPolicyTitle_Text + " " + FinLang.PrivacyPolicy_Text;
        lblLicense.Text = FinLang.LicenseTitle_Text + ": " + FinLang.License_Text + "\n\n" + FinLang.LicenseMit2_Text;
        lblAboutExplanation.Text = FinLang.AboutExplanation_Text;
    }

    // Open e-mail program.
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
            await DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
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
                await DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
            }
        }
#endif
    }

    // Open website in default browser.
    private async void OnbtnWebsiteLinkClicked(object sender, EventArgs e)
    {
        try
        {
            Uri uri = new Uri("https://geertgeerits.wixsite.com/finance");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            await DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
        }
    }
}