# Exchange Structure Rules

本書は、`DesignContract.md` で定義された契約を **実装レベルで具体化する規約集**である。

* 本書は **HOW（どう実装するか）** を定義する
* 本書は DesignContract に **従属**する
* 内容が衝突する場合、DesignContract を正とする

---

## 1. Factory の既定方針

### Raw default

すべての取引所 Factory は、以下を満たさなければならない。

* `CreateRaw()` を既定とする
* Adapter / Wire は opt-in とする
* 取引所ごとに既定レイヤを変えてはならない

---

## 2. Facade の責務

* Facade は **委譲のみ**を行う
* new / 組み立て / 配線を行ってはならない
* 互換 API（Obsolete）は Facade に残さない

---

## 3. Symbol の扱い（MUST）

* Adapter 内での symbol 変換は **ExchangeInfo 駆動**
* 一覧に存在しない symbol は例外
* 推測・暗黙変換は禁止

---

## 4. enum / 未知値ポリシー（MUST）

* 未知値を既知値に丸めてはならない
* Raw / Adapter ともに fail-fast
* 互換的処理が必要な場合は Legacy に隔離する

---

## 5. 例外ポリシー

* Adapter の公開 API は例外型を統一する
* operation 名は定数化する

---

## 6. テスト規約（抜粋）

最低限、以下をテストで保証する。

* 未知 enum 値 → 例外
* 未知 symbol → 例外
* Factory default が Raw であること
