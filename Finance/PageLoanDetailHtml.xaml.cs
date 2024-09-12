namespace Finance
{
    public sealed partial class PageLoanDetailHtml : ContentPage
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
#if WINDOWS
            // Set the left margin of the title for windows
            lblTitlePage.Margin = new Thickness(50, 0, 0, 0);
#endif
            // Set the WebView Source
            wvWebpage.Source = cDocumentUrl;
        }
    }
}