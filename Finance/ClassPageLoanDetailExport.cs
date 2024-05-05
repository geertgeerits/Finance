using static Finance.PageLoanDetail;

namespace Finance
{
    internal sealed class ClassPageLoanDetailExport
    {
        // Export loan with detail per period as a CSV file with a semicolon delimiter
        public static void ExportDetailLoanCSV(int nNumberPeriods, string cCurrency, string cDocTitle, string cFileName,
            (string lblInterestRateText, string entInterestRateText, string lblCapitalInitialText, string entCapitalInitialText,
            string lblDurationYearsText, string entDurationYearsText, string lblPeriodsYearText, string entPeriodsYearText) tLblEnt)
        {
            int nRow;

            try
            {
                using StreamWriter sw = new(cFileName, false);

                // Current date
                sw.WriteLine(DateTime.Today.ToString(Globals.cDateFormat));

                // Page number
                //sw.WriteLine(nPagNo.ToString());

                sw.WriteLine(cDocTitle);
                sw.WriteLine("");

                // Data loan
                sw.WriteLine($"{tLblEnt.lblInterestRateText} {tLblEnt.entInterestRateText} %");
                double nCapitalInitial = Convert.ToDouble(tLblEnt.entCapitalInitialText);
                sw.WriteLine($"{tLblEnt.lblCapitalInitialText} {Globals.RoundToNumDecimals(ref nCapitalInitial, Convert.ToInt32(Globals.cNumDecimalDigits), "N")} {cCurrency}");
                sw.WriteLine($"{tLblEnt.lblDurationYearsText} {tLblEnt.entDurationYearsText}");
                sw.WriteLine($"{tLblEnt.lblPeriodsYearText} {tLblEnt.entPeriodsYearText}");
                sw.WriteLine("");

                // Detail per period
                sw.WriteLine($"{aColHeader[0]};{aColHeader[1]};{aColHeader[2]};{aColHeader[3]};{aColHeader[4]};{aColHeader[5]};{aColHeader[6]}");

                for (nRow = 0; nRow < nNumberPeriods; nRow++)
                {
                    sw.WriteLine($"{aLoanDetail[nRow, 0]};{aLoanDetail[nRow, 1]};{aLoanDetail[nRow, 2]};{aLoanDetail[nRow, 3]};{aLoanDetail[nRow, 4]};{aLoanDetail[nRow, 5]};{aLoanDetail[nRow, 6]}");
                }

                // Totals
                sw.WriteLine("");
                sw.WriteLine($" ; ;{aLoanDetail[nRow, 2]};{aLoanDetail[nRow, 3]};{aLoanDetail[nRow, 4]}");

                // Close the StreamWriter object
                sw.Close();
            }
            catch (Exception ex)
            {
                _ = Application.Current.MainPage.DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
                return;
            }
        }

