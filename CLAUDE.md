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
