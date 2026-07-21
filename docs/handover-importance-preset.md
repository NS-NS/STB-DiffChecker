# 引継ぎ: 重要度プリセット(S2/S4)＋プロファイル切替機能

最終更新: 2026-07-21。別アカウント/別セッションで続きを行うための自己完結メモ。
（設計プラン・作業ログは元アカウントの `~/.claude` 配下でこのリポジトリには無いため、必要情報を全てここに集約している。）

> **2026-07-21更新: セクション1(プロファイル切替機能)と「次にやること」1〜6は実装完了。**
> ビルド0エラー、ハーネス回帰(202実データ・210Mini)は変更前後で全レコード一致、
> 6プリセットの埋め込み読込・適用をランタイム検証済み(分布はセクション0の記載と一致)。
> 残タスク: Desktop手動検証(下記「検証」参照)とセクション2の符号解決(保留)。

---

## 0. 現在の状態(このブランチでコミット済みのもの)

- **プリセットCSV 6ファイル = 完成**（全行有効値・不正値0）
  - `STBridge201/Presets/S2.csv`,`S4.csv`（各2149行）
  - `STBridge202/Presets/S2.csv`,`S4.csv`（各2187行）
  - `STBridge210/Presets/S2.csv`,`S4.csv`（各3846行）
  - 値は「高/中/低/対象外」の4種のみ。S4は各S2の「中→高」変換版。
  - 最終分布: 201 S2=高847/中897/低291/対象外114、202 S2=高865/中909/低293/対象外120、210 S2=高1186/中1481/低928/対象外251（S4は中が全部高に移動）。
- **コード修正済み: `id_dependence` の符号解決**
  - `STBridge201/202/210/ICompare/StbStory.cs` に `IProperty` を実装。`@id_dependence`(依存階のid)を参照先の階名(`StbStory.name`)に解決して比較するよう変更（従来は生id比較）。
  - ビルドOK・ハーネス回帰なし確認済み（元TestDataはベースライン一致）。

これらは**まだ実装本体(UI等)には繋がっていない**。プリセットは同梱設定(EmbeddedResource)も未実施＝ファイルがリポジトリに置いてあるだけ。

---

## 1. これから実装する機能(承認済みプラン)

Desktop版(WPF)の「入力・設定」→「重要度」タブを、**設計段階プロファイル S2/S4/カスタム を切り替えて編集**できる形にする。

### 要件
- 重要度タブに **プロファイル切替 S2 / S4 / カスタム(ファイル読込)**。選択で現在バージョンの該当プロファイルをグリッドに読込・編集可能表示。
- 選択中プロファイルから**編集したセルを着色**、選択表示を「**S2\***」等に変える。
- 許容差タブは**着色しない**（従来どおり手編集）。
- 起動直後(STB未読込=バージョン未確定)は**重要度タブ無効化**＋「先にファイルを指定してバージョンを認識させてください」表示。
- STB読込でバージョン確定→**既定S2**表示。別バージョン読込時、S2/S4は同プロファイル再適用(編集破棄)、カスタムはS2へ戻す。カスタム読込はバージョン確定後のみ許可。
- Web版(Blazor)は **S2/S4適用ボタンのみ**追加（編集・着色・カスタム管理はDesktop限定）。既存カスタムCSV読込は残す。

### なぜ編集不可だったか(原因)
`DiffCheckerCore/Interface/IImportanceSetting.cs` の `CreateTable()` で重要度列が `ReadOnly=true`。読み戻し(`STB-DiffChecker.Desktop/MainWindow.xaml.cs` の `SetSetting()`)は編集値反映済みなので、列を編集可能化すれば実行時比較に反映される。値は`TranslateJapanese()`(`DiffCheckerCore/Enum/EnumExtension.cs`)が4値以外で例外→クラッシュのため**4値限定ドロップダウン**にする。

### 実装方針(ファイル別)
- **A. プリセット取得API**: `IImportanceSetting` に `static readonly IReadOnlyList<string> PresetNames = ["S2","S4"];` と `string? GetPresetCsv(string name);` を追加。各 `ImportanceSetting`(201/202/210)で実装＝`GetSchemaContent`と同じ埋め込み読込ヘルパ(`XmlValidate.GetEmbeddedXsd`同様)で `{asm}.Presets.{name}.csv` を返す(無ければnull)。`CreateTable()`は変更しない。
  - **各 `STBridge2XX.csproj` に `Presets/S2.csv`・`S4.csv` を `<EmbeddedResource>` 追加**(XSDと同じやり方)。
