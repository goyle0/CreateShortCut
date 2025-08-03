using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace CreateShortCut
{
    public partial class MainForm : Form
    {
        private readonly IConfigurationService _configService;
        
        public MainForm()
        {
            _configService = new ConfigurationService();
            InitializeComponent();
            InitializeComboBox();
            LinkTxt.Focus();
            // フォームの境界スタイルをFixedSingleに設定します
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            // 最大化ボタンを無効にします
            this.MaximizeBox = false;
            // 最小化ボタンを無効にします
            this.MinimizeBox = false;
            // フォームを画面の中央に配置します
            this.StartPosition = FormStartPosition.CenterScreen;

            // TabIndexを設定します
            SaveFolderCmb.TabIndex = 0;
            LinkTxt.TabIndex = 1;
            NameTxt.TabIndex = 2;
            OpenFolderBtn.TabIndex = 3;
            SettingBtn.TabIndex = 4;
            CreateBtn.TabIndex = 5;

            // EnterキーでCreateBtn_Clickが実行されるように設定
            this.AcceptButton = CreateBtn;
        }

        private void InitializeComboBox()
        {
            string path = _configService.GetFolderPath();

            SaveFolderCmb.Items.Clear();

            try
            {
                // パスの存在確認
                if (!Directory.Exists(path))
                {
                    LoggingUtility.LogError($"指定されたフォルダパスが存在しません: {path}");
                    MessageBox.Show($"指定されたフォルダパスが存在しません: {path}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // アクセス権限の確認
                if (!HasDirectoryAccess(path))
                {
                    LoggingUtility.LogError($"フォルダパスへのアクセス権限がありません: {path}");
                    MessageBox.Show($"フォルダパスへのアクセス権限がありません: {path}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 指定したパスの下にあるすべてのディレクトリを取得します
                string[] directories = Directory.GetDirectories(path);

                // ComboBoxにディレクトリ名を追加します
                SaveFolderCmb.Items.AddRange(directories);
            }
            catch (UnauthorizedAccessException ex)
            {
                LoggingUtility.LogError($"アクセス拒否エラー: {ex.Message}", ex);
                MessageBox.Show($"アクセス拒否: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (DirectoryNotFoundException ex)
            {
                LoggingUtility.LogError($"ディレクトリが見つかりません: {ex.Message}", ex);
                MessageBox.Show($"ディレクトリが見つかりません: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                LoggingUtility.LogError($"IO エラー: {ex.Message}", ex);
                MessageBox.Show($"IO エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"予期しないエラー: {ex.Message}", ex);
                MessageBox.Show($"予期しないエラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // デフォルトパスの設定
                string defaultPath = _configService.GetDefaultPath();
                if (!string.IsNullOrEmpty(defaultPath) && Directory.Exists(defaultPath) && SaveFolderCmb.Items.Contains(defaultPath))
                {
                    SaveFolderCmb.SelectedItem = defaultPath;
                }
                else if (SaveFolderCmb.Items.Count > 0)
                {
                    SaveFolderCmb.SelectedIndex = 0;
                }
                
                // デフォルトパスが設定されていないか存在しない場合の警告
                if (string.IsNullOrEmpty(defaultPath) || !Directory.Exists(defaultPath))
                {
                    string adminWarning = IsRunningAsAdministrator() ? "" : "\n\nなお、設定の保存には管理者権限が必要です。このアプリを管理者として再起動することをお勧めします。";
                    LoggingUtility.LogError($"デフォルトパスが設定されていないか、存在しません: {defaultPath}");
                    MessageBox.Show($"デフォルトパスが設定されていないか、存在しません。\n設定画面でデフォルトパスを設定してください。{adminWarning}", "設定が必要", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void CreateShortcut(string shortcutPath, string url)
        {
            try
            {
                // URLファイルのパスを作成
                string urlFilePath = Path.Combine(shortcutPath, $"{NameTxt.Text}.url");

                // デバッグログ: 受信したURL
                LoggingUtility.LogError($"受信URL: {url}");

                string finalUrl;
                
                // URLまたはfile:///形式かによって処理を分ける
                if (url.StartsWith("file:///"))
                {
                    // file:///形式の場合はそのまま使用（既に適切に変換済み）
                    finalUrl = url;
                    LoggingUtility.LogError($"file:///形式のURL使用: {finalUrl}");
                }
                else
                {
                    // HTTP/HTTPS URLの場合はエスケープ処理
                    finalUrl = Uri.EscapeUriString(url);
                    LoggingUtility.LogError($"HTTP/HTTPSエスケープ後URL: {finalUrl}");
                }

                // URLファイルの内容を作成
                string urlFileContent = $"[InternetShortcut]{Environment.NewLine}URL={finalUrl}";

                // デバッグログ: 保存される内容
                LoggingUtility.LogError($"保存内容: {urlFileContent}");

                // URLファイルを作成（Windows標準のANSIエンコーディングを使用）
                System.IO.File.WriteAllText(urlFilePath, urlFileContent, Encoding.Default);

                LoggingUtility.LogError($".urlファイルが正常に作成されました: {urlFilePath}");
                MessageBox.Show(".urlファイルが作成されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UriFormatException ex)
            {
                LoggingUtility.LogError($"URL形式エラー: {ex.Message}", ex);
                MessageBox.Show("URLの形式が正しくありません。\n\n" + ex.Message, "URL形式エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"ショートカット作成エラー: {ex.Message}", ex);
                MessageBox.Show("ショートカットの作成中にエラーが発生しました。\n\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateBtn_Click(object sender, EventArgs e)
        {
            if (SaveFolderCmb.SelectedItem == null)
            {
                MessageBox.Show("保存先フォルダを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string selectedPath = SaveFolderCmb.SelectedItem.ToString();

            if (string.IsNullOrEmpty(LinkTxt.Text) || string.IsNullOrEmpty(NameTxt.Text))
            {
                MessageBox.Show("リンクまたはパス、およびショートカット名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string inputValue = LinkTxt.Text.Trim();
            
            // 入力値の種類を判定
            bool? inputType = IsUrlOrPath(inputValue);
            if (inputType == null)
            {
                MessageBox.Show("入力された値が正しくありません。\n\n対応形式:\n・URL: https://example.com\n・ローカルパス: C:\\Users\\user\\Documents\\file.txt\n・相対パス: .\\folder\\file.txt", "入力形式エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string finalUrl;

            if (inputType == true) // URL形式
            {
                LoggingUtility.LogError($"URL形式として処理: {inputValue}");
                
                // URLの場合はそのまま使用（CreateShortcutメソッド内でエスケープ処理される）
                finalUrl = inputValue;
            }
            else // ローカルパス形式
            {
                LoggingUtility.LogError($"ローカルパス形式として処理: {inputValue}");
                
                // ローカルパスの存在とアクセス性を検証
                if (!ValidateFileAccess(inputValue, out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "ファイル/ディレクトリアクセスエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ローカルパスをfile:///形式に変換
                finalUrl = ConvertToValidUrl(inputValue);
                if (string.IsNullOrEmpty(finalUrl))
                {
                    MessageBox.Show("ローカルパスのfile:///形式への変換に失敗しました。\n\nパス: " + inputValue, "変換エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoggingUtility.LogError($"file:///形式に変換完了: {finalUrl}");
            }

            // 処理済みのURLまたはfile:///形式のパスでショートカットを作成
            LoggingUtility.LogError($"ショートカット作成開始 - 最終URL: {finalUrl}");
            CreateShortcut(selectedPath, finalUrl);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void FolderBtn_Click(object sender, EventArgs e)
        {
            // SettingFormをインスタンス化して表示する
            using (var settingForm = new SettingForm())
            {
                settingForm.ShowDialog();
                InitializeComboBox(); // 画面情報を更新するためにInitializeComboBoxメソッドを呼び出す
                LinkTxt.Focus();
            }
        }

        private void OpenFolderBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // 選択されているフォルダパスを取得
                if (SaveFolderCmb.SelectedItem == null)
                {
                    MessageBox.Show("保存先フォルダが選択されていません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedPath = SaveFolderCmb.SelectedItem.ToString();

                // フォルダの存在確認
                if (!Directory.Exists(selectedPath))
                {
                    LoggingUtility.LogError($"選択されたフォルダが存在しません: {selectedPath}");
                    MessageBox.Show($"選択されたフォルダが存在しません:\n{selectedPath}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // エクスプローラーでフォルダを開く
                Process.Start("explorer.exe", selectedPath);
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"フォルダを開く際にエラーが発生しました: {ex.Message}", ex);
                MessageBox.Show($"フォルダを開く際にエラーが発生しました:\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool HasDirectoryAccess(string path)
        {
            try
            {
                Directory.GetDirectories(path);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }


        private bool IsRunningAsAdministrator()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 入力値がURL（http/https）かローカルパスかを判定する
        /// </summary>
        /// <param name="input">ユーザーの入力値</param>
        /// <returns>URL:true, ローカルパス:false, 不正:null</returns>
        private bool? IsUrlOrPath(string input)
        {
            try
            {
                // 空または空白の場合は不正
                if (string.IsNullOrWhiteSpace(input))
                {
                    LoggingUtility.LogError($"入力が空または空白です: '{input}'");
                    return null;
                }

                // HTTPまたはHTTPS URLの場合
                if (Uri.TryCreate(input, UriKind.Absolute, out Uri uriResult))
                {
                    if (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
                    {
                        LoggingUtility.LogError($"URL形式として認識: {input}");
                        return true;
                    }
                }

                // ローカルパスの判定
                // 1. Windowsの絶対パス（C:\...）
                if (Path.IsPathRooted(input))
                {
                    LoggingUtility.LogError($"絶対パスとして認識: {input}");
                    return false;
                }

                // 2. 相対パス（.\... または ..\...）
                if (input.StartsWith(".\\") || input.StartsWith("../") || input.StartsWith(".\\"))
                {
                    LoggingUtility.LogError($"相対パスとして認識: {input}");
                    return false;
                }

                // 3. UNCパス（\\...）
                if (input.StartsWith("\\\\"))
                {
                    LoggingUtility.LogError($"UNCパスとして認識: {input}");
                    return false;
                }

                // 4. ファイル名のみの場合（拡張子があるか、一般的なファイル名パターン）
                string extension = Path.GetExtension(input);
                if (!string.IsNullOrEmpty(extension) || input.Contains("\\") || input.Contains("/"))
                {
                    LoggingUtility.LogError($"ファイルパスとして認識: {input}");
                    return false;
                }

                // どちらでもない場合は不正
                LoggingUtility.LogError($"不正な入力形式: {input}");
                return null;
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"IsUrlOrPath判定エラー: {ex.Message}", ex);
                return null;
            }
        }

        /// <summary>
        /// ローカルパスをfile:///形式のURIに変換する
        /// </summary>
        /// <param name="localPath">ローカルファイルパス</param>
        /// <returns>file:///形式のURI、失敗時はnull</returns>
        private string ConvertToValidUrl(string localPath)
        {
            try
            {
                // 相対パスの場合は絶対パスに変換
                string absolutePath;
                if (Path.IsPathRooted(localPath))
                {
                    absolutePath = localPath;
                }
                else
                {
                    absolutePath = Path.GetFullPath(localPath);
                }

                // パスの正規化（バックスラッシュを統一、重複スラッシュを除去）
                absolutePath = Path.GetFullPath(absolutePath);

                // file:///形式のURIに変換
                Uri fileUri = new Uri(absolutePath);
                string fileUrl = fileUri.AbsoluteUri;

                LoggingUtility.LogError($"ローカルパス変換: {localPath} → {fileUrl}");
                return fileUrl;
            }
            catch (ArgumentException ex)
            {
                LoggingUtility.LogError($"パス引数エラー: {localPath} - {ex.Message}", ex);
                return null;
            }
            catch (NotSupportedException ex)
            {
                LoggingUtility.LogError($"パス形式サポート外: {localPath} - {ex.Message}", ex);
                return null;
            }
            catch (PathTooLongException ex)
            {
                LoggingUtility.LogError($"パスが長すぎます: {localPath} - {ex.Message}", ex);
                return null;
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"ConvertToValidUrl変換エラー: {localPath} - {ex.Message}", ex);
                return null;
            }
        }

        /// <summary>
        /// ローカルパスの存在とアクセス性を検証する
        /// </summary>
        /// <param name="localPath">検証するローカルパス</param>
        /// <param name="errorMessage">エラーメッセージ（出力パラメータ）</param>
        /// <returns>検証成功:true、失敗:false</returns>
        private bool ValidateFileAccess(string localPath, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                // 絶対パスに変換
                string absolutePath;
                if (Path.IsPathRooted(localPath))
                {
                    absolutePath = localPath;
                }
                else
                {
                    absolutePath = Path.GetFullPath(localPath);
                }

                // ファイルまたはディレクトリの存在確認
                bool isFile = File.Exists(absolutePath);
                bool isDirectory = Directory.Exists(absolutePath);

                if (!isFile && !isDirectory)
                {
                    errorMessage = $"指定されたファイルまたはディレクトリが存在しません:\n{absolutePath}";
                    LoggingUtility.LogError($"パス存在確認失敗: {absolutePath}");
                    return false;
                }

                // ファイルの場合のアクセス権限確認
                if (isFile)
                {
                    try
                    {
                        // ファイルへの読み取りアクセステスト
                        using (FileStream fs = File.OpenRead(absolutePath))
                        {
                            // 正常にアクセスできることを確認
                        }
                        LoggingUtility.LogError($"ファイルアクセス確認成功: {absolutePath}");
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

                // ディレクトリの場合のアクセス権限確認
                if (isDirectory)
                {
                    try
                    {
                        // ディレクトリへのアクセステスト
                        Directory.GetFiles(absolutePath);
                        LoggingUtility.LogError($"ディレクトリアクセス確認成功: {absolutePath}");
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

                LoggingUtility.LogError($"パス検証成功: {absolutePath} (ファイル:{isFile}, ディレクトリ:{isDirectory})");
                return true;
            }
            catch (ArgumentException ex)
            {
                errorMessage = $"無効なパス形式です:\n{localPath}\n{ex.Message}";
                LoggingUtility.LogError($"パス形式エラー: {localPath} - {ex.Message}", ex);
                return false;
            }
            catch (NotSupportedException ex)
            {
                errorMessage = $"サポートされていないパス形式です:\n{localPath}\n{ex.Message}";
                LoggingUtility.LogError($"パス形式サポート外: {localPath} - {ex.Message}", ex);
                return false;
            }
            catch (PathTooLongException ex)
            {
                errorMessage = $"パスが長すぎます:\n{localPath}\n{ex.Message}";
                LoggingUtility.LogError($"パス長エラー: {localPath} - {ex.Message}", ex);
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"パス検証中に予期しないエラーが発生しました:\n{localPath}\n{ex.Message}";
                LoggingUtility.LogError($"ValidateFileAccess予期しないエラー: {localPath} - {ex.Message}", ex);
                return false;
            }
        }
    }
}
