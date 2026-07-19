# 0006: WASMはAOTコンパイル + 選択的トリミング

- ステータス: 採用（2026-07）
- 関連: [0001](0001-reflection-based-comparison.md), [0005](0005-blazor-wasm-pages.md)

## 文脈
Blazor WASMの既定はILインタープリタ実行で、リフレクション主体の比較エンジンは
ネイティブ比10〜30倍遅かった(実データの比較に30〜40秒)。

## 決定
- Pages用publishで`RunAOTCompilation=true`を有効にする(比較が約3倍高速化、13秒程度)
- AOTはILトリミング必須のため`PublishTrimmed=true`とし、リフレクション・XmlSerializerで
  触るアセンブリを**TrimmerRootAssembly**で保護する
  (DiffCheckerCore, STBridge201/202/210, System.Private.Xml, System.Text.Encoding.CodePages)
- ローカル開発(dotnet run)はAOTなしのまま(ビルド時間を優先)

## 影響・落とし穴
- **Webから参照するライブラリを追加したらTrimmerRootAssemblyにも追加する**こと。
  忘れるとビルドは通るのに本番だけ実行時エラーになる
- CIビルドが7分前後かかる(AOTコンパイルのため)
- SDKはglobal.jsonで9系に固定(ランナーの新SDKがwasm-toolsワークロード解決を壊すため)
