# STB-DiffChecker 保守ドキュメント

リポジトリの構成・CI/CD・リリース運用・バージョン追加手順をまとめる。
利用者向けの説明はREADME.mdを参照。

## 1. リポジトリ構成

```
【アプリ層】
  STB-DiffChecker.Desktop  … WPFデスクトップ(統合版, net8.0-windows)
  DiffCheckerWeb           … Blazor WebAssembly(net9.0, GitHub Pages配信)
        │                        │
        ▼                        │
  DiffCheckerLib (WPF共通UI・Excel出力)   │
        │                        │
【バージョン層】                   ▼
  STBridge201 / STBridge202 / STBridge210 (net8.0, UIなし)
    各: XSD生成クラス + ICompare(比較キー定義) + 許容差/重要度設定 + 埋め込みXSD
        │
【エンジン層】
  DiffCheckerCore (net8.0, 依存なし)
    ObjectComparer(リフレクション比較) / XmlValidate / XmlTree / Record / ElementCounter
```

- 設計原則: **比較ロジックはデスクトップとWebで同一DLLを共有する**。
  バージョン固有の知識はバージョン層に閉じ込め、エンジンはインターフェース
  (IST_BRIDGE / ICompare / IProperty / IToleranceSetting / IImportanceSetting)経由でのみ触る。
- namespaceは歴史的経緯でアセンブリ名と一致しない(例: DiffCheckerCoreの中身は`DiffCheckerLib.*`)。
  リネームすると差分が膨れるため意図的に維持している。

## 2. GitHub Actions（.github/workflows/）

| ワークフロー | トリガー | 内容 |
|---|---|---|
| build.yml | PR / 手動 | ソリューション全体のDebug/Releaseビルドチェック |
| release.yml | `v*`タグpush / 手動 | 統合exeをself-contained単一ファイルでpublishし、zipをReleasesに添付 |
| pages.yml | masterへのpush / 手動 | Web版をAOTでpublishしGitHub Pagesへデプロイ |

方針:
- **権限は最小化**する。既定は`contents: read`、書き込みが要るジョブにだけ個別付与。
- サードパーティ製アクション(softprops/action-gh-release)は**コミットSHAで固定**する
  (タグ書き換えによるサプライチェーン攻撃対策)。更新時は新タグのSHAを確認して差し替える。
- SDKは**global.jsonで9系に固定**。ランナーには複数SDK(将来の.NET 10等)が入っており、
  固定しないとwasm-toolsワークロードの解決が壊れる(実際に壊れた)。SDKを上げるときは
  global.json・setup-dotnetのバージョン指定・wasm-toolsの3点セットで上げること。

## 3. リリース方針（デスクトップ版）

- 配布物は**統合exe1本**(STB-DiffChecker_win-x64.zip、約73MB)。
  .NETランタイム同梱(self-contained)のため利用者側のインストール不要。
- 手順: `git tag vX.Y.Z && git push origin vX.Y.Z` だけ。以後は自動。
- バージョン番号はSTB-DiffChecker.Desktop.csprojの`<Version>`を更新してからタグを打つ
  (タイトルバーの表示に使われる)。
- zipには**exeのみ**を入れる(pdbは除外。特にNPOIが持ち込むlibSkiaSharp.pdbは約80MBあるので注意)。

## 4. GitHub Pages方針（Web版）

- masterにpushすると自動でビルド・デプロイされる。リポジトリ設定で
  **Settings → Pages → Source を「GitHub Actions」**にしておくこと(初回のみ)。
- URLは `https://<owner>.github.io/STB-DiffChecker/`。プロジェクトサイトのため
  pages.yml内でindex.htmlの`<base href>`をリポジトリ名に書き換えている
  (リポジトリ名を変えたらここも変える)。
- **AOTコンパイル**を有効にしている(比較速度が約3倍)。AOTはILトリミング必須のため、
  リフレクションで触るアセンブリをDiffCheckerWeb.csprojの`TrimmerRootAssembly`で保護している。
  **新しいライブラリを追加してWebから参照する場合はTrimmerRootAssemblyへの追記を忘れない**こと
  (忘れると本番だけ実行時エラーになる)。
- セキュリティ: index.htmlのCSPで外部通信・外部スクリプトを遮断している。
  外部リソースを追加する場合はCSPの更新が必要(基本は追加しない方針)。
