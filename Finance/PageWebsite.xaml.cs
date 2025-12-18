using System.Text.RegularExpressions;

namespace Finance;

public partial class PageWebsite : ContentPage
{
    public PageWebsite()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlertAsync("InitializeComponent: PageWebsite", ex.Message, "OK");
            return;
        }

        // Set WebView properties
        wvWebpage.Source = "https://geertgeerits.wixsite.com/geertgeerits/finance";
        wvWebpage.Navigating += OnNavigating;
        wvWebpage.Navigated += OnNavigated;
    }


    //// Navigating event that's raised when page navigation starts
    private async void OnNavigating(object? sender, WebNavigatingEventArgs e)
    {
        var url = e.Url ?? string.Empty;

        // If 'mailto' link in webpage then open the e-mail app
        if (url.StartsWith("mailto", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                _ = await Launcher.TryOpenAsync(url);
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                _ = DisplayAlertAsync(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
            }

            return;
        }

        // Handle Google Play / market / intent links that WebView can't navigate to
        if (url.StartsWith("market://", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith("intent://", StringComparison.OrdinalIgnoreCase) ||
            url.Contains("play.google.com/store", StringComparison.OrdinalIgnoreCase))
        {
            e.Cancel = true;

            try
            {
                string launchUrl = url;

                // market://details?id=com.example -> https://play.google.com/store/apps/details?id=com.example
                if (url.StartsWith("market://", StringComparison.OrdinalIgnoreCase))
                {
                    launchUrl = url.Replace("market://details?", "https://play.google.com/store/apps/details?");
                    launchUrl = launchUrl.Replace("market://", "https://play.google.com/");
                }
                // intent://...;package=com.example;... -> https://play.google.com/store/apps/details?id=com.example
                else if (url.StartsWith("intent://", StringComparison.OrdinalIgnoreCase))
                {
                    Match m = Regex.Match(url, @"package=([^;]+)");
                    if (m.Success)
                    {
                        var pkg = m.Groups[1].Value;
                        launchUrl = $"https://play.google.com/store/apps/details?id={pkg}";
                    }
                    else
                    {
                        // fallback to Play Store web front page
                        launchUrl = "https://play.google.com/store";
                    }
                }
                else if (url.Contains("play.google.com/store", StringComparison.OrdinalIgnoreCase))
                {
                    launchUrl = url;
                }

                // Try to open the Play Store app first, otherwise open browser web page
                var uri = new Uri(launchUrl);
                if (!await Launcher.TryOpenAsync(uri))
                {
                    // If TryOpenAsync with the https URL fails, still attempt with plain string
                    _ = await Launcher.TryOpenAsync(launchUrl);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                await DisplayAlertAsync($"PageWebsite, OnNavigating: {url}", ex.Message, "OK");
#endif
            }
        }
    }

    //// Navigated event that's raised when page navigation completes
    private async void OnNavigated(object? sender, WebNavigatedEventArgs e)
    {
        // Enable or disable the back and forward buttons
        btnGoBack.IsEnabled = wvWebpage.CanGoBack;
        btnGoForward.IsEnabled = wvWebpage.CanGoForward;

        // Changes the target of all the links in _self
        string result = "";

        try
        {
            result = await wvWebpage.EvaluateJavaScriptAsync(@"(function() {
                var links = document.getElementsByTagName('a');
                for (var i = 0; i < links.length; i++)
                {
                    links[i].setAttribute('target', '_self');
                }
            })()");
        }
        catch (Exception ex)
        {
#if DEBUG
            await DisplayAlertAsync($"PageWebsite, OnNavigated, result = {result}", ex.Message, "OK");
#endif        
        }
    }

    //// Go backwards, if allowed
    private void OnGoBackClicked(object sender, EventArgs e)
    {
        if (wvWebpage.CanGoBack)
        {
            wvWebpage.GoBack();
        }
    }

    //// Go forwards, if allowed
    private void OnGoForwardClicked(object sender, EventArgs e)
    {
        if (wvWebpage.CanGoForward)
        {
            wvWebpage.GoForward();
        }
    }
}