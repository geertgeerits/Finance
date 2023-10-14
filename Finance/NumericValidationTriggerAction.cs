﻿namespace Finance
{
    public class NumericValidationTriggerAction : TriggerAction<Entry>
    {
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string BorderName { get; set; }

        protected override void Invoke(Entry entry)
        {
            bool isValid = decimal.TryParse(entry.Text, out decimal result);
            isValid = isValid && result >= MinValue && result <= MaxValue;

            Border border = (Border)entry.Parent.FindByName(BorderName);
            border.Stroke = isValid ? Color.FromArgb("969696") : Colors.Red;
        }
    }
}
