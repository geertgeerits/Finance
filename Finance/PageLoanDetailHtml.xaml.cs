namespace Finance;

public partial class PageLoanDetailHtml : ContentPage
{
    public PageLoanDetailHtml(string cDocumentUrl)
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlert("InitializeComponent: PageWebsite", ex.Message, "OK");
            return;
        }
        
        // Set the WebView Source.
        wvWebpage.Source = cDocumentUrl;
    }
}