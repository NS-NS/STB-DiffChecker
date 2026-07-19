# ADR (Architecture Decision Records)

このディレクトリには、STB-DiffCheckerの設計上の重要な決定を記録する。
「なぜこうなっているのか」を将来の保守者(半年後の自分を含む)が追えるようにするのが目的。

決定を変更する場合は、古いADRを書き換えるのではなく、新しいADRを追加して
古いものをSuperseded(置き換え済み)にする。

| # | タイトル | ステータス |
|---|---|---|
| [0001](0001-reflection-based-comparison.md) | リフレクションによる汎用比較エンジン | 採用 |
| [0002](0002-matching-without-ids.md) | idではなく座標・符号でマッチングする | 採用(元設計の明文化) |
| [0003](0003-version-as-assembly.md) | ST-Bridgeバージョン=アセンブリ+partial classでの比較定義 | 採用 |
| [0004](0004-core-wpf-separation.md) | エンジン(Core)とWPF UIの分離、namespace非変更 | 採用 |
| [0005](0005-blazor-wasm-pages.md) | Web版はBlazor WebAssembly+GitHub Pages(データ非送信) | 採用 |
| [0006](0006-wasm-aot-trimming.md) | WASMはAOTコンパイル+選択的トリミング | 採用 |
| [0007](0007-unified-desktop-exe.md) | デスクトップは統合exe1本(バージョン自動判別) | 採用 |
| [0008](0008-npoi-274-pin.md) | NPOIを2.7.4に固定(ライセンス理由) | 採用 |
| [0009](0009-settings-json-over-registry.md) | ユーザー設定はレジストリでなくJSONファイル | 採用 |
