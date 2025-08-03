# CLAUDE.md

このファイルは、このリポジトリでコードを操作する際にClaude Code (claude.ai/code) にガイダンスを提供します。

## プロジェクト概要

CreateShortCutは、C#（.NET Framework 4.8）で書かれたWindows Formsアプリケーションです。指定されたディレクトリにURLショートカット（.urlファイル）を作成し、設定可能なデフォルトパスで異なるフォルダにショートカットを整理できます。

## ビルドコマンド

- **ソリューションのビルド**: Visual Studioを使用するか、コマンドラインから `MSBuild CreateShortCut.sln` を実行
- **デバッグビルド**: `CreateShortCut\bin\Debug\` に出力
- **リリースビルド**: `CreateShortCut\bin\Release\` に出力
- **インストーラー**: ソリューションにはMSIインストーラーを作成するVisual Studio配置プロジェクト（`ShortCutSetup.vdproj`）が含まれています

## アーキテクチャ

アプリケーションは2つのメインフォームを持つシンプルなWindows Formsアーキテクチャに従っています：

- **MainForm**: ショートカット作成のプライマリインターフェース
  - 設定から読み込まれたComboBoxを使用したフォルダ選択管理
  - Windows Internet Shortcut形式を使用した.urlファイル作成
  - 入力検証とエラーログ処理
  - 日本語文字サポートのためのShift-JISエンコーディング使用
  - **デフォルトパス存在チェック機能**: 起動時にApp.config内のDefaultPathの存在を確認し、未設定または存在しない場合は適切なガイダンスメッセージを表示
  - **管理者権限状況判定**: IsRunningAsAdministrator()メソッドにより現在の実行権限を判定し、設定変更に必要な管理者権限の案内を動的に表示

- **SettingForm**: フォルダパスの設定インターフェース
  - フォルダパスとデフォルト選択でApp.configファイルを更新
  - ディレクトリアクセス権限の検証
  - **管理者権限事前チェック**: 設定保存前にIsRunningAsAdministrator()メソッドで管理者権限を確認し、権限不足時は保存処理を中止して適切なエラーメッセージを表示
  - **ファイル書き込み権限検証**: App.configファイルへの書き込み権限をHasFileWriteAccess()メソッドで事前確認

## 主要な技術詳細

- **設定**: App.configに`FolderPath`と`DefaultPath`設定を保存
- **エンコーディング**: 日本語テキストサポートのため全体でShift-JISエンコーディングを使用
  - .urlファイル保存: Shift-JISエンコーディング（Issue #10で統一）
  - ログ出力: Shift-JISエンコーディング  
  - UI表示: .NET FrameworkのUnicode標準処理
  - App.config: UTF-8エンコーディング
- **COM参照**: WindowsショートカットファンクションのためIWshRuntimeLibraryを使用
- **エラー処理**: 複数のフォールバックログ場所を持つ包括的なエラーログ
- **権限処理**: 操作前にディレクトリとファイルアクセスを検証
- **管理者権限チェック**: 設定保存時の管理者権限確認機能（WindowsIdentity/WindowsPrincipal使用）
- **デフォルトパス検証**: アプリ起動時のデフォルトパス存在確認とユーザーガイダンス機能
- **単一インスタンス**: アプリケーションの複数インスタンス実行を防止

## 設定構造

App.configファイルには以下が含まれます：

- `FolderPath`: サブディレクトリをスキャンするルートディレクトリ
- `DefaultPath`: ショートカット作成のデフォルト選択フォルダ

## エラーログ

アプリケーションは以下の優先順序で複数の場所にエラーログの書き込みを試行します：

1. アプリケーション起動パス（`error.log`）
2. ユーザーのドキュメントフォルダ（`CreateShortCut_error.log`）
3. ユーザーのAppDataフォルダ（`CreateShortCut\error.log`）
4. 一時ディレクトリ（`CreateShortCut_error.log`）

## 開発メモ

- 日本語文字互換性のため全テキストエンコーディングでShift-JISを使用
- キーボードナビゲーションのためフォームコントロールに特定のTabIndex順序設定
- Enterキー処理のためAcceptボタンをCreateBtnに設定
- フォームは固定サイズ、中央配置ダイアログとして設定
- 重複インスタンス検出によりアプリケーションの複数起動を防止

## セキュリティ・権限管理機能詳細

### 管理者権限チェック機能
- **実装場所**: MainForm.cs、SettingForm.cs共通メソッド `IsRunningAsAdministrator()`
- **実装方法**: `System.Security.Principal`名前空間の`WindowsIdentity`と`WindowsPrincipal`クラスを使用
- **動作原理**: 現在のプロセスの実行ユーザーが`WindowsBuiltInRole.Administrator`ロールに属しているかを確認
- **使用タイミング**: 
  - MainForm起動時: デフォルトパス未設定警告メッセージの内容決定
  - SettingForm保存時: 設定変更処理の実行可否判定

### デフォルトパス存在チェック機能
- **実装場所**: MainForm.cs `InitializeComboBox()`メソッド内
- **チェック内容**: 
  - App.config内`DefaultPath`設定値の`null`/空文字列チェック
  - 指定パスの物理的存在確認（`Directory.Exists()`使用）
  - ComboBox内アイテムとの一致確認
- **警告表示条件**: デフォルトパスが未設定、または物理的に存在しない場合
- **メッセージ内容**: 
  - 基本メッセージ: 設定画面での設定を促す案内
  - 動的追加: 管理者権限未実行時は管理者権限での再起動案内を併記

### ファイル書き込み権限検証
- **実装場所**: SettingForm.cs `HasFileWriteAccess()`メソッド
- **検証方法**: App.configファイルに対してFileMode.Open、FileAccess.Writeでの一時アクセステスト
- **フォールバック処理**: 権限不足時はUnauthorizedAccessExceptionをキャッチし、適切なエラーメッセージを表示

## パス文字化け問題対応（Issue #10）

### 概要
Issue #10「パスが文字化けしている　2回目」として報告されたURL生成時の日本語文字UTF-8パーセントエンコーディング問題の根本的解決。

### 問題の背景と根本原因特定
- **Issue #7**: ログ出力の文字化け問題（GetUserFriendlyPath()メソッドで解決済み）
- **Issue #10**: .urlファイル内のURL自体が文字化け（UTF-8パーセントエンコーディング）
- **根本原因**: ConvertToValidUrl()メソッドでの.NET Uri クラス使用により自動UTF-8エンコーディング発生
- **具体例**: 「教材」→「%E6%95%99%E6%9D%90」（UTF-8パーセントエンコーディング）

### 解決アプローチ

#### 第1段階：エンコーディング統一（初回修正）
- **.urlファイル保存エンコーディング**: `Encoding.Default` → `Encoding.GetEncoding("Shift_JIS")`
- **実装場所**: MainForm.cs CreateShortcut()メソッド内
- **結果**: 文字化け継続（根本原因ではなかった）

#### 第2段階：URL生成アルゴリズム修正（根本解決）
- **修正場所**: MainForm.cs ConvertToValidUrl()メソッド
- **問題コード**: `new Uri(absolutePath).AbsoluteUri` （自動UTF-8エンコーディング）
- **解決策**: 手動file:///URL構築でUTF-8エンコーディング回避

### 技術的実装詳細

#### ConvertToValidUrl()メソッド修正
```csharp
// 修正前（問題のあるコード）
Uri fileUri = new Uri(absolutePath);
string fileUrl = fileUri.AbsoluteUri;  // ←自動UTF-8エンコーディング

