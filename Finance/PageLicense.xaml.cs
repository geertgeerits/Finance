using Finance.Resources.Languages;

namespace Finance;

public partial class PageLicense : ContentPage
{
	public PageLicense()
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
        lblTitle.Text = FinLang.LicenseTitle_Text;
        
        lblNameProgram.Text = FinLang.NameProgram_Text;
        lblCopyright.Text = FinLang.Copyright_Text + " © 1992-2022 Geert Geerits";
        lblLicense.Text = FinLang.License_Text;
        btnAgree.Text = FinLang.Agree_Text;
        btnDisagree.Text = FinLang.Disagree_Text;

        // Remove or disable the back button (not possible for now!!!).
    }

    // Button btnAgree clicked event.
    private void OnButtonAgreeClicked(object sender, EventArgs e)
    {
        // Save the licence setting.
        Preferences.Default.Set("SettingLicense", true);

        //Close the license page and return to the root page.
        Navigation.PopToRootAsync();
    }

    // Button btnDisagree clicked event.
    private void OnButtonDisagreeClicked(object sender, EventArgs e)
    {
        // Save the licence setting.
        Preferences.Default.Set("SettingLicense", false);

        // Close the application.
        Application.Current.Quit();
    }
}