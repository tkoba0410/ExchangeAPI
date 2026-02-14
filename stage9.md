# Stage9

## Stage9 ゴール

Stage9 は「実働の証明」と「安全な回帰保証」に絞る。

* **実働サンプル**：利用者が最短で動かせる入口を提供する（代表API・安全）
* **実地試験（Live Probe）**：取引所別 × public/private で **全Endpointの実呼び出し**を行い、観測結果を収集する（証跡）
* **CI回帰（Deterministic）**：実地で収集したサンプル（Fixture）を用いてネットワーク無しで回帰試験を行う（再現性）
* **セキュリティ強化**：Private実行・キャプチャ保存に必要なゲートとサニタイズを規約化する

Stage9 では以下を行わない。

* Exchange 拡張（新規取引所追加、既存取引所のエンドポイント追加）
* Contracts の意味拡張
* 仕様正本（公式API文書）を置き換えるような運用

> 原則：公式API文書が正本。実地試験で得たデータは **証跡（audit）** であり正本ではない。

---

# Stage9 の成果物（Deliverables）

1. **実働サンプル**（SampleBot）
2. **実地試験ランナー**（ApiProbe：手動/明示実行）
3. **Fixture 回帰テスト**（CI常時）
4. （任意）**Live Test Harness**（tests配下・既定SKIP）

---

# 体系（3レーン）

Stage9 では「テスト」を次の3レーンに分離する。

## レーンA：Deterministic Tests（CI常時実行）

目的：実装が壊れていないことを保証する（再現性100%）。

入力：

* 実地試験で取得した **サニタイズ済みFixture**

対象：

* Wire → Raw パース
* Raw → Normalized 変換
* Contracts 整合
* 署名生成（Private：署名の構築とヘッダ整形の検証）
* 429 / Timeout / Partial Failure の疑似注入（Transport層）
* サニタイズの漏洩防止（fixtureが安全であることの検証）

特性：

* ネットワーク不要
* APIキー不要
* 安定・高速

## レーンB：Live Probe（実地試験・手動/明示実行）

目的：全APIの **実呼び出し** を行い、観測結果（サンプル）を収集する。

対象：

* 取引所別
* public/private 別
* Inventory 駆動で EndpointId 列挙（全Endpointを回せること）

出力：

* レポート（成功/失敗/HTTP/遅延/リトライなど）
* request/response/meta キャプチャ（サニタイズ済み）

特性：

* 既定で安全（publicのみ）
* private は二重ゲート必須
* 発注系はさらに段階ゲート（Stage9では原則 read-only 優先）

## レーンC：Live Test Harness（任意・既定SKIP）

目的：Live Probe をテスト形式で実行し、成功/失敗を機械判定できるようにする。

原則：

* 既定は **全テストSKIP**
* 明示フラグがある場合のみ実行
* CI（標準）では実行しない

---

# 役割分担：サンプル vs プローブ vs テスト

## SampleBot（実働サンプル）

目的：利用者が最短で「動かせた」を得る。

* 代表APIのみ（Public中心）
* 短い・安全・説明が主
* Privateは read-only を必要最小限（採用する場合でもゲート必須）
* 網羅性は要求しない（全APIは ApiProbe の責務）

## ApiProbe（実地試験ランナー）

目的：全Endpointの実呼び出しと観測結果の収集。

* Inventory 駆動で列挙
* 取引所別/public-private別に実行
* キャプチャ保存（サニタイズ必須）
* 429/Timeout/PartialFailure の観測も対象（自然発生 or 設定で誘発）

## CI Fixture Tests

目的：ApiProbeで集めた現実サンプルを用いた回帰保証。

* ネット不要
* deterministic
* 変更で壊れたらCIが検出

---

# 実装の層設計（重複排除）

Stage9 の中核は **Probe Core ライブラリ**として共通化し、samples と tests から共有する。

## Probe Core（共通ライブラリ）

配置案：

* `src/Diagnostics/ApiProbe/`（例：`ExchangeApi.Diagnostics.ApiProbe`）

責務：

* `ProbePlan`：実行対象（EndpointId）と実行順序
* `ProbeExecutor`：実行して `ProbeReport` を生成
* `CaptureWriter`：request/response/meta の保存
* `Sanitizer`：保存前の自動マスク（必須）

原則：

* 仕様判断・規範化を入れない（観測と検証のみに留める）

## Runner（CLI入口）

配置案：

* `samples/ApiProbe/`（または `tools/ApiProbe/`）

責務：

* 引数解析（exchange/scope/endpoint selection/output）
* 実行ゲート（private/live）
* `ProbeExecutor` 呼び出し
* レポート出力

## Tests（fixture/任意live）

* Fixture Tests：`tests/*`（CI常時）
* Live Harness：`tests/Live.*`（既定SKIP）

---

# データ取り扱い（Fixture と _references の住み分け）

## Fixture（CI用）

保存場所：

* `tests/Fixtures/{exchange}/{scope}/{EndpointId}/{case}/...`

用途：

* CIの回帰試験に使用する

ルール：

* サニタイズ済みのみ保存
* 代表ケース中心（1〜3例/EndpointId）

## _refere
