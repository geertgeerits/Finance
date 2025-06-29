namespace Finance
{
    public sealed class ValidationTriggerActionDecimal : TriggerAction<Entry>
    {
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }

        protected override void Invoke(Entry entry)
        {
            // Validate the number
            bool isValid = decimal.TryParse(entry.Text, out decimal result);
            isValid = isValid && result >= MinValue && result <= MaxValue;

            // Set the border color if the input is invalid
            Border border = (Border)entry.Parent;

            if (border != null)
            {
                if (entry.Text.Contains(ClassEntryMethods.cNumGroupSeparator))
                {
                    border.Stroke = Color.FromArgb("969696");
                }
                else
                {
                    border.Stroke = isValid ? Color.FromArgb("969696") : Colors.OrangeRed;
                }
            }
        }
    }
}
