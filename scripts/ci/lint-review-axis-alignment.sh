#!/usr/bin/env bash
set -euo pipefail

framework_path="docs/process/review-framework.md"
runbook_path="docs/process/codex-review-runbook.md"

if [[ ! -f "${framework_path}" ]]; then
  echo "ERROR: Missing ${framework_path}" >&2
  exit 1
fi

if [[ ! -f "${runbook_path}" ]]; then
  echo "ERROR: Missing ${runbook_path}" >&2
  exit 1
fi

mapfile -t framework_axes < <(
  awk '
  /# 品質軸/ {in_section=1; next}
  in_section && /^---$/ {exit}
  in_section {print}
  ' "${framework_path}" \
    | sed -nE 's/^[[:space:]]*[0-9]+\.[[:space:]]*([A-Za-z]+).*/\1/p' \
    | tr '[:lower:]' '[:upper:]' \
    | sort -u
)

if (( ${#framework_axes[@]} == 0 )); then
  echo "ERROR: Failed to read quality axes from ${framework_path}" >&2
  exit 1
fi

mapfile -t runbook_l2_axes < <(
  awk '
  /## 4\. L2 トリガ対応表/ {in_section=1; next}
  in_section && /^---$/ {exit}
  in_section {print}
  ' "${runbook_path}" \
    | grep -oE 'REVIEW-[A-Z-]+\.md' \
    | sed -E 's/^REVIEW-//; s/\.md$//' \
    | grep -vE '^(DOCS|USER-GUIDE)$' \
    | sort -u
)

if (( ${#runbook_l2_axes[@]} == 0 )); then
  echo "ERROR: Failed to read L2 template axes from ${runbook_path}" >&2
  exit 1
fi

replacement_line="$(grep -E '置き換えて使用' "${runbook_path}" | grep -E '`[A-Za-z-]+`' | head -n 1 || true)"
mapfile -t replacement_axes < <(
  printf '%s\n' "${replacement_line}" \
    | grep -oE '`[A-Za-z-]+`' \
    | tr -d '`' \
    | tr '[:lower:]' '[:upper:]' \
    | sort -u
)

errors=0

if [[ -z "${replacement_line}" || ${#replacement_axes[@]} -eq 0 ]]; then
  echo "ERROR: Failed to parse L2 axis replacement note from ${runbook_path}" >&2
  errors=1
fi

for axis in "${framework_axes[@]}"; do
  if ! printf '%s\n' "${runbook_l2_axes[@]}" | grep -qx "${axis}"; then
    echo "ERROR: L2 trigger table does not include axis ${axis}" >&2
    errors=1
  fi

  if ! printf '%s\n' "${replacement_axes[@]}" | grep -qx "${axis}"; then
    echo "ERROR: L2 axis replacement note does not include ${axis}" >&2
    errors=1
  fi
done

for axis in "${runbook_l2_axes[@]}"; do
  if ! printf '%s\n' "${framework_axes[@]}" | grep -qx "${axis}"; then
    echo "ERROR: L2 trigger table contains unknown axis ${axis}" >&2
    errors=1
  fi
done

if (( errors != 0 )); then
  exit 1
fi

echo "OK: review axis alignment lint passed (${#framework_axes[@]} axes)."
