# EndpointId のコード反映規約

本書は、EndpointId を **文書（inventory）とコードの双方で一貫して扱うための最終規約**を定義する。
本規約は、取引所ごとの実装差を許容しつつ、命名揺れ・重複・手動裁定を排除することを目的とする。
また、EndpointId を CallMeta に埋め込み、観測・診断・追跡に耐える最低限のメタ情報として扱う。

---

## 1. 正本と責務分離

1. EndpointId の正本は、取引所別 inventory（`docs/inventory/endpoints-*.md`）である。
2. inventory は次の 4 列のみを持つ。

   * EndpointId
   * Method
   * Path
   * Scope
3. コードは inventory を正本として参照し、EndpointId を **定数および CallMeta** に反映する。
4. Contracts 層では EndpointId を **Call 名として使用しない**。

---

## 2. EndpointId 定数の扱い

### 2.1 定義場所

1. EndpointId は **取引所配下**にのみ定義する。
2. 共通層（Contracts / Core / Common）に EndpointId の文字列定義を置いてはならない。
3. 取引所ごとに `*EndpointIds` クラスを設け、各 EndpointId を `public const string` として定義する。
4. `*EndpointIds` は原則として **Wire 層に閉じる**（`src/Exchanges/<Ex>/Wire/Constants`）。

例：

* `BitflyerEndpointIds`
* `BittradeEndpointIds`

### 2.2 命名規則

1. 定数名は inventory の EndpointId と **完全一致**させる。
2. EndpointId はコード内で直接記述せず、必ず `*EndpointIds` を参照する。

---

## 3. Path 定数の扱い

### 3.1 定義場所

1. Path は取引所ごとに `*Paths` クラスを設け、`public const string` として定義する。
2. Path 定数は取引所実装内に閉じ、共通層へ公開しない。
3. `*Paths` は原則として **Wire 層に閉じる**（`src/Exchanges/<Ex>/Wire/Constants`）。

### 3.2 命名規則

1. Path 定数の変数名は EndpointId から **機械的に導出**する。
2. 形式は次の通りとする。

```
<EndpointId>Path
```

3. 定数の値は inventory に記載された Path を **そのまま**使用する（`{parameter}` を含む）。

---

## 4. CallMeta における利用

1. Wire / Raw / Normalized 各層の Call は `CallMeta` を保持する。
2. `CallMeta` には少なくとも次の情報を含める。

   * EndpointId
3. `CallMeta.EndpointId` は `*EndpointIds` の定数を参照して設定する。
4. 文字列リテラルによる EndpointId の設定を禁止する。
5. **Endpoint 呼び出しに対応しない内部処理**は `CallMeta.InternalEndpointId`（例：`"Internal"`）を用いる。
6. 内部処理の `CallMeta` は `CallMeta.CreateInternal(...)`（または同等のヘルパ）で生成することを推奨する。

---

## 5. 各層における Call 命名

### 5.1 対象層

* Wire 層
* Raw 層
* Normalized 層

### 5.2 命名規則

1. 各層における endpoint 呼び出しメソッド名は、次の形式で統一する。

```
<EndpointId>CallAsync
```

2. 同一 EndpointId に対して、Wire / Raw / Normalized のすべての層で **同一のメソッド名**を使用する。
3. 層ごとの差異は、戻り値の DTO 型および内部実装に限定する。

---

## 6. Contracts 層での扱い

1. Contracts 層の API 名（Capability I/F 等）には EndpointId を使用しない。
2. Contracts 層はドメイン語彙による API 名を採用する。
3. EndpointId は Adapter / Exchange 実装内部でのみ使用される。

---

## 7. 禁止事項

1. EndpointId を文字列リテラルとして直接記述してはならない。
2. inventory に存在しない EndpointId をコード側に追加してはならない。
3. `*EndpointIds` に定義されている EndpointId が inventory に存在しない状態を許容してはならない。
4. Raw / Normalized 層で `CallMeta` を新規生成して EndpointId を落とす実装を推奨しない（原則：上流 `Meta` を伝播する）。

---

## 8. 整合性検証（推奨）

1. 取引所別 inventory の EndpointId 列と、`*EndpointIds` の定数一覧が完全一致することをテストで検証する。
2. `*EndpointIds` 内で EndpointId の重複が存在しないことを検証する。
3. これらの検証は CI に組み込むことを推奨する。
4. 追加で、`WireCallSpec.EndpointId` が常に非 null / 非空であることをテストで検証することを推奨する。

---

## 9. 本規約の位置付け

本規約は、EndpointId の命名およびコード反映に関する **最終判断基準**である。
取引所追加・API 追加時は、本規約および inventory を同時に更新しなければならない。
