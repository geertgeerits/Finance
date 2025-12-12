using Microsoft.Extensions.Logging;

#if IOS
//using System.Linq;
//using Microsoft.Maui.Platform;
//using UIKit;
#endif

namespace Finance
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
//				.ConfigureMauiHandlers((s) =>
//				{
//					Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Keyboard), (handler, view) =>
//					{
//#if IOS
//						if (handler.PlatformView is not null && view is not null)
//						{
//							var textField = handler.PlatformView;
			
//							// Force DecimalPad and attach an accessory toolbar with "+/-" and "Done"
//							textField.SetKeyboardType(UIKeyboardType.DecimalPad);

//							var toolbar = new UIToolbar
//							{
//								Translucent = true
//							};

//							// Toggle the sign of the current text
//							void ToggleSign()
//							{
//								var current = textField.Text ?? string.Empty;
//								var trimmed = current.Trim();

//								if (string.IsNullOrEmpty(trimmed))
//									return;

//								string updated = trimmed.StartsWith('-') ? trimmed[1..] : "-" + trimmed;
//								textField.Text = updated;

//								// Move cursor to the end for better UX
//								textField.SelectedTextRange = textField.GetTextRange(textField.EndOfDocument, textField.EndOfDocument);

//                                // Notify the virtual view about the text change
//                                handler.PlatformView?.UpdateText(view);
//                            }

//							// Try to focus the next UITextField / UITextView in the hierarchy
//							void FocusNextField()
//							{
//								UIView? parent = textField.Superview;
//								while (parent is not null)
//								{
//									var textInputs = parent.Subviews.Where(v => v is UITextField || v is UITextView).ToArray();
//									if (textInputs.Length > 0)
//									{
//										// Find the current index among siblings
//										int idx = System.Array.IndexOf(textInputs, textField);
//										if (idx >= 0 && idx < textInputs.Length - 1)
//										{
//											var next = textInputs[idx + 1];
//											next.BecomeFirstResponder();
//											return;
//										}
//									}

//									parent = parent.Superview;
//								}

//								// No next field found -> dismiss keyboard
//								textField.ResignFirstResponder();
//							}

//							var plusMinus = new UIBarButtonItem("+/-", UIBarButtonItemStyle.Plain, (s, e) => ToggleSign());
//							var nextButton = new UIBarButtonItem("Next", UIBarButtonItemStyle.Plain, (s, e) => FocusNextField());
//							var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
//							var done = new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, e) => textField.ResignFirstResponder());

//                            // How to add a next field button?
//                            // The 'nextButton' above will attempt to find the next UITextField/UITextView in the view hierarchy
//                            // and call BecomeFirstResponder() on it. If none is found the keyboard is dismissed.

//                            toolbar.SetItems(new[] { plusMinus, nextButton, flexible, done }, false);
//                            toolbar.SizeToFit();

//							textField.InputAccessoryView = toolbar;
//						}
//#endif
//					});
//				})
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
