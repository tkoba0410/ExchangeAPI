# 位置づけ

本ドキュメントは **エンドポイントの一覧のみ** を目的とする。

各エンドポイントの仕様・意味・制約については、
公式 API 文書を正本とし、ここには記述しない。

# Endpoints Inventory

本書は、本リポジトリで **扱っている API エンドポイントの存在一覧（Inventory）** を示す。
本書は **Normative ではない**。

記載内容は「存在」と「参照先」に限定し、  
仕様・意味・注意点の説明は含めない。

---

## 記載ルール（固定）

- 各エンドポイントは **1 行 = 1 事実**
- 書くのは「取引所 / 種別 / HTTP / Path / 公式参照」のみ
- 振る舞い・注意・制限事項を書いてはならない

---

## Public APIs

| Exchange | Category | Method | Path | Official Reference |
|---------|----------|--------|------|--------------------|
| bitFlyer | Public | GET | /v1/markets | https://lightning.bitflyer.com/docs |
| bitFlyer | Public | GET | /v1/board | https://lightning.bitflyer.com/docs |

---

## Private APIs

| Exchange | Category | Method | Path | Official Reference |
|---------|----------|--------|------|--------------------|
| bitFlyer | Private | GET | /v1/me/getbalance | https://lightning.bitflyer.com/docs |

---

※ 実装有無・対応状況は `inventory-*.md` を参照すること