- **B. 着色コンバーター(新規)**: `DiffCheckerLib/WPF/ChangedCellToBrushConverter.cs`（`IMultiValueConverter`）。基準値 `IReadOnlyDictionary<string,string>`(StbName→適用時の重要度)とハイライトBrushを持ち、`Convert([Importance,StbName])`で基準と現在値が異なればBrush、一致/不明は`DependencyProperty.UnsetValue`。
- **C. 重要度列ComboBox化＋着色**: `DiffCheckerLib/WPF/AbstractMainWindow.cs` の `dataGrid_AutoGeneratingColumn_Importance` で `case "Importance":` を `DataGridComboBoxColumn` に差替(`e.Column=combo`)。ItemsSourceは `Enum.GetValues(typeof(Importance)).Cast<Importance>().Select(i=>i.ToJapanese())`、`SelectedItemBinding=new Binding("Importance")`、CellStyleのBackground SetterにMultiBinding(Importance,StbName→上記コンバーター)。ベースに `protected Dictionary<string,string> importanceBaseline = new();` 追加。許容差ハンドラは変更しない。
- **D. CSV適用の共通化**: `AbstractMainWindow.ReadCsv(path)` の本体を `ApplyCsvLines(string[] lines)` に切出し。プリセット適用は `GetPresetCsv(name)` の文字列を行分割→`ApplyCsvLines`。
- **E. プロファイル状態機械**: `MainWindow.xaml.cs` に `enum ImportanceProfile{S2,S4,Custom}`、`currentProfile`(既定S2)、`importanceModified`、`versionConfirmed` を追加。`ApplyProfileForCurrentVersion()`(プリセット/カスタム適用→`RefreshSettingTables()`→基準再構築→modified=false→UI更新)、`RefreshSettingTables()`で `importanceTable.Columns["Importance"].ReadOnly=false` と基準再構築を**DataContext代入の前に**行う。ラジオChecked/`CellEditEnding`/起動ゲート/STB読込成功時/バージョン変更(`PrepareForVersion`)にフック。詳細は下記「プラン全文」参照。
- **F. UI**: `STB-DiffChecker.Desktop/MainWindow.xaml` の `tabImportance` を2段化(上=S2/S4/カスタムのラジオ、下=`dgrdImportance`)＋未確定時の案内TextBlock。既存 `btnReadSet`(設定ファイル読込)はカスタム扱いに整合、`btnExportSet`(書き出す)は現状維持。
- **G. Web**: `DiffCheckerWeb/Pages/Home.razor` の許容差設定付近にS2/S4適用ボタン(`engine.Importance.GetPresetCsv(name)`→`<Importance>`抽出→`ImportCsv`)。
- **H. README** に運用フロー追記。

### 検証
`dotnet build STB-DiffChecker.sln` が通ること＋Desktop手動: 起動時タブ無効→STB読込でS2表示→編集で着色＆「S2\*」→S4/カスタム切替→別バージョン再適用(破棄)→書き出す/読込ループ→未配置バージョンで「プリセットがありません」。ハーネス `tools/DiffCheckerHarness`(`dotnet run -- <201|202|210> A.stb B.stb`)で比較結果の回帰確認。

---

## 2. 保留タスク(ユーザー希望・後で実装): 参照idの符号解決

`id_dependence` と同型で、**参照idを参照先の符号に解決して比較**する処理を追加する。現状は生id比較(=対象外運用)。実装したら該当プリセット値を「高」に上げる(下記の通りv210 S2/S4では既に高設定済みの箇所あり)。**v210のみ**(接合部・免震・制振は2.1.0新設)。

### 2-1. `@id_weld` → 溶接符号(`@mark`)
- `@id_weld` を持つ7要素: `StbConnectionSpecGussetPlate` / `StbConnectionSpecRibPlate` / `StbConnectionSpecStiffner` / `StbConnectionSpecDiaphragm`(StbCommon/StbConnectionSpecs配下)、`StbGussetPlate` / `StbRibPlate` / `StbDiaphragm`(StbModel/StbConnections配下)。
- 解決先: `StbModel/StbWeld/StbWeld{FullPenetration,PartialPenetration,Fillet,Flare}` を `@id==id_weld` で検索し `@mark` を返すヘルパ1本。各要素の partial class に `IProperty`(IsSpecial("id_weld")+CompareProperty)を実装。
- 実装時に v210 preset の `@id_weld`(現在=対象外)を「高」へ。※現状プリセットは対象外のまま(未実装のため)。

### 2-2. 免震/制振デバイスの `@id_section_start` / `@id_section_end` → デバイス断面符号(`@name`)
- `StbIsolatingDevice` / `StbDampingDevice` に ICompare 無し＝生id比較。参照先 `StbSecIsolatingDevice` / `StbSecDampingDevice` は `@name`(符号)を持つ。
- `id_section` と同型で参照先 `@name` を解決する処理を追加。
- **preset側は既に v210 S2/S4 で4件とも「高」設定済み**(実装まで生id比較のまま＝ノイズになり得る点に注意)。

