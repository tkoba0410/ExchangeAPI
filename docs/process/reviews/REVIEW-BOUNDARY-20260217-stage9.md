# REVIEW-BOUNDARY（2026-02-17 / stage9）

本レビューは Boundary（層/依存/境界）軸に基づく確認を行う。

対象: branch `stage9`（Contracts capability 分離・関連同期）

---

# Scope

対象の層（Contracts / Adapter / Composition / inventory 文書）と変更点の概要を確認した。

- Contracts: `IPublicApi` から Candlesticks を分離し `ICandlesticksApi` へ移設
- Facade: `IExchangeClient` に nullable capability `Candlesticks` を追加
- Adapter: bitflyer は `Candlesticks => null`、bittrade は capability 実装を提供

---

# Checklist

* [x] 層責務の混線がない（層ジャンプ、責務の漏れ込みがない）
* [x] 依存方向の逆流がない（上位が下位の内部へ侵入していない）
* [x] Core が exchange 固有へ依存していない
* [x] 取引所差異が `src/Exchanges/<Ex>/` 配下に閉じ込められている
* [x] 横断的な情報塊（ExecutionContext 等）の復活がない

---

# Findings

## Must

なし

## Should

なし

## Nit

なし

---

# Conclusion

本変更は Boundary 軸において重大な問題はない。
