# 0004: エンジン(Core)とWPF UIの分離、namespace非変更

- ステータス: 採用（2026-07）

## 文脈
元のDiffCheckerLibは比較エンジンとWPF UIが混在しており、
WPF依存アセンブリはブラウザ(Blazor WASM)から参照できないためWeb版が作れなかった。

## 決定
- WPFに依存しないコード(ObjectComparer/XmlValidate/XmlTree/Record/インターフェース群)を
  DiffCheckerCore(素のnet8.0)に分離。DiffCheckerLibはWPF UI専用にする
- 移動時に**namespaceは変更しない**(DiffCheckerCoreの中身はDiffCheckerLib.*のまま)
- UI依存が混ざりそうな箇所はAPI境界で分離する
  (例: ImportCsvはMessageBoxを呼ばず「未対応パスの一覧」を返し、表示はUI側の責務)

## 理由
- namespace維持により既存コード(ICompare約400ファイル)を無変更で移行できた
- C#ではnamespaceとアセンブリ名は独立しており、実害がない

## 影響
- 新しいコードをCoreに足すときは`using System.Windows`等のUI依存を入れないこと
  (入れるとWeb版のビルドが壊れるのですぐ気づける)
