# TopSpec.Core

本書は、本リポジトリの設計を迷いなく維持するための最小規範を定義する。
詳細ルールは境界（Interfaces）および契約（Contracts）に委譲する。

---

## 1. Scope

本書が決めること：目的・大原則・層の俯瞰（論理階層）

本書が決めないこと：
- 境界の詳細規則（`docs/contracts/interfaces.md`）
- DTO の形状・命名・Nullable 等（`docs/contracts/contracts.md`）
- 取引所 API の仕様（公式文書が正本）
- endpoint 一覧（`docs/inventory/endpoints.md`）
- 例外（`docs/exceptions.md`）

## 2. Principles

1. 公開境界は明示する。
2. 層を跨いだ責務の混在を禁止する。
3. 下層から上層への依存を禁止する。
4. spec（Wire/Raw/Normalized）と domain（Contracts 以降）を分離する。
5. 取引所固有の差異は隠蔽しない（抽象化できる範囲のみを Contracts とする）。
6. 取引所 API 仕様の正本は公式文書とする（写経しない）。

## 3. Stability

TopSpec は短く保ち、追加より編集（削除/統合）を優先する。

## 4. Goals / Non-Goals

### 4.1 Goals

1. 取引所ごとの API を前提に、Wire / Raw / Normalized（spec）を第一級の API として提供する。
2. 抽象化可能な範囲のみを Contracts とし、横断語彙の意味論を安定させる。
3. 公開入口（Factory）により、spec 直利用と抽象化利用を明示的に選択できる。

### 4.2 Non-Goals

1. 全 endpoint の網羅（完全写像）。
2. 取引所差異の完全な抽象化。
3. spec（Wire/Raw/Normalized）を不変に保つこと。

## 5. Logical Layers

1. **Wire**：transport（spec）
2. **Raw**：公式 API の鏡像（spec）
3. **Normalized**：単独取引所内の正規化（spec）
4. **Adapter**：spec → contracts の翻訳境界
5. **Contracts**：取引所間抽象化（domain 入口）
6. **Domain**：横断的なふるまい
7. **Composition**：DI/Factory/組み立て

## Data Shape by Layer

- wire：text のみ
- raw：RawJson 鏡像（プリミティブDTO）
- normalized：正規化（enum/type DTO）
- contracts：取引所間抽象化（enum/type DTO）
境界ルールの詳細は `docs/contracts/interfaces.md` を正本とする。

## 6. Authority / References

迷った場合の正本：

- 境界（層間の許可/禁止）：`docs/contracts/interfaces.md`
- 公開契約（DTO 形状・意味論）：`docs/contracts/contracts.md`
- endpoint 一覧（索引）：`docs/inventory/endpoints.md`
- 例外台帳：`docs/exceptions.md`