### 参考: 実装の型(既に入っている `id_dependence`)
`STBridge210/ICompare/StbStory.cs` を参照。`IsSpecial("id_dependence")` + `CompareProperty` で `stb.StbModel.StbStories.FirstOrDefault(n=>n.id==id)?.name` を解決して比較している。`id_section` の解決は `STBridge210/ICompare/StbColumn.cs` の `FindColumnName`(id→`floor+"/"+name`)が参考。

---

## 3. プリセットCSVの生成規則(再生成が必要になった場合)

- 各バージョンの `ImportanceSetting.OrderedImportance()` を実行してXMLパスを列挙し、`<Importance>` セクションのみ(`パス,重要度`)で出力。UTF-8(BOMなし)、LF。
- **初期値ルール**: 生id属性(`@id`/`@guid`/`@id_*`)は「対象外」、それ以外は「高」。ただし**参照解決される id は「高」**＝`id_section`/`id_section_FD`/`id_section_WR`/`joint_id_top`/`joint_id_bottom`/`joint_id_start`/`joint_id_end`/`id_dependence`。
- 使い捨てダンプは `OrderedImportance()` を各版で呼ぶだけの小さなコンソール(3つの STBridge プロジェクトを参照)。
- **S4はS2の「中→高」変換**: `awk -F, '$1~/^\//{v=$2; if(v=="中")v="高"; print $1","v; next}{print}' S2.csv > S4.csv`。
- v202 S2 は v201 S2 と完全一致パスで突き合わせ(新規40件は個別判断)。v210 S2 は v202 S2 と突き合わせ(完全一致894＋断面種別×属性名の意味的一致656、残りはユーザー手動確定)。2.1.0で免震/制振/溶接/接合部/継手/貫通孔/パネルゾーン/プレキャスト杭/新鉄骨形状が新規＋既存断面の配筋が再ネストのため、v202との完全一致は894/3846のみ。

### プリセット編集時のチェック(表記ゆれ/不正値)
- 不正値: 値が `高/中/低/対象外` 以外、または列数≠2 → `awk -F, '$1~/^\//{v=$2; if(v!="高"&&v!="中"&&v!="低"&&v!="対象外")print}' file`
- 全角スペース混入(例`低　`)に注意。
- 表記ゆれ: 同名`@属性`が複数値になっている箇所、特に単独外れ値。要素内の対称属性(X/Y/Z, start/end, top/bottom, left/right)が食い違っていないか。
- 意図的に別値でよいもの(判断の記録): プレキャストの`@depth_cover_top`=高(他は低)、`StbNode/@Z`=高、`StbSecRoundBar/@R`=高(丸鋼半径=断面寸法)、パネルゾーン/貫通孔配置は一律低優先、`StbApplyConditionsList`配下は全て低。

### StbApplyConditionsList(適用条件)について
`set_default=false`運用(断面で個別指定)では既定値が空でnull同士=比較レコードを生まない休眠状態。実データもfalse。当て込み(defaultを断面へ補完してから比較)は**旧コードでも未実装**で今もしない。プリセットでは配下を全て「低」で残している。将来 set_default=true モデルを正確に比較したい場合のみ、当て込み前処理を新規実装(スコープ保留)。

---

## 4. 次にやることチェックリスト

1. ~~各 `STBridge2XX.csproj` に `Presets/S2.csv`・`S4.csv` の `<EmbeddedResource>` を追加。~~ 済(2026-07-21)
2. ~~`IImportanceSetting` に `PresetNames` / `GetPresetCsv` を追加し各版実装。~~ 済
3. ~~`ChangedCellToBrushConverter` 新規、`AbstractMainWindow` の ComboBox化＋着色＋`ApplyCsvLines`切出し。~~ 済
4. ~~`MainWindow.xaml`/`.xaml.cs` にプロファイル切替UI＋状態機械＋起動ゲート。~~ 済
5. ~~`Home.razor` にS2/S4ボタン。README追記。~~ 済
6. `dotnet build`(済・0エラー) ＋ Desktop手動検証(**未**: セクション1「検証」の手順で要確認)。
7. (保留)`id_weld` / `id_section_start-end` の符号解決実装＋該当プリセット値を高へ。

### 実装時の判断メモ(プランからの差分)
- 「重要度タブ無効化」は、タブ自体を無効化すると案内文が見えないため、
  タブ内のプロファイル行とグリッドを`IsEnabled=false`にし案内TextBlockを表示する形にした。
- `BtnRun_Click`(実行)時に`CommitEdit`を追加(セル編集中に実行を押した場合の未確定値対策)。
- `ApplyCsvLines`は空白行をスキップするようにした(プリセット/手書きCSVの空行でのクラッシュ防止)。
- 編集検知は`DataTable.ColumnChanged`で行い、基準(適用時値)との全行比較で`S2*`表示を復帰可能にした
  (編集を手で元に戻すと`*`が消える)。