- ビルド時間はAOTのため7分前後かかる。

## 5. ST-Bridge新バージョン対応手順（例: 2.2が出たら）

1. buildingSMART Japan公式からXSDを入手
   (2.1の例: https://www.building-smart.or.jp/wp-content/uploads/2023/05/ST-Bridge210.zip)。
   ※過去にZIP内XSDの先頭に余分な空白がありxsd.exeが失敗した。先頭が`<?xml`で始まるか確認する。
2. クラス生成: `xsd.exe STBridge_v2XX.xsd /classes /namespace:ST_BRIDGE2XX /language:CS`
   (Windows SDK付属のNETFX Tools版を使用)
3. STBridge2XXプロジェクトを新設(STBridge210をコピーが早い):
   - 生成クラスとXSD(埋め込みリソース)を配置。csprojのRootNamespaceを`STB_DiffChecker_2XX`にする
     (埋め込みXSDのリソース名がこれに依存)
   - ICompareを旧バージョンからコピーし、スキーマ差分に対応
     (型が消えた→削除、配列化された→order/pos等のキーでICompare追加)
   - ImportanceSettingのタブ一覧を新要素に合わせて更新
4. 組み込み:
   - デスクトップ: DesktopVersionEngine.Create と SupportedVersions に1エントリ追加
   - Web: VersionEngine.Create と SupportedVersions に1エントリ追加、
     DiffCheckerWeb.csprojのProjectReferenceと**TrimmerRootAssembly**に追加
5. 検証: 新旧の実ファイルで比較を実行し、概要集計とキーマッチング
   (並び順を入れ替えたファイルで正しくペアになるか)を確認

## 6. 依存パッケージ方針

- **NPOIは2.7.4に固定**する。理由: 2.8.0は保守料EULA、2.7.5以降はNSax(LGPL)を含むため。
  変更する場合はライセンス確認を先に行うこと。
- 脆弱性チェック: `dotnet list STB-DiffChecker.sln package --vulnerable --include-transitive`
  を定期的に(少なくともリリース前に)実行。推移的依存の脆弱性は、修正版の明示的な
  PackageReference追加で上書きする(DiffCheckerLib.csprojに前例あり)。

## 7. 開発環境

- Visual Studio 2022 または .NET SDK 9(global.jsonで固定。net8.0プロジェクトもSDK9でビルド可)
- Web版のローカル実行: `dotnet run --project DiffCheckerWeb`(通常publishはAOTなし。
  AOTを試す場合は `dotnet workload install wasm-tools` の上で
  `dotnet publish -c Release -p:RunAOTCompilation=true -p:PublishTrimmed=true`)
- ユーザー設定は `%APPDATA%\STB-DiffChecker\settings.json`
  (旧レジストリHKCU\NS-NS\STB-DiffCheckerからは初回起動時に自動移行)

## 8. 検証の考え方

- 比較エンジンに手を入れたら、**変更前後で比較結果の全レコードが一致すること**を確認する。
  TestData/(2.0.2実データ)を入力に、タブ・XMLパス・キー・A/B値・判定をダンプして
  ハッシュ比較するのが確実(意図した挙動変更がある場合は差分を目視確認)。
- バージョン層に手を入れたら、該当バージョンの実ファイルで読込〜比較〜概要集計まで通す。
- 許容差まわりを触ったら「座標を数mmずらしたファイル」で許容差内判定が出ることを確認する
  (過去にこの判定が数年間無効になっていたバグがあった)。

## 9. 既知の落とし穴

- .gitignoreに歴史的な`*.html`全体ルールがある。HTMLを追加する場合は例外指定が必要
  (DiffCheckerWeb/wwwrootには例外設定済み)。
- 生成クラス(STBridge_v2XX.cs)は手で編集しない。比較ロジックはICompare/IPropertyの
  partial classで注入する。
- ObjectComparerの配列マッチングは、ICompareのない要素を「同型の先頭」とペアにする。
  複数要素が並ぶ新要素にはICompare(キー定義)を必ず用意すること。
- ImportanceSettingのタブを増やしたら、概要の要素数はElementCounter(汎用)が自動対応するが、
  タブ名がST_BRIDGEオブジェクトのプロパティ名と一致している必要がある。
