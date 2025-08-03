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
            
            // ユーザーガイダンス: ログファイルの説明
            LoggingUtility.LogError("=== CreateShortCut アプリケーション開始 ===");
            LoggingUtility.LogError("注意: ログに表示されるfile:///形式やエンコードされたパスは、.url仕様に準拠した技術的な表記です。");
            LoggingUtility.LogError("実際のファイル作成やパス処理は正常に動作しています。");
            
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
                Console.WriteLine("================== アクセス拒否エラー ==================");
                Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                Console.WriteLine($"エラーメッセージ: {ex.Message}");
                Console.WriteLine($"発生場所: InitializeComboBox()メソッド");
                Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                Console.WriteLine("========================================================");
                
                LoggingUtility.LogError($"アクセス拒否エラー: {ex.Message}", ex);
                MessageBox.Show($"アクセス拒否: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine("============== ディレクトリ未発見エラー ================");
                Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                Console.WriteLine($"エラーメッセージ: {ex.Message}");
                Console.WriteLine($"発生場所: InitializeComboBox()メソッド");
                Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                Console.WriteLine("========================================================");
                
                LoggingUtility.LogError($"ディレクトリが見つかりません: {ex.Message}", ex);
                MessageBox.Show($"ディレクトリが見つかりません: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                Console.WriteLine("===================== IOエラー =======================");
                Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                Console.WriteLine($"エラーメッセージ: {ex.Message}");
                Console.WriteLine($"発生場所: InitializeComboBox()メソッド");
                Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                Console.WriteLine("========================================================");
                
                LoggingUtility.LogError($"IO エラー: {ex.Message}", ex);
                MessageBox.Show($"IO エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Console.WriteLine("================== 予期しないエラー ====================");
                Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                Console.WriteLine($"エラーメッセージ: {ex.Message}");
                Console.WriteLine($"発生場所: InitializeComboBox()メソッド");
                Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                Console.WriteLine("========================================================");
                
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
                // ラジオボタンの選択状態に基づいてプレフィックスを決定（角括弧を丸括弧に変更）
                string prefix = LocalRadioBtn.Checked ? "(Local)" : "(Remote)";
                
                // ローカル/リモートの候補を半角空白区切りで連結
                string suffix = LocalRadioBtn.Checked ? 
                    " local ローカル ろ ろーかる ro-karu ro- ろー" : 
                    " Remote リモート り りもーと rimo-to rimoto rimo りもー りも";
                
                // ファイル名を構築してサニタイズ
                string rawFileName = $"{prefix}{NameTxt.Text}{suffix}";
                string sanitizedFileName = SanitizeFileName(rawFileName);
                
                // URLファイルのパスを作成
                string urlFilePath = Path.Combine(shortcutPath, $"{sanitizedFileName}.url");
                
                // ショートカットファイル作成情報
                LoggingUtility.LogError($"ショートカット作成: {sanitizedFileName}.url");

                // デバッグログ: 受信したURL（エンコード前後両方表示）
                LoggingUtility.LogError($"受信URL: {url}");
                LoggingUtility.LogError($"表示用パス: {GetUserFriendlyPath(url)}");

                string finalUrl;
                
                // URLまたはfile:///形式かによって処理を分ける
                if (url.StartsWith("file:///"))
                {
                    // file:///形式の場合はそのまま使用（既に適切に変換済み）
                    finalUrl = url;
                    LoggingUtility.LogError($"file:///形式のURL使用: {finalUrl}");
                    LoggingUtility.LogError($"file:///表示用パス: {GetUserFriendlyPath(finalUrl)}");
                }
                else
                {
                    // HTTP/HTTPS URLの場合はエスケープ処理
                    finalUrl = Uri.EscapeUriString(url);
                    LoggingUtility.LogError($"HTTP/HTTPSエスケープ後URL: {finalUrl}");
                    LoggingUtility.LogError($"HTTP/HTTPS表示用URL: {GetUserFriendlyPath(finalUrl)}");
                }

                // URLファイルの内容を作成
                string urlFileContent = $"[InternetShortcut]{Environment.NewLine}URL={finalUrl}";

                // .urlファイル作成完了ログ
                LoggingUtility.LogError($".urlファイル作成完了: {urlFilePath}");

                // URLファイルを作成（Windows標準のANSIエンコーディングを使用）
                System.IO.File.WriteAllText(urlFilePath, urlFileContent, Encoding.Default);

                MessageBox.Show(".urlファイルが作成されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UriFormatException ex)
            {
                Console.WriteLine("==================== URI形式エラー ====================");
                Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                Console.WriteLine($"エラーメッセージ: {ex.Message}");
                Console.WriteLine($"発生場所: CreateShortcut()メソッド");
                Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                Console.WriteLine("========================================================");
                
                LoggingUtility.LogError($"URL形式エラー: {ex.Message}", ex);
                MessageBox.Show("URLの形式が正しくありません。\n\n" + ex.Message, "URL形式エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Console.WriteLine("================ ショートカット作成エラー ================");
                Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                Console.WriteLine($"エラーメッセージ: {ex.Message}");
                Console.WriteLine($"発生場所: CreateShortcut()メソッド");
                Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                Console.WriteLine("========================================================");
                
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
                    string displayPath = GetUserFriendlyPath(inputValue);
                    MessageBox.Show($"ローカルパスの変換に失敗しました。\n\nパス: {displayPath}", "変換エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoggingUtility.LogError($"file:///形式に変換完了: {finalUrl}");
                LoggingUtility.LogError($"変換完了表示用パス: {GetUserFriendlyPath(finalUrl)}");
            }

            // 処理済みのURLまたはfile:///形式のパスでショートカットを作成
            LoggingUtility.LogError($"ショートカット作成開始 - 最終URL: {finalUrl}");
            LoggingUtility.LogError($"ショートカット作成 - 表示用パス: {GetUserFriendlyPath(finalUrl)}");
            CreateShortcut(selectedPath, finalUrl);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Ctrl + W でアプリケーション終了（確認ダイアログ付き）
            if (keyData == (Keys.Control | Keys.W))
            {
                try
                {
                    // 確認ダイアログでアプリケーション終了確認
                    var result = MessageBox.Show(
                        "アプリケーションを終了しますか？",
                        "終了確認", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1); // はいがデフォルト
                        
                    if (result == DialogResult.Yes)
                    {
                        LoggingUtility.LogError("Ctrl+W によるアプリケーション終了");
                        Application.Exit();
                    }
                    
                    return true; // キー処理完了を通知
                }
                catch (Exception ex)
                {
                    LoggingUtility.LogError($"Ctrl+W処理エラー: {ex.Message}", ex);
                    // エラー時はフォールバック（通常終了）
                    this.Close();
                    return true;
                }
            }
            
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
            string selectedPath = null;
            
            try
            {
                // 選択されているフォルダパスを取得
                if (SaveFolderCmb.SelectedItem == null)
                {
                    MessageBox.Show("保存先フォルダが選択されていません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectedPath = SaveFolderCmb.SelectedItem.ToString();

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
                Console.WriteLine("=============== フォルダオープンエラー ================");
                Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                Console.WriteLine($"エラーメッセージ: {ex.Message}");
                Console.WriteLine($"発生場所: OpenFolderBtn_Click()メソッド");
                Console.WriteLine($"対象パス: {selectedPath ?? "取得できませんでした"}");
                Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                Console.WriteLine("========================================================");
                
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
                LoggingUtility.LogError($"変換後表示用パス: {GetUserFriendlyPath(fileUrl)}");
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
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine("============= ファイルアクセス権限エラー =================");
                        Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                        Console.WriteLine($"エラーメッセージ: {ex.Message}");
                        Console.WriteLine($"発生場所: ValidateFileAccess()メソッド（ファイルアクセス）");
                        Console.WriteLine($"対象パス: {absolutePath}");
                        Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                        Console.WriteLine("========================================================");
                        
                        errorMessage = $"ファイルへのアクセス権限がありません:\n{absolutePath}";
                        LoggingUtility.LogError($"ファイルアクセス権限エラー: {absolutePath}");
                        return false;
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine("================== ファイルIOエラー ====================");
                        Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                        Console.WriteLine($"エラーメッセージ: {ex.Message}");
                        Console.WriteLine($"発生場所: ValidateFileAccess()メソッド（ファイルアクセス）");
                        Console.WriteLine($"対象パス: {absolutePath}");
                        Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                        Console.WriteLine("========================================================");
                        
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
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine("=========== ディレクトリアクセス権限エラー ============");
                        Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                        Console.WriteLine($"エラーメッセージ: {ex.Message}");
                        Console.WriteLine($"発生場所: ValidateFileAccess()メソッド（ディレクトリアクセス）");
                        Console.WriteLine($"対象パス: {absolutePath}");
                        Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                        Console.WriteLine("========================================================");
                        
                        errorMessage = $"ディレクトリへのアクセス権限がありません:\n{absolutePath}";
                        LoggingUtility.LogError($"ディレクトリアクセス権限エラー: {absolutePath}");
                        return false;
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine("================ ディレクトリIOエラー =================");
                        Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                        Console.WriteLine($"エラーメッセージ: {ex.Message}");
                        Console.WriteLine($"発生場所: ValidateFileAccess()メソッド（ディレクトリアクセス）");
                        Console.WriteLine($"対象パス: {absolutePath}");
                        Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                        Console.WriteLine("========================================================");
                        
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
                Console.WriteLine("================= パス形式エラー ==================");
                Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                Console.WriteLine($"エラーメッセージ: {ex.Message}");
                Console.WriteLine($"発生場所: ValidateFileAccess()メソッド");
                Console.WriteLine($"対象パス: {localPath}");
                Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                Console.WriteLine("========================================================");
                
                errorMessage = $"無効なパス形式です:\n{localPath}\n{ex.Message}";
                LoggingUtility.LogError($"パス形式エラー: {localPath} - {ex.Message}", ex);
                return false;
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine("============ パス形式サポート外エラー =============");
                Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                Console.WriteLine($"エラーメッセージ: {ex.Message}");
                Console.WriteLine($"発生場所: ValidateFileAccess()メソッド");
                Console.WriteLine($"対象パス: {localPath}");
                Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                Console.WriteLine("========================================================");
                
                errorMessage = $"サポートされていないパス形式です:\n{localPath}\n{ex.Message}";
                LoggingUtility.LogError($"パス形式サポート外: {localPath} - {ex.Message}", ex);
                return false;
            }
            catch (PathTooLongException ex)
            {
                Console.WriteLine("================== パス長エラー ===================");
                Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                Console.WriteLine($"エラーメッセージ: {ex.Message}");
                Console.WriteLine($"発生場所: ValidateFileAccess()メソッド");
                Console.WriteLine($"対象パス: {localPath}");
                Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                Console.WriteLine("========================================================");
                
                errorMessage = $"パスが長すぎます:\n{localPath}\n{ex.Message}";
                LoggingUtility.LogError($"パス長エラー: {localPath} - {ex.Message}", ex);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("============ ValidateFileAccess予期しないエラー ============");
                Console.WriteLine($"エラータイプ: {ex.GetType().Name}");
                Console.WriteLine($"エラーメッセージ: {ex.Message}");
                Console.WriteLine($"発生場所: ValidateFileAccess()メソッド");
                Console.WriteLine($"対象パス: {localPath}");
                Console.WriteLine($"スタックトレース:\n{ex.StackTrace}");
                Console.WriteLine("========================================================");
                
                errorMessage = $"パス検証中に予期しないエラーが発生しました:\n{localPath}\n{ex.Message}";
                LoggingUtility.LogError($"ValidateFileAccess予期しないエラー: {localPath} - {ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// ログ表示用にエンコードされたURLを読みやすいパスに変換する
        /// </summary>
        /// <param name="url">変換対象のURL文字列</param>
        /// <returns>読みやすい形式のパス文字列</returns>
        private string GetUserFriendlyPath(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            try
            {
                // file:///形式のURLの場合はデコードして読みやすいパスに変換
                if (url.StartsWith("file:///"))
                {
                    Uri uri = new Uri(url);
                    return Uri.UnescapeDataString(uri.LocalPath);
                }
                
                // HTTPまたはHTTPS URLの場合はURLデコードのみ実行
                if (url.StartsWith("http://") || url.StartsWith("https://"))
                {
                    return Uri.UnescapeDataString(url);
                }
                
                // その他の場合はそのまま返す
                return url;
            }
            catch (Exception ex)
            {
                // デコードに失敗した場合は元の文字列を返してログに記録
                LoggingUtility.LogError($"URLデコードエラー: {url} - {ex.Message}");
                return url;
            }
        }

        /// <summary>
        /// ファイル名をWindowsファイルシステム用にサニタイズする
        /// </summary>
        /// <param name="fileName">元のファイル名</param>
        /// <returns>サニタイズされたファイル名</returns>
        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return "無題";
            }

            try
            {
                // Windowsで禁止されている文字を安全な文字に置換
                char[] invalidChars = Path.GetInvalidFileNameChars();
                string sanitized = fileName;

                foreach (char invalidChar in invalidChars)
                {
                    sanitized = sanitized.Replace(invalidChar, '_');
                }

                // 追加の問題文字を置換
                sanitized = sanitized.Replace('[', '(')
                                   .Replace(']', ')')
                                   .Replace('<', '(')
                                   .Replace('>', ')')
                                   .Replace('|', '_')
                                   .Replace('?', '_')
                                   .Replace('*', '_');

                // 先頭・末尾の空白とピリオドを削除
                sanitized = sanitized.Trim().Trim('.');

                // 空になった場合のフォールバック
                if (string.IsNullOrWhiteSpace(sanitized))
                {
                    sanitized = "無題";
                }

                // ファイル名の長さを制限（拡張子.url分を考慮して240文字）
                if (sanitized.Length > 240)
                {
                    sanitized = sanitized.Substring(0, 240);
                    LoggingUtility.LogError($"ファイル名が長すぎるため切り詰めました: {sanitized.Length}文字に短縮");
                }

                // Windowsの予約語をチェック
                string[] reservedNames = { "CON", "PRN", "AUX", "NUL", 
                    "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                    "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

                string upperName = sanitized.ToUpper();
                if (reservedNames.Contains(upperName))
                {
                    sanitized = "_" + sanitized;
                    LoggingUtility.LogError($"予約語を回避: _{sanitized}");
                }

                LoggingUtility.LogError($"ファイル名サニタイズ完了: '{fileName}' → '{sanitized}'");
                return sanitized;
            }
            catch (Exception ex)
            {
                LoggingUtility.LogError($"ファイル名サニタイズエラー: {ex.Message}", ex);
                return "無題_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            }
        }

    }
}
