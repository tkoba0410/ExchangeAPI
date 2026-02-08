# 実装チェックリスト（再検証版 / Non-Normative）

本書は **仕様を定義しない**。
TopSpec / inventory / governance により定義された規範が、
**コード化の過程で揺らがずに守られているかを確認するためのチェックリスト**である。

* 本書は裁定を行わない
* 判断が必要になった場合は、必ず Normative 文書へ戻る
* 各項目は **事実確認** としてチェックされる

---

## 0. 正本（Source of Truth）の確認

* [ ] 今回の実装対象について、参照する正本（SoT）を明示した

  * 公式 API ドキュメント（取引所公式）
  * TopSpec（docs/ 配下の規範文書）
  * Naming Rules（`docs/naming-rules.md`）
  * 該当 inventory（例：endpoints-bitflyer.md）
* [ ] 「この実装判断はどの正本に基づくか」を PR / 作業メモに明記した

---

## 1. EndpointId の確認（最重要）

* [ ] 実装対象のエンドポイントが inventory に列挙されていることを確認した
* [ ] inventory に存在しない EndpointId を新規実装していない
* [ ] EndpointId の構成（HTTP Method を表す語を採用する/省略する等）が、当該取引所 inventory の EndpointId ルールと一致していることを確認した
* [ ] EndpointId が識別子用途に限定され、意味・分類・ナビゲーションを背負っていないことを確認した
* [ ] 単語境界の粒度が取引所ルールと一致していることを確認した
* [ ] 別名・略称・代替表記（alias）を EndpointId に混入させていないことを確認した
  * [ ] alias を扱う必要がある場合は、inventory の `Aliases` セクション（または所定の記録箇所）に対応関係として記録し、EndpointId と混同しないことを確認した
  * [ ] コード側で alias を扱う場合、その根拠が inventory の `Aliases` の記載と一致していることを確認した

---

## 2. PresentIn（実装層）の確認

* [ ] inventory に記載された PresentIn を確認した
* [ ] 実装した層（Wire / Raw / Normalized）が PresentIn と一致していることを確認した
* [ ] PresentIn に記載のない層を実装していないことを確認した
* [ ] PresentIn が `None` の場合、いかなる層にも当該 EndpointId の API が存在しないことを確認した
* [ ] PresentIn の解釈に迷いが生じた場合、TopSpec に立ち戻って裁定した

---

## 3. 命名派生ルールの確認

* [ ] Wire 層のメソッド名が `<EndpointId>` になっていることを確認した（該当する場合）
* [ ] Raw / Normalized 層のメソッド名が `<EndpointId>CallAsync` になっていることを確認した
* [ ] `<EndpointId>` / `<EndpointId>CallAsync` の派生規則に反する独自命名を導入していないことを確認した
  * [ ] EndpointId 自体に Method 語を含める取引所ルールの場合、その Method 語は「独自補助語」ではなく EndpointId の一部として扱われていることを確認した
* [ ] `Request` / `Response` 接尾辞が API 境界の第1階層 DTO のみに使われていることを確認した
* [ ] 例外命名が必要な場合、exceptions.md に記録したことを確認した

---

## 4. DTO・型の層境界確認

* [ ] Wire 層では文字列 / Json ミラーを使用していることを確認した
* [ ] Raw / Normalized / 共通層へ文字列が流入していないことを確認した（例外がある場合は例外として記録した）
* [ ] 配列DTOが「ルート配列=List継承（許容） / 内部配列=IReadOnlyList（必須）」の方針に一致していることを確認した
* [ ] Price / Size 等が専用型で表現されていることを確認した
* [ ] Normalized / Contracts で識別子（OrderId/AccountId 等）が専用型で表現されていることを確認した
* [ ] Normalized / Contracts で `State` / `Type` / `Side` 等の列挙的概念が専用型（未知値保持を含む）で表現されていることを確認した
* [ ] 専用型化可能な値が `FreeText` のまま残っていないことを確認した
* [ ] parsing が Try 系を本流とし、OrThrow 系が補助として併設されていることを確認した

---

## 5. CallMeta.EndpointId（string）運用の確認

* [ ] `CallMeta.EndpointId` がログ・表示用途の識別子であることを確認した
* [ ] 規範的な識別は EndpointId 定数・型で行っていることを確認した
* [ ] この運用方針が文書（TopSpec / governance 等）に明文化されていることを確認した
* [ ] `CallMeta.EndpointId` 文字列を仕様分岐キーとして使用していないことを確認した
* [ ] `CallMeta.EndpointId` と規範識別子の乖離補正が Adapter 層で実施されていることを確認した

---

## 6. 取引所間の実装揺らぎ確認

* [ ] 取引所差分が `src/Exchanges/<Exchange>/` 配下に閉じていることを確認した
* [ ] 物理配置と namespace が既存のリファレンス実装と同型であることを確認した
* [ ] 共通層へ取引所固有の都合が逆流していないことを確認した

---

## 7. 分類語彙の固定確認

* [ ] Wire〜Adapter の API 分離が Public / Private のみであり、意味分類（MarketData / Trading / Account 等）を導入していないことを確認した
* [ ] 新しい分類語彙を実装側で先行追加していないことを確認した
* [ ] 語彙追加が必要な場合、governance / TopSpec による裁定を経ていることを確認した

---

## 8. 公式ドキュメント参照の確認

* [ ] inventory が「公式ドキュメント側で追跡可能な参照（API名・章名等）」を保持する設計になっていることを確認した
  * [ ] 参照を保持する列/欄（例：OfficialRef / Note 等）が存在し、運用されている
* [ ] 外部仕様変更時に、影響範囲を inventory から辿れることを確認した（参照が不足する場合は inventory の改善として扱う）

---

## 9. 例外運用の確認

* [ ] 例外が「外部仕様へ収束できない場合」に限定されていることを確認した
* [ ] 例外内容が `docs/exceptions.md` に記録されていることを確認した

  * 差分内容
  * 影響範囲（EndpointId / 層 / 型）
  * 解消条件

---

## 本チェックリストの位置づけ

* 本書は運用チェックリストであり、仕様を定義しない
* 仕様上の判断・裁定は Normative 文書（TopSpec / governance）で行う
* 本書の目的は、コード化に際しての **仕様の揺らぎ再発防止** にある
