using System.Drawing;

namespace CreateShortCut.Styles
{
    public static class AppColors
    {
        // Windows 11 Background
        public static readonly Color Background = ColorTranslator.FromHtml("#F9F9F9");
        // Text Color
        public static readonly Color Text = ColorTranslator.FromHtml("#202020");
        // Border Color
        public static readonly Color Border = ColorTranslator.FromHtml("#CCCCCC");

        // Button Colors - all distinct
        public static readonly Color Primary = ColorTranslator.FromHtml("#0078D4");       // Blue - 作成/適用
        public static readonly Color Setting = ColorTranslator.FromHtml("#107C10");       // Green - 設定
        public static readonly Color OpenFolder = ColorTranslator.FromHtml("#CA5010");    // Orange - フォルダを開く
        public static readonly Color Reference = ColorTranslator.FromHtml("#6B69D6");     // Purple - フォルダ参照
    }
}
