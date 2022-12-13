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

        // Set the current UI culture of the selected language.
        MainPage.SetCultureSelectedLanguage();

        // Put text in the chosen language in the controls.
        lblTitle.Text = FinLang.About_Text;

        lblNameProgram.Text = FinLang.NameProgram_Text;
        lblDescription.Text = FinLang.Description_Text;
        lblVersion.Text = FinLang.Version_Text + " 3.0.52";
        lblCopyright.Text = FinLang.Copyright_Text + " © 1992-2022 Geert Geerits";
        lblEmail.Text = FinLang.Email_Text + " " + lblEmail.Text;
        lblWebsite.Text = FinLang.Website_Text + " " + lblWebsite.Text;
        lblPrivacyPolicy.Text = FinLang.PrivacyPolicyTitle_Text + " " + FinLang.PrivacyPolicy_Text;
        lblLicense.Text = FinLang.LicenseTitle_Text + ": " + FinLang.License_Text + "\n\n" + FinLang.LicenseMit2_Text;
        lblAboutExplanation.Text = FinLang.AboutExplanation_Text;
    }

    // Open e-mail program.
    private void OnbtnEmailLinkClicked(object sender, EventArgs e)
    {
        string cAddress = "geertgeerits@gmail.com";

        try
        {
            Launcher.OpenAsync(new Uri($"mailto:{cAddress}"));
        }
        catch (Exception ex)
        {
            DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
        }
    }

    // Open website.
    private void OnbtnWebsiteLinkClicked(object sender, EventArgs e)
    {
        string cUrl = "https://geertgeerits.wixsite.com/finance";

        try
        {
            Launcher.OpenAsync(new Uri(cUrl));
        }
        catch (Exception ex)
        {
            DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
        }
    }
}