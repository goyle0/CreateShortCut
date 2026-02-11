using System;
using System.IO;

namespace CreateShortCut.Utilities
{
    public static class PathUtility
    {
        public static string ResolveAbsolutePath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            return Path.GetFullPath(path);
        }

        public static bool ValidateAccess(string localPath, out string errorMessage)
        {
            errorMessage = null;

            string absolutePath;
            try
            {
                absolutePath = ResolveAbsolutePath(localPath);
            }
            catch (ArgumentException ex)
            {
                errorMessage = $"パスに無効な文字が含まれています:\n{localPath}";
                LoggingUtility.LogError($"パス引数エラー: {localPath} - {ex.Message}", ex);
                return false;
            }
            catch (NotSupportedException ex)
            {
                errorMessage = $"サポートされていないパス形式です:\n{localPath}";
                LoggingUtility.LogError($"パス形式サポート外: {localPath} - {ex.Message}", ex);
                return false;
            }
            catch (PathTooLongException ex)
            {
                errorMessage = $"パスが長すぎます:\n{localPath}";
                LoggingUtility.LogError($"パスが長すぎます: {localPath} - {ex.Message}", ex);
                return false;
            }

            bool isFile = File.Exists(absolutePath);
            bool isDirectory = Directory.Exists(absolutePath);

            if (!isFile && !isDirectory)
            {
                errorMessage = $"指定されたファイルまたはディレクトリが存在しません:\n{absolutePath}";
                LoggingUtility.LogError($"パス存在確認失敗: {absolutePath}");
                return false;
            }

            if (isFile)
            {
                try
                {
                    using (FileStream fs = File.OpenRead(absolutePath))
                    {
                    }
                    LoggingUtility.LogDebug($"ファイルアクセス確認成功: {absolutePath}");
                }
                catch (UnauthorizedAccessException)
                {
                    errorMessage = $"ファイルへのアクセス権限がありません:\n{absolutePath}";
                    LoggingUtility.LogError($"ファイルアクセス権限エラー: {absolutePath}");
                    return false;
                }
                catch (IOException ex)
                {
                    errorMessage = $"ファイルアクセス中にIOエラーが発生しました:\n{absolutePath}\n{ex.Message}";
                    LoggingUtility.LogError($"ファイルIOエラー: {absolutePath} - {ex.Message}", ex);
                    return false;
                }
            }

            if (isDirectory)
            {
                try
                {
                    Directory.GetFiles(absolutePath);
                    LoggingUtility.LogDebug($"ディレクトリアクセス確認成功: {absolutePath}");
                }
                catch (UnauthorizedAccessException)
                {
                    errorMessage = $"ディレクトリへのアクセス権限がありません:\n{absolutePath}";
                    LoggingUtility.LogError($"ディレクトリアクセス権限エラー: {absolutePath}");
                    return false;
                }
                catch (IOException ex)
                {
                    errorMessage = $"ディレクトリアクセス中にIOエラーが発生しました:\n{absolutePath}\n{ex.Message}";
                    LoggingUtility.LogError($"ディレクトリIOエラー: {absolutePath} - {ex.Message}", ex);
                    return false;
                }
            }

            LoggingUtility.LogDebug($"パス検証成功: {absolutePath} (ファイル:{isFile}, ディレクトリ:{isDirectory})");
            return true;
        }

        public static bool? ClassifyInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                LoggingUtility.LogDebug($"入力が空または空白です: '{input}'");
                return null;
            }

            // HTTPまたはHTTPS URLの場合
            if (Uri.TryCreate(input, UriKind.Absolute, out Uri uriResult))
            {
                if (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
                {
                    LoggingUtility.LogDebug($"URL形式として認識: {input}");
                    return true;
                }
            }

            // Windowsの絶対パス
            if (Path.IsPathRooted(input))
            {
                LoggingUtility.LogDebug($"絶対パスとして認識: {input}");
                return false;
            }

            // 相対パス（.\, ./, ..\, ../）
            if (input.StartsWith(".\\") || input.StartsWith("./") || input.StartsWith("..\\") || input.StartsWith("../"))
            {
                LoggingUtility.LogDebug($"相対パスとして認識: {input}");
                return false;
            }

            // UNCパス
            if (input.StartsWith("\\\\"))
            {
                LoggingUtility.LogDebug($"UNCパスとして認識: {input}");
                return false;
            }

            // ファイル名のみの場合
            string extension = Path.GetExtension(input);
            if (!string.IsNullOrEmpty(extension) || input.Contains("\\") || input.Contains("/"))
            {
                LoggingUtility.LogDebug($"ファイルパスとして認識: {input}");
                return false;
            }

            LoggingUtility.LogDebug($"不正な入力形式: {input}");
            return null;
        }
    }
}
