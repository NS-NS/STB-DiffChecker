# 0007: デスクトップは統合exe1本（バージョン自動判別）

- ステータス: 採用（2026-07。旧「バージョン別3exe」を置き換え）
- 関連: [0003](0003-version-as-assembly.md)

## 文脈
元はST-Bridgeバージョンごとに別exe(201/202/210)で、利用者がファイルのバージョンを
自分で判断して起動するexeを選ぶ必要があった。配布物も3本(各73MB)。
Web版で「version属性からの自動判別」を実装し、同じ仕組みがデスクトップにも適用可能になった。

## 決定
- デスクトップアプリをSTB-DiffChecker.Desktop 1本に統合する
- ファイル選択時にversion属性からDesktopVersionEngineがスキーマ・設定・比較定義を切り替える。
  判別結果はタイトルバーに表示。A/Bのバージョン不一致はエラーで拒否
- AbstractMainWindowの「固定バージョン検査(GetVersion)」は
  「バージョン受入フック(PrepareForVersion)」に変更

## 理由
- 利用者がバージョンを意識する必要がなくなる(Web版と同じUX)
- 配布物が1本になり、3バージョン分を含めてもサイズはほぼ変わらない
  (self-containedの.NETランタイムが支配的なため)

## 影響
- バージョン切替時に許容差・重要度テーブルはリセットされる(設定CSVは切替後に読み込む)
- 新バージョン追加時はDesktopVersionEngine.CreateとSupportedVersionsに1エントリ足す
