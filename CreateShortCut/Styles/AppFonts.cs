using System.Drawing;

namespace CreateShortCut.Styles
{
    public static class AppFonts
    {
        private const string FontFamily = "Yu Gothic UI";

        public static readonly Font Label = new Font(FontFamily, 12F, FontStyle.Regular);
        public static readonly Font Input = new Font(FontFamily, 12F, FontStyle.Regular);
        public static readonly Font ComboBox = new Font(FontFamily, 12F, FontStyle.Regular);
        public static readonly Font Button = new Font(FontFamily, 12F, FontStyle.Regular);
        public static readonly Font GroupBox = new Font(FontFamily, 11F, FontStyle.Regular);
        public static readonly Font RadioButton = new Font(FontFamily, 11F, FontStyle.Regular);
    }
}