// 修正後（手動URL構築）
string urlPath = absolutePath.Replace('\\', '/');     // バックスラッシュ→スラッシュ
urlPath = urlPath.Replace(" ", "%20");               // スペースのみエスケープ
string fileUrl = "file:///" + urlPath;               // 手動file:///構築
```

#### .urlファイル保存（既存修正維持）
```csharp
// Shift-JISエンコーディングで保存（Issue #10第1段階修正）
System.IO.File.WriteAllText(urlFilePath, urlFileContent, Encoding.GetEncoding("Shift_JIS"));
```

### 修正効果と検証結果

#### Before（問題状況）
```
入力: C:\Users\s-fujino\mtrx Dropbox\matrix\教材
結果: file:///C:/Users/s-fujino/mtrx%20Dropbox/matrix/%E6%95%99%E6%9D%90
問題: 日本語「教材」がUTF-8パーセントエンコーディング「%E6%95%99%E6%9D%90」
```

#### After（解決後）
```
入力: C:\Users\s-fujino\mtrx Dropbox\matrix\教材  
結果: file:///C:/Users/s-fujino/mtrx%20Dropbox/matrix/教材
効果: 日本語文字はそのまま保持、スペースのみ%20エスケープ
```

### 互換性確認

#### Issue #7機能との互換性
- **GetUserFriendlyPath()メソッド**: ✅ 問題なし
- **動作**: file:///URLデコードで正常な日本語パス復元
- **影響**: なし（むしろ処理が効率化）

#### Windows .urlファイル標準準拠
- **ファイル形式**: `[InternetShortcut]\nURL=<URL>` ✅ 準拠
- **エンコーディング**: Shift-JIS（日本語環境標準） ✅ 適切
- **URL形式**: file:///スキーム with 日本語文字 ✅ 対応
- **ブラウザ互換**: Windows標準ブラウザで正常動作 ✅ 確認済み

### 技術的特徴
- **根本解決**: UTF-8パーセントエンコーディング問題の完全解決
- **標準準拠**: Windows .urlファイル仕様完全準拠
- **後方互換**: 既存のIssue #7機能と完全互換
- **エンコーディング統一**: Shift-JIS統一でログ・ファイル一貫性確保

## キーボードショートカット機能（Ctrl+W）

### 概要
Issue #8対応として実装されたアプリケーション終了のキーボードショートカット機能。

### 重要な注意事項
⚠️ **非標準動作**: Ctrl+Wによるアプリケーション終了は、一般的なWindows標準（Alt+F4）とは異なります。

### 実装詳細

#### MainForm のCtrl+W機能
- **実装場所**: MainForm.cs `ProcessCmdKey()`メソッド
- **動作**: Ctrl+W押下時に確認ダイアログを表示してからアプリケーション全体を終了
- **確認メッセージ**: "アプリケーションを終了しますか？"
- **安全機能**: 
  - デフォルト選択は「はい」（ユーザビリティ優先）
  - エラー時は通常の`this.Close()`にフォールバック
  - 操作ログを詳細記録

#### SettingForm のCtrl+W機能
- **実装場所**: SettingForm.cs `ProcessCmdKey()`メソッド
- **動作**: 設定画面からCtrl+W押下時に未保存変更に関する警告を表示
- **確認メッセージ**: "設定を保存せずにアプリケーションを終了しますか？"
- **安全機能**:
  - デフォルト選択は「はい」（ユーザビリティ優先）
  - エラー時は設定画面のみ閉じる（`this.Close()`）
  - 操作ログを詳細記録

### セキュリティ・UX配慮

#### 意図しない終了防止
- **確認ダイアログ必須**: 全てのCtrl+W操作で確認ダイアログを表示
- **ユーザビリティ重視**: 確認ダイアログの初期選択は「はい」（素早い操作を支援）
- **明確なメッセージ**: 何が起こるかを具体的に表示

#### エラーハンドリング
- **包括的例外処理**: ProcessCmdKeyメソッド内で全例外をキャッチ
- **適切なフォールバック**: 
  - MainForm: `this.Close()`で通常終了
  - SettingForm: `this.Close()`で設定画面のみ終了
- **詳細ログ記録**: LoggingUtilityを使用した操作ログとエラーログ

#### ログ記録内容
- 正常終了: "Ctrl+W によるアプリケーション終了"
- 設定画面終了: "Ctrl+W による設定画面からのアプリケーション終了"
- エラー発生: "Ctrl+W処理エラー: {詳細メッセージ}"

### 技術実装方式
- **メソッド**: `ProcessCmdKey()` オーバーライド
- **キー判定**: `keyData == (Keys.Control | Keys.W)`
- **終了方式**: `Application.Exit()`（適切なクリーンアップを保証）
- **戻り値**: 常に`true`（キー処理完了を通知）

### テスト対象項目
1. **基本機能**: 両フォームでのCtrl+W動作確認
2. **確認ダイアログ**: Yes/No選択による動作分岐
3. **エラーハンドリング**: 例外発生時のフォールバック動作
4. **ログ記録**: 操作ログとエラーログの適切な記録
5. **他機能への影響**: 既存のキーボードショートカットとの競合確認

## 開発手順（Claude Code向け指示）

Claude Codeによる開発作業では、以下の手順を必ず守ること：

### 作業前の必須プロセス

1. **計画立案**: 必ずultrathinking機能を使用して詳細な計画を立てる
2. **計画の明示**: 計画後に日本語で箇条書きにて実施内容をわかりやすく記載する
3. **ユーザー承認**: 計画を提示した後、必ずユーザーに「この内容で実施してよろしいでしょうか？」と確認し、承認を得てから作業を開始する
4. **履歴管理**: 全ての計画と実施内容を`history/`フォルダに保存する

### 作業の記録・履歴管理

- 作業開始時に日付・時刻とタスク内容を含むファイル名で履歴ファイルを作成
- 計画内容、実施手順、結果を履歴ファイルに記録
- ファイル名形式: `YYYY-MM-DD_HHMM_タスク名.md`（時分まで含めて詳細管理）

### 必須事項

- 計画なしでの直接的なコード変更は禁止
- 作業内容は必ず日本語で記録
- 履歴ファイルには以下を含める：
  - 作業の目的と背景
  - 詳細な実施計画
  - 変更内容の記録
  - 結果の評価
