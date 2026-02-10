# タスクマネージャー デモアプリケーション

このワークスペースには、異なるフレームワークと最新の開発手法を紹介する2つの包括的な.NETタスク管理アプリケーションが含まれています。

## 🏗️ プロジェクト構成

```
dotnet-app/
├── TaskManager/                    # .NET Framework 4.7.2 バージョン
│   ├── Models/                     # ドメインモデルと列挙型
│   ├── Services/                   # ビジネスロジックとデータアクセス
│   ├── Properties/                 # アセンブリ情報
│   ├── App.config                  # 構成設定
│   ├── TaskManager.csproj          # .NET Frameworkプロジェクトファイル
│   ├── Program.cs                  # メインアプリケーションエントリポイント
│   └── README.md                   # 詳細なプロジェクトドキュメント
├── TaskManagerModern/              # .NET 8 バージョン (推奨)
│   ├── Models/                     # 最新のC#機能を使用したドメインモデル
│   ├── Services/                   # 非同期パターンを使用したビジネスサービス
│   ├── TaskManagerModern.csproj    # 最新のSDKスタイルプロジェクトファイル
│   └── Program.cs                  # 最新のC#構文を使用したメインアプリケーション
├── .github/
│   └── instructions/               # 開発ガイドライン
├── .gitignore                      # Git除外ルール
├── dotnet-app.sln                  # Visual Studioソリューションファイル
└── DEMO_GUIDE.md                   # 包括的なデモドキュメント
```

## 🚀 クイックスタート

### モダンバージョンの実行 (推奨)
```bash
cd TaskManagerModern
dotnet run
```

### フレームワークバージョンのビルド
```bash
cd TaskManager
# .NET Framework 4.7.2 Developer Packが必要です
msbuild /t:rebuild
```

## 📖 機能

両方のアプリケーションには以下が含まれています:
- ✅ 完全なCRUD操作
- ✅ タスクの優先順位付けとステータス追跡
- ✅ 期限管理と期限超過検出
- ✅ 時間追跡 (見積時間 vs 実績時間)
- ✅ 割り当てとタグ付けシステム
- ✅ 高度なフィルタリングと検索
- ✅ 統計レポート
- ✅ インタラクティブなコンソールUI
- ✅ 永続的なデータストレージ

## 🎯 学習目標

- エンタープライズグレードのアプリケーションアーキテクチャ
- Async/await プログラミングパターン
- データ永続化戦略
- 構成管理
- エラー処理と検証
- クリーンコードの原則
- 最新のC#言語機能

## 📚 ドキュメント

- 包括的な機能のウォークスルーについては `DEMO_GUIDE.md` を参照してください
- .NET Frameworkの詳細なドキュメントについては `TaskManager/README.md` を参照してください
- 開発ガイドラインについては `.github/instructions/` を参照してください

---

デモ、学習、そして本番アプリケーションの基盤として最適です！ 🎉
