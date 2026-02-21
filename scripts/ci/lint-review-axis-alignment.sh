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

allowed_non_framework_axes=("DOCS" "USER-GUIDE")
axis_universe=("${framework_axes[@]}" "${allowed_non_framework_axes[@]}")

mapfile -t replacement_section_lines < <(
  awk '
  /### 5\.2 L2（軸別）レビュー/ {in_section=1; next}
  in_section && /^### / {exit}
  in_section {print}
  ' "${runbook_path}"
)

replacement_line=""
replacement_axis_count=0

for line in "${replacement_section_lines[@]}"; do
  mapfile -t line_tokens < <(
    printf '%s\n' "${line}" \
      | grep -oE '`[A-Za-z-]+`' \
      | tr -d '`' \
      | tr '[:lower:]' '[:upper:]' \
      | sort -u || true
  )

  if (( ${#line_tokens[@]} == 0 )); then
    continue
  fi

  line_axis_count=0
  for token in "${line_tokens[@]}"; do
    if printf '%s\n' "${axis_universe[@]}" | grep -qx "${token}"; then
      line_axis_count=$((line_axis_count + 1))
    fi
  done

  if (( line_axis_count > replacement_axis_count )); then
    replacement_axis_count="${line_axis_count}"
    replacement_line="${line}"
  fi
done

mapfile -t replacement_axes < <(
  printf '%s\n' "${replacement_line}" \
    | grep -oE '`[A-Za-z-]+`' \
    | tr -d '`' \
    | tr '[:lower:]' '[:upper:]' \
    | sort -u || true
)

errors=0

if [[ -z "${replacement_line}" || ${#replacement_axes[@]} -eq 0 || ${replacement_axis_count} -eq 0 ]]; then
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

for axis in "${replacement_axes[@]}"; do
  if ! printf '%s\n' "${axis_universe[@]}" | grep -qx "${axis}"; then
    echo "ERROR: L2 axis replacement note contains unknown axis ${axis}" >&2
    errors=1
  fi
done

if (( errors != 0 )); then
  exit 1
fi

echo "OK: review axis alignment lint passed (${#framework_axes[@]} axes)."
