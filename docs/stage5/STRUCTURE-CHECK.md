# Stage5 構成比較 (A010準拠)

## 1. 現状 vs A010 想定構成
- Contracts/Dtos: Market/Trading/Account/ExchangeInfo/Common に階層化済み。Margin フォルダは廃止し、A010の「Account配下に Balance/Collateral/Position」に合わせた。
- Transport: Protocol/Transport/Policy/Logging/Time で構成済み（A010想定と一致）。
- Factory: 今回は変更なし（A010想定どおり）。
- Bitflyer: Http/Models、ExchangeInfo、Factory は分離済みだが、A010の細分化（Trading/Market/Account/Margin/RawApi、Adapters 内マッパー等）は未実装。

## 2. A010との差分と対応方針
- Bitflyer配下のサブフォルダ（Trading/Market/Account/Margin/RawApi、AdaptersのMapper分離）はこれから実施が必要。
- ドキュメント（A010-STG5-OVER）に Margin フォルダを廃止し Account配下に統合した点を反映する必要あり。

## 3. 完了状況サマリ
- DTO階層化: 完了（A010準拠に更新）。
- Realtime除去: 完了。
- Transport構成: A010と一致。
- Bitflyer責務分離: 一部完了（Http/Models、ExchangeInfo、Factory分離）。残タスクとして APIごとのサブフォルダとマッパー整理が必要。