        // Export loan with detail per period as a HTML file
        public static void ExportDetailLoanHTML(int nNumberPeriods, string cCurrency, string cDocTitle, string cFileName,
            (string lblInterestRateText, string entInterestRateText, string lblCapitalInitialText, string entCapitalInitialText,
            string lblDurationYearsText, string entDurationYearsText, string lblPeriodsYearText, string entPeriodsYearText) tLblEnt)
        {
            int nRow;

            try
            {
                using StreamWriter sw = new(cFileName, false);

                sw.WriteLine("<!DOCTYPE html>");
                sw.WriteLine("<html>");

                sw.WriteLine("<head>");
                sw.WriteLine("<meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\"/>");
                sw.WriteLine("<title>Finance</title>");
                sw.WriteLine("<meta name=\"author\" content=\"Geert Geerits\"/>");
                sw.WriteLine("<meta name=\"created\" content=\"2022-06-06T11:18:37.52\"/>");
                sw.WriteLine("<meta name=\"changedby\" content=\"Geert Geerits\"/>");
                sw.WriteLine("<meta name=\"changed\" content=\"2022-06-09T16:11:00.00\"/>");
                sw.WriteLine("</head>");

                sw.WriteLine("<body>");

                // Current date
                sw.WriteLine($"{DateTime.Today.ToString(Globals.cDateFormat)}<br>");

                // Page number
                //sw.WriteLine(nPagNo.ToString() + "<br>");

                sw.WriteLine($"<p align=\"center\"><font size=\"4\"><b>{cDocTitle}</b></font></p>");
                sw.WriteLine("<hr>");

                // Data loan
                sw.WriteLine($"<p>{tLblEnt.lblInterestRateText} {tLblEnt.entInterestRateText} %<br>");
                double nCapitalInitial = Convert.ToDouble(tLblEnt.entCapitalInitialText);
                sw.WriteLine($"{tLblEnt.lblCapitalInitialText} {Globals.RoundToNumDecimals(ref nCapitalInitial, Convert.ToInt32(Globals.cNumDecimalDigits), "N")} {cCurrency}<br>");
                sw.WriteLine($"{tLblEnt.lblDurationYearsText} {tLblEnt.entDurationYearsText}<br>");
                sw.WriteLine($"{tLblEnt.lblPeriodsYearText} {tLblEnt.entPeriodsYearText}</p>");

                // Detail per period
                sw.WriteLine("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"0\">");
                sw.WriteLine("<col width=\"37*\"/>");
                sw.WriteLine("<col width=\"37*\"/>");
                sw.WriteLine("<col width=\"37*\"/>");
                sw.WriteLine("<col width=\"37*\"/>");
                sw.WriteLine("<col width=\"37*\"/>");
                sw.WriteLine("<col width=\"37*\"/>");
                sw.WriteLine("<col width=\"37*\"/>");

                // Column headings
                sw.WriteLine("<tr valign=\"top\">");
                sw.WriteLine($"<td width=\"6%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0.05cm; padding-bottom: 0.05cm; padding-left: 0.05cm; padding-right: 0cm\"><p align=\"right\">{aColHeader[0]}</p></td>");
                sw.WriteLine($"<td width=\"12%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">{aColHeader[1]}</p></td>");
                sw.WriteLine($"<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">{aColHeader[2]}</p></td>");
                sw.WriteLine($"<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">{aColHeader[3]}</p></td>");
                sw.WriteLine($"<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">{aColHeader[4]}</p></td>");
                sw.WriteLine($"<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">{aColHeader[5]}</p></td>");
                sw.WriteLine($"<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: 1px solid #000000; padding-top: 0.05cm; padding-bottom: 0.05cm; padding-left: 0cm; padding-right: 0.05cm\"><p align=\"right\">{aColHeader[6]}</p></td>");
                sw.WriteLine("</tr>");

                // Detail per period
                for (nRow = 0; nRow < nNumberPeriods; nRow++)
                {
                    sw.WriteLine("<tr valign=\"top\">");
                    sw.WriteLine($"<td width=\"6%\" style=\"border-top: none; border-bottom: none; border-left: 1px solid #000000; border-right: none; padding-top: 0cm; padding-bottom: 0cm; padding-left: 0.05cm; padding-right: 0cm\"><p align=\"right\">{aLoanDetail[nRow, 0]}</p></td>");
                    sw.WriteLine($"<td width=\"12%\" style=\border: none; padding: 0cm\"><p align=\"right\">{aLoanDetail[nRow, 1]}</p></td>");
                    sw.WriteLine($"<td width=\"16%\" style=\border: none; padding: 0cm\"><p align=\"right\">{aLoanDetail[nRow, 2]}</p></td>");
                    sw.WriteLine($"<td width=\"16%\" style=\border: none; padding: 0cm\"><p align=\"right\">{aLoanDetail[nRow, 3]}</p></td>");
                    sw.WriteLine($"<td width=\"16%\" style=\border: none; padding: 0cm\"><p align=\"right\">{aLoanDetail[nRow, 4]}</p></td>");
                    sw.WriteLine($"<td width=\"16%\" style=\border: none; padding: 0cm\"><p align=\"right\">{aLoanDetail[nRow, 5]}</p></td>");
                    sw.WriteLine($"<td width=\"16%\" style=\"border-top: none; border-bottom: none; border-left: none; border-right: 1px solid #000000; padding-top: 0cm; padding-bottom: 0cm; padding-left: 0cm; padding-right: 0.05cm\"><p align=\"right\">{aLoanDetail[nRow, 6]}</p></td>");
                    sw.WriteLine("</tr>");
                }

                // Totals
                sw.WriteLine("<tr valign=\"top\">");
                sw.WriteLine("<td width=\"6%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0.05cm; padding-bottom: 0.05cm; padding-left: 0.05cm; padding-right: 0cm\"><p align=\"right\"></p></td>");
                sw.WriteLine("<td width=\"12%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\"></p></td>");
                sw.WriteLine($"<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">{aLoanDetail[nRow, 2]}</p></td>");
                sw.WriteLine($"<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">{aLoanDetail[nRow, 3]}</p></td>");
                sw.WriteLine($"<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">{aLoanDetail[nRow, 4]}</p></td>");
                sw.WriteLine("<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\"></p></td>");
                sw.WriteLine("<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: 1px solid #000000; padding-top: 0.05cm; padding-bottom: 0.05cm; padding-left: 0cm; padding-right: 0.05cm\"><p align=\"right\"></p></td>");
                sw.WriteLine("</tr>");

                sw.WriteLine("</table>");
                sw.WriteLine("</body>");
                sw.WriteLine("</html>");

                // Close the StreamWriter object
                sw.Close();
            }
            catch (Exception ex)
            {
                _ = Application.Current.MainPage.DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
                return;
            }
        }

        // Export loan with detail per period as a PDF file
        public static void ExportDetailLoanPDF(int nNumberPeriods, string cCurrency, string cDocTitle, string cFileName,
            (string lblInterestRateText, string entInterestRateText, string lblCapitalInitialText, string entCapitalInitialText,
            string lblDurationYearsText, string entDurationYearsText, string lblPeriodsYearText, string entPeriodsYearText) tLblEnt)
        {
            // Counters     
            int nRow;
            int nPagNo;
            int nRowStart = 0;
            int nRowsPage;
            int nPosFirstRow;
            int nNumObjects;
            string cNoObject;
            string cNoContents;

            // Others
            string cPageSize;
            bool bPageLandscape;
            string cFontSize;
            string[] cTextLines = new string[2000];
            int nTextLine;

            // Begin and end location of the horizontal line in points
            string cHorPointsBeg;
            string cHorPointsEnd;

            // Begin and end location of the document in characters (Font: Courier ; Fontsize: 12)
            int nTabBeg12;
            int nTabEnd12;

            // Begin and end location of the document in characters (Font: Courier ; Fontsize: 10)
            int nTabBeg10;
            int nTabEnd10;

            // Column widths in characters for Portrait (Font: Courier ; Fontsize: 9 and 8)
            const int nColWidthPort0 = 7;
            const int nColWidthPort1 = 14;
            const int nColWidthPort2 = 18;
            const int nColWidthPort3 = 19;
            const int nColWidthPort4 = 19;
            const int nColWidthPort5 = 19;
            const int nColWidthPort6 = 19;

            // Column widths in characters for Landscape (Font: Courier ; Fontsize: 9 and 8)
            const int nColWidthLand0 = 7;
            const int nColWidthLand1 = 14;
            const int nColWidthLand2 = 23;
            const int nColWidthLand3 = 23;
            const int nColWidthLand4 = 23;
            const int nColWidthLand5 = 23;
            const int nColWidthLand6 = 23;

            // Portrait
            if (tLblEnt.entCapitalInitialText.Length < 14)
            {
                bPageLandscape = false;
                nRowsPage = 36;
            }
            // Landscape
            else
            {
                bPageLandscape = true;
                nRowsPage = 24;
            }

            // Number of pages
            string cPageObj = "[3 0 R";
            int nNumPages = 1;
            long[] aPosObject = new long[150];

            if (nNumberPeriods > nRowsPage)
            {
                if (nNumberPeriods % nRowsPage == 0)
                {
                    nNumPages = nNumberPeriods / nRowsPage;
                }
                else
                {
                    nNumPages = nNumberPeriods / nRowsPage + 1;
                }
            }

            // Set the page objects
            if (nNumPages > 1)
            {
                int nPage;
                int nPageObj = 7;
                for (nPage = 2; nPage < nNumPages + 1; nPage++)
                {
                    cPageObj = $"{cPageObj} {Convert.ToString(nPageObj)} 0 R";
                    nPageObj += 2;
                }
            }

            cPageObj = $"{cPageObj}] /Count {Convert.ToString(nNumPages)}";
            //DisplayAlert("cPageObj", cPageObj, "OK");

            // Format settings portrait
            if (bPageLandscape == false)
            {
                // Paper size in PostScript points A4: 595 x 842 ; Letter: 612 x 792
                if (Globals.cPageFormat == "A4")
                {
                    cPageSize = "0 0 595 842";
                    nPosFirstRow = 782;
                }
                else
                {
                    cPageSize = "0 0 612 792";
                    nPosFirstRow = 732;
                }

                // Begin and end location of a horizontal line in points
                cHorPointsBeg = "20";
                cHorPointsEnd = "575";

                // Begin and end location of the document in characters (Font: Courier ; Fontsize: 12)
                nTabBeg12 = 4;
                nTabEnd12 = 81;

                // Begin and end location of the document in characters (Font: Courier ; Fontsize: 10)
                nTabBeg10 = 4;
                nTabEnd10 = 96;

                // Fontsize (Font: Courier ; Fontsize: 8)
                cFontSize = "8";
            }
            // Format settings Landscape
            else
            {
                // Paper size in PostScript points A4: 842 x 595 ; Letter: 792 x 612
                if (Globals.cPageFormat == "A4")
                {
                    cPageSize = "0 0 842 595";
                    nPosFirstRow = 555;
                }
                else
                {
                    cPageSize = "0 0 792 612";
                    nPosFirstRow = 572;
                }

                // Begin and end location of a horizontal line in points
                cHorPointsBeg = "30";
                cHorPointsEnd = "766";

                // Begin and end location of the document in characters (Font: Courier ; Fontsize: 12)
                nTabBeg12 = 10;
                nTabEnd12 = 112;

                // Begin and end location of the document in characters (Font: Courier ; Fontsize: 10)
                nTabBeg10 = 10;
                nTabEnd10 = 132;

                // Fontsize (Font: Courier ; Fontsize: 9)
                cFontSize = "9";
            }

            int nHeight = nPosFirstRow;

            // Variables
            string cText;
            string cColText;
            string cColTextPart2;

            int nLeftPadding = (nTabEnd12 - nTabBeg12 + 1 - cDocTitle.Length) / 2;
            cDocTitle = new string(' ', nLeftPadding) + cDocTitle;
            cDocTitle = ReplaceCharacters(cDocTitle);

            // Column headings
            string cColText0Part2 = "";
            string cColText1Part2 = "";
            string cColText2Part2 = "";
            string cColText3Part2 = "";
            string cColText4Part2 = "";
            string cColText5Part2 = "";
            string cColText6Part2 = "";

            if (!bPageLandscape)
            {
                string cColText0 = SplitStringInTwo(aColHeader[0], ref cColText0Part2, nColWidthPort0);
                string cColText1 = SplitStringInTwo(aColHeader[1], ref cColText1Part2, nColWidthPort1);
                string cColText2 = SplitStringInTwo(aColHeader[2], ref cColText2Part2, nColWidthPort2);
                string cColText3 = SplitStringInTwo(aColHeader[3], ref cColText3Part2, nColWidthPort3);
                string cColText4 = SplitStringInTwo(aColHeader[4], ref cColText4Part2, nColWidthPort4);
                string cColText5 = SplitStringInTwo(aColHeader[5], ref cColText5Part2, nColWidthPort5);
                string cColText6 = SplitStringInTwo(aColHeader[6], ref cColText6Part2, nColWidthPort6);

                cColText = $"{cColText0}{cColText1}{cColText2}{cColText3}{cColText4}{cColText5}{cColText6}";
            }
            else
            {
                string cColText0 = SplitStringInTwo(aColHeader[0], ref cColText0Part2, nColWidthLand0);
                string cColText1 = SplitStringInTwo(aColHeader[1], ref cColText1Part2, nColWidthLand1);
                string cColText2 = SplitStringInTwo(aColHeader[2], ref cColText2Part2, nColWidthLand2);
                string cColText3 = SplitStringInTwo(aColHeader[3], ref cColText3Part2, nColWidthLand3);
                string cColText4 = SplitStringInTwo(aColHeader[4], ref cColText4Part2, nColWidthLand4);
                string cColText5 = SplitStringInTwo(aColHeader[5], ref cColText5Part2, nColWidthLand5);
                string cColText6 = SplitStringInTwo(aColHeader[6], ref cColText6Part2, nColWidthLand6);

                cColText = $"{cColText0}{cColText1}{cColText2}{cColText3}{cColText4}{cColText5}{cColText6}";
            }

            cColText = ReplaceCharacters(cColText);
            cColTextPart2 = $"{cColText0Part2}{cColText1Part2}{cColText2Part2}{cColText3Part2}{cColText4Part2}{cColText5Part2}{cColText6Part2}";
            cColTextPart2 = ReplaceCharacters(cColTextPart2);

            // Decoding filter: /FlateDecode ; /ASCII85Decode ; [/ASCII85Decode /FlateDecode] ; /ASCIIHexDecode ; /LZWDecode
            string cFilter = "";
            //string cFilter = " /Filter /FlateDecode";

            // Make PDF file
            try
            {
                // Start writing to PDF array
                nTextLine = 1;

                cText = "%PDF-1.7";
                cTextLines[nTextLine] = cText;
                nTextLine++;

                cText = "1 0 obj <</Type /Catalog /Pages 2 0 R>>";
                cTextLines[nTextLine] = cText;
                nTextLine++;
                cTextLines[nTextLine] = "endobj";
                nTextLine++;

                cText = $"2 0 obj <</Type /Pages /Kids {cPageObj}>>";
                cTextLines[nTextLine] = cText;
                nTextLine++;
                cTextLines[nTextLine] = "endobj";
                nTextLine++;

                cText = $"3 0 obj <</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [{cPageSize}] /Contents 6 0 R>>";
                cTextLines[nTextLine] = cText;
                nTextLine++;
                cTextLines[nTextLine] = "endobj";
                nTextLine++;

                //cText = "4 0 obj <</Font <</F1 5 0 R>>>>";
                cText = "4 0 obj <</Font <</F1 5 0 R>>/ProcSet[/PDF/Text]>>";
                cTextLines[nTextLine] = cText;
                nTextLine++;
                cTextLines[nTextLine] = "endobj";
                nTextLine++;

                // Encoding: /StandardEncoding ; /MacRomanEncoding ; /WinAnsiEncoding ; /PDFDocEncoding ; /Unicode ; /UTF-8
                cText = "5 0 obj <</Type /Font /Subtype /Type1 /BaseFont /Courier /Encoding /WinAnsiEncoding>>";
                cTextLines[nTextLine] = cText;
                nTextLine++;
                cTextLines[nTextLine] = "endobj";
                nTextLine++;

                cText = "6 0 obj";
                cTextLines[nTextLine] = cText;
                nTextLine++;

                nNumObjects = 7;

                cText = $"<</Length 0000{cFilter}>>";
                cTextLines[nTextLine] = cText;
                nTextLine++;

                // Convert string to int for number of decimal digits after decimal point
                int nNumDec = int.Parse(Globals.cNumDecimalDigits);

                for (nPagNo = 1; nPagNo < nNumPages + 1; nPagNo++)
                {
                    cTextLines[nTextLine] = "stream";
                    nTextLine++;

                    // Current date and page number
                    int nSpaceLength = nTabEnd10 - nTabBeg10 - DateTime.Today.ToString(Globals.cDateFormat).Length - nPagNo.ToString().Length;
                    string cDatePageNo = DateTime.Today.ToString(Globals.cDateFormat) + new string(' ', nSpaceLength) + nPagNo.ToString();
                    nHeight = nPosFirstRow;
                    cText = $"BT /F1 10 Tf {cHorPointsBeg} {Convert.ToString(nHeight)} Td ({cDatePageNo})Tj ET";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;

                    // Document title
                    nHeight -= 20;
                    cText = $"BT /F1 12 Tf {cHorPointsBeg} {Convert.ToString(nHeight)} Td ({cDocTitle})Tj ET";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;

                    // Horizontal line
                    nHeight -= 8;
                    cText = $"{cHorPointsBeg} {Convert.ToString(nHeight)} m {cHorPointsEnd} {Convert.ToString(nHeight)} l h S";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;

                    // Data loan
                    if (nPagNo == 1)
                    {
                        nHeight -= 12;
                        cText = $"BT /F1 10 Tf {cHorPointsBeg} {Convert.ToString(nHeight)} Td ({ReplaceCharacters(tLblEnt.lblInterestRateText)} {tLblEnt.entInterestRateText} %)Tj ET";
                        cTextLines[nTextLine] = cText;
                        nTextLine++;

                        double nCapitalInitial = Convert.ToDouble(tLblEnt.entCapitalInitialText);
                        nHeight -= 15;
                        cText = $"BT /F1 10 Tf {cHorPointsBeg} {Convert.ToString(nHeight)} Td ({ReplaceCharacters(tLblEnt.lblCapitalInitialText)} {Globals.RoundToNumDecimals(ref nCapitalInitial, nNumDec, "N")} {cCurrency})Tj ET";
                        cTextLines[nTextLine] = cText;
                        nTextLine++;

                        nHeight -= 15;
                        cText = $"BT /F1 10 Tf {cHorPointsBeg} {Convert.ToString(nHeight)} Td ({ReplaceCharacters(tLblEnt.lblDurationYearsText)} {tLblEnt.entDurationYearsText})Tj ET";
                        cTextLines[nTextLine] = cText;
                        nTextLine++;

                        nHeight -= 15;
                        cText = $"BT /F1 10 Tf {cHorPointsBeg} {Convert.ToString(nHeight)} Td ({ReplaceCharacters(tLblEnt.lblPeriodsYearText)} {tLblEnt.entPeriodsYearText})Tj ET";
                        cTextLines[nTextLine] = cText;
                        nTextLine++;

                        // Horizontal line
                        nHeight -= 5;
                        cText = $"{cHorPointsBeg} {Convert.ToString(nHeight)} m {cHorPointsEnd} {Convert.ToString(nHeight)} l h S";
                        cTextLines[nTextLine] = cText;
                        nTextLine++;
                    }

                    // Print test line to calculate the number of characters on 1 line
                    //nHeight -= 15;
                    //cText = $"BT /F1 {cFontSize} Tf {cHorPointsBeg} {Convert.ToString(nHeight)} Td (123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 )Tj ET";
                    //cTextLines[nTextLine] = cText;
                    //nTextLine++;

                    // Column heading line 1
                    nHeight -= 15;
                    cText = $"BT /F1 {cFontSize} Tf {cHorPointsBeg} {Convert.ToString(nHeight)} Td ({cColText})Tj ET";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;

                    // Column headings line 2
                    if (cColTextPart2.Trim() != "")
                    {
                        nHeight -= 10;
                        cText = $"BT /F1 {cFontSize} Tf {cHorPointsBeg} {Convert.ToString(nHeight)} Td ({cColTextPart2})Tj ET";
                        cTextLines[nTextLine] = cText;
                        nTextLine++;
                    }

                    // Horizontal line
                    nHeight -= 5;
                    cText = $"{cHorPointsBeg} {Convert.ToString(nHeight)} m {cHorPointsEnd} {Convert.ToString(nHeight)} l h S";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;

                    // Detail per period
                    for (nRow = nRowStart; nRow < nNumberPeriods; nRow++)
                    {
                        if (!bPageLandscape)
                        {
                            cText = $"{aLoanDetail[nRow, 0],nColWidthPort0}{aLoanDetail[nRow, 1],nColWidthPort1}{aLoanDetail[nRow, 2],nColWidthPort2}{aLoanDetail[nRow, 3],nColWidthPort3}{aLoanDetail[nRow, 4],nColWidthPort4}{aLoanDetail[nRow, 5],nColWidthPort5}{aLoanDetail[nRow, 6],nColWidthPort6}";
                        }
                        else
                        {
                            cText = $"{aLoanDetail[nRow, 0],nColWidthLand0}{aLoanDetail[nRow, 1],nColWidthLand1}{aLoanDetail[nRow, 2],nColWidthLand2}{aLoanDetail[nRow, 3],nColWidthLand3}{aLoanDetail[nRow, 4],nColWidthLand4}{aLoanDetail[nRow, 5],nColWidthLand5}{aLoanDetail[nRow, 6],nColWidthLand6}";
                        }

                        nHeight -= 15;
                        cText = $"BT /F1 {cFontSize} Tf {cHorPointsBeg} {Convert.ToString(nHeight)} Td ({cText})Tj ET";
                        cTextLines[nTextLine] = cText;
                        nTextLine++;

                        if (nRow == nNumberPeriods - 1)
                        {
                            break;
                        }

                        else if ((nRow + 1) % nRowsPage == 0 && nRow > nRowsPage - 2)
                        {
                            cTextLines[nTextLine] = "endstream";
                            nTextLine++;
                            cTextLines[nTextLine] = "endobj";
                            nTextLine++;

                            nRowStart = nRow + 1;

                            //DisplayAlert("nNumObjects", Convert.ToString(nNumObjects), "OK");

                            cNoObject = Convert.ToString(nNumObjects);
                            cNoContents = Convert.ToString(nNumObjects + 1);

                            cText = $"{cNoObject} 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [{cPageSize}] /Contents {cNoContents} 0 R>>";
                            cTextLines[nTextLine] = cText;
                            nTextLine++;
                            cTextLines[nTextLine] = "endobj";
                            nTextLine++;
                            nNumObjects++;

                            cNoObject = Convert.ToString(nNumObjects);
                            cTextLines[nTextLine] = $"{cNoObject} 0 obj";
                            nTextLine++;
                            nNumObjects++;

                            cText = $"<</Length 0000{cFilter}>>";
                            cTextLines[nTextLine] = cText;
                            nTextLine++;

                            break;
                        }
                    }
                }

                // End
                // Horizontal line
                nHeight -= 7;
                cText = $"{cHorPointsBeg} {Convert.ToString(nHeight)} m {cHorPointsEnd} {Convert.ToString(nHeight)} l h S";
                cTextLines[nTextLine] = cText;
                nTextLine++;

                // Totals
                if (!bPageLandscape)
                {
                    cText = $"{new string(' ', nColWidthPort0 + nColWidthPort1)}{aLoanDetail[nNumberPeriods, 2],nColWidthPort2}{aLoanDetail[nNumberPeriods, 3],nColWidthPort3}{aLoanDetail[nNumberPeriods, 4],nColWidthPort4}";
                }
                else
                {
                    cText = $"{new string(' ', nColWidthLand0 + nColWidthLand1)}{aLoanDetail[nNumberPeriods, 2],nColWidthLand2}{aLoanDetail[nNumberPeriods, 3],nColWidthLand3}{aLoanDetail[nNumberPeriods, 4],nColWidthLand4}";
                }

                nHeight -= 12;
                cText = $"BT /F1 {cFontSize} Tf {cHorPointsBeg} {Convert.ToString(nHeight)} Td ({cText})Tj ET";
                cTextLines[nTextLine] = cText;
                nTextLine++;

                // Horizontal line
                nHeight -= 7;
                cText = $"{cHorPointsBeg} {Convert.ToString(nHeight)} m {cHorPointsEnd} {Convert.ToString(nHeight)} l h S";
                cTextLines[nTextLine] = cText;
                nTextLine++;

                cTextLines[nTextLine] = "endstream";
                nTextLine++;
                cTextLines[nTextLine] = "endobj";
                nTextLine++;

                // PDF Xref table
                cTextLines[nTextLine] = "xref";
                nTextLine++;
                cTextLines[nTextLine] = $"0 {Convert.ToString(nNumObjects)}";
                nTextLine++;
                cTextLines[nTextLine] = "0000000000 65535 f ";
                nTextLine++;

                for (int nObject = 1; nObject < nNumObjects; nObject++)
                {
                    cText = "0000000000 00000 n ";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;
                }

                cTextLines[nTextLine] = $"trailer <</Size {Convert.ToString(nNumObjects)} /Root 1 0 R>>";
                nTextLine++;
                cTextLines[nTextLine] = "startxref";
                nTextLine++;
                cTextLines[nTextLine] = Convert.ToString(aPosObject[nNumObjects]);
                nTextLine++;
                cTextLines[nTextLine] = "%%EOF";

                // Count the number of characters, set the object locations and set the size of the streams
                long nNumChar = 0;
                nNumObjects = 1;
                aPosObject[0] = 0;
                int nRowStream = 0;

                // Calulate the length of the streams
                for (nRow = 1; nRow < nTextLine + 1; nRow++)
                {
                    if (cTextLines[nRow].Contains("stream") && cTextLines[nRow][..6] == "stream")
                    {
                        nNumChar = 0;
                        nRowStream = nRow;
                    }
                    else
                    {
                        nNumChar = nNumChar + cTextLines[nRow].Length + 1;
                    }

                    if (cTextLines[nRow].Contains("endstream") && cTextLines[nRow][..9] == "endstream")
                    {
                        cTextLines[nRowStream - 1] = $"<</Length {Convert.ToString(nNumChar - 10)}{cFilter}>>";
                    }
                }

                // Calulate the position of the objects and the xref
                nNumChar = 0;

                for (nRow = 1; nRow < nTextLine + 1; nRow++)
                {
                    if (cTextLines[nRow].Contains(" 0 obj"))
                    {
                        aPosObject[nNumObjects] = nNumChar + 1;
                        nNumObjects++;
                    }

                    if (cTextLines[nRow].Contains("xref") && cTextLines[nRow][..4] == "xref")
                    {
                        aPosObject[nNumObjects] = nNumChar + 1;
                        break;
                    }

                    nNumChar = nNumChar + cTextLines[nRow].Length + 1;
                }

                // Set the xref data
                nRow += 3;
                for (int nObject = 1; nObject < nNumObjects; nObject++)
                {
                    cText = new String('0', 10) + Convert.ToString(aPosObject[nObject]);
                    cText = string.Concat(cText.AsSpan(cText.Length - 10, 10), " 00000 n ");
                    cTextLines[nRow] = cText;
                    nRow++;
                }

                cTextLines[nRow + 2] = Convert.ToString(aPosObject[nNumObjects]);

                // Save the PDF file
                using StreamWriter sw = new(cFileName, false);

                for (nRow = 1; nRow < nTextLine + 1; nRow++)
                {
                    sw.WriteLine(cTextLines[nRow]);
                }

                // Close the StreamWriter object
                sw.Close();
                sw.Dispose();
            }

            catch (Exception ex)
            {
                _ = Application.Current.MainPage.DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
                return;
            }
        }

        // Replace special characters in strings for output PDF
        private static string ReplaceCharacters(string cText)
        {
            // Convert characters from UTF-8 to characters that are supported for a PDF document
            // Code: Windows-1250 ; Language: cs, hu, ro
            if ("cs;hu;ro".Contains(Globals.cLanguage))
            {
                cText = cText.Replace('ă', 'a');        // ASCII = d 227  o 343  h E3  replaced with a
                cText = cText.Replace('č', 'c');        // ASCII = d 232  o 350  h E8  replaced with c
                cText = cText.Replace('ě', 'e');        // ASCII = d 236  o 354  h EC  replaced with e
                cText = cText.Replace('ő', 'o');        // ASCII = d 245  o 365  h F5  replaced with o
                cText = cText.Replace('ț', 't');        // ASCII = d 254  o 376  h FE  replaced with t
            }

            // Code: Windows-1257 ; Language: pl
            if ("pl".Contains(Globals.cLanguage))
            {
                cText = cText.Replace('ą', 'a');        // ASCII = d 224  o 340  h E0  replaced with a
                cText = cText.Replace('ć', 'c');        // ASCII = d 227  o 343  h E3  replaced with c
                cText = cText.Replace('ł', 'l');        // ASCII = d 249  o 371  h F9  replaced with l
                cText = cText.Replace('ż', 'z');        // ASCII = d 253  o 375  h FD  replaced with z
            }

            // Loop through the text string to examen each character
            foreach (char cChar in cText)
            {
                // Get the decimal ASCII value of the character
                int nDecimalValue = (int)cChar;

                if (nDecimalValue > 127)
                {
                    // Convert char to string
                    string cCharOri = Convert.ToString(cChar);

                    // Replace the character in the string with the octal value.  Sample: ã becomes \\343
                    cText = cText.Replace(cCharOri, $"\\{Convert.ToString(nDecimalValue, 8)}");
                }
            }

            return cText;
        }

        // Split string in two parts
        private static string SplitStringInTwo(string cText, ref string cTextPart2, int nMaxTextLength)
        {
            try
            {
                int nPos = cText.LastIndexOf(' ');

                if (nPos > 0 && cText.Length > nMaxTextLength - 1)  // Decrease the maximum length by 1 so that the text of 2 columns is not connected.
                {
                    cTextPart2 = cText[(nPos + 1)..];
                    cText = cText[..nPos];
                }

                if (cText.Length > nMaxTextLength)
                {
                    cText = cText[..nMaxTextLength];
                }

                if (cTextPart2.Length > nMaxTextLength)
                {
                    cTextPart2 = cTextPart2[..nMaxTextLength];
                }

                cText = new String(' ', nMaxTextLength - cText.Length) + cText;
                cTextPart2 = new String(' ', nMaxTextLength - cTextPart2.Length) + cTextPart2;

                return cText;
            }

            catch (Exception ex)
            {
                _ = Application.Current.MainPage.DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
                return cText;
            }
        }
    }
}
