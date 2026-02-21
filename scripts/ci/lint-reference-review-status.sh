#!/usr/bin/env bash
set -euo pipefail

reviews_dir="docs/reference/reviews"
readme_path="${reviews_dir}/README.md"

if [[ ! -f "${readme_path}" ]]; then
  echo "ERROR: README not found: ${readme_path}" >&2
  exit 1
fi

mapfile -t review_basenames < <(
  find "${reviews_dir}" -maxdepth 1 -type f -name '*.md' ! -name 'README.md' -printf '%f\n' | sort
)

if (( ${#review_basenames[@]} == 0 )); then
  echo "ERROR: No review documents found under ${reviews_dir}" >&2
  exit 1
fi

declare -A file_status=()
declare -A table_status=()
errors=0

for base in "${review_basenames[@]}"; do
  path="${reviews_dir}/${base}"
  count="$(grep -cE '^Status: (Active|Archived)$' "${path}" || true)"
  if [[ "${count}" != "1" ]]; then
    echo "ERROR: ${path} must contain exactly one 'Status: Active|Archived' header (found: ${count})." >&2
    errors=1
    continue
  fi

  status="$(sed -nE 's/^Status: (Active|Archived)$/\1/p' "${path}")"
  file_status["${base}"]="${status}"
done

mapfile -t table_rows < <(
  grep -E '^\| \[[^]]+\]\(\./[^)]+\.md\) \| (Active|Archived) \|.*\|$' "${readme_path}" || true
)

if (( ${#table_rows[@]} == 0 )); then
  echo "ERROR: No status rows found in ${readme_path}" >&2
  errors=1
else
  for row in "${table_rows[@]}"; do
    base="$(sed -nE 's#^\| \[[^]]+\]\(\./([^)]+\.md)\) \| (Active|Archived) \|.*\|$#\1#p' <<< "${row}")"
    status="$(sed -nE 's#^\| \[[^]]+\]\(\./([^)]+\.md)\) \| (Active|Archived) \|.*\|$#\2#p' <<< "${row}")"

    if [[ -z "${base}" || -z "${status}" ]]; then
      echo "ERROR: Failed to parse status row: ${row}" >&2
      errors=1
      continue
    fi

    if [[ -n "${table_status[${base}]+x}" ]]; then
      echo "ERROR: Duplicate status row for ${base} in ${readme_path}" >&2
      errors=1
      continue
    fi

    table_status["${base}"]="${status}"
  done
fi

for base in "${review_basenames[@]}"; do
  if [[ -z "${table_status[${base}]+x}" ]]; then
    echo "ERROR: Missing status row in README for ${base}" >&2
    errors=1
    continue
  fi

  if [[ "${table_status[${base}]}" != "${file_status[${base}]}" ]]; then
    echo "ERROR: Status mismatch for ${base}: file=${file_status[${base}]} readme=${table_status[${base}]}" >&2
    errors=1
  fi
done

for base in "${!table_status[@]}"; do
  if [[ -z "${file_status[${base}]+x}" ]]; then
    echo "ERROR: README has extra status row for non-review file ${base}" >&2
    errors=1
  fi
done

if (( errors != 0 )); then
  exit 1
fi

echo "OK: reference review status lint passed (${#review_basenames[@]} files)."
