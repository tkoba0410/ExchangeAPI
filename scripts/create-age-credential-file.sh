#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "${script_dir}/.." && pwd)"

venue=""
identity_path="${HOME}/.config/exchangeapi/keys/age.key"
new_identity_path=""
output_path=""
profile_path="${repo_root}/local/credentials/credential-profile.json"
label="default"
assume_yes=0

api_key=""
api_secret=""
credentials_json=""

cleanup_secret_variables() {
  unset api_key api_secret credentials_json
}
trap cleanup_secret_variables EXIT

usage() {
  cat <<'EOF'
ExchangeAPI age credential file 作成支援

使い方:
  scripts/create-age-credential-file.sh --venue bitflyer
  scripts/create-age-credential-file.sh --venue bitflyer --identity ~/.config/exchangeapi/keys/age.key
  scripts/create-age-credential-file.sh --venue bitflyer --new-identity ~/.config/exchangeapi/keys/age.key

options:
  --venue <venue>          対象 venue。bitflyer または binance。
  --identity <path>        既存の age identity file を使う。
  --new-identity <path>    age identity file を新規作成する。
  --output <path>          encrypted credentials file の出力先。
  --profile <path>         credential profile の出力先。
  --label <label>          credentials JSON の optional label。
  --yes                    起動時確認と入力直前確認を省略する。
  -h, --help               この help を表示する。secret 入力は行わない。

既定:
  identity: ~/.config/exchangeapi/keys/age.key
  output:   local/credentials/current/<venue>.age
  profile:  local/credentials/credential-profile.json

この script は API key / API secret を command line 引数では受け取りません。
EOF
}

fail() {
  echo "error: $*" >&2
  exit 1
}

confirm() {
  local prompt="$1"
  if [[ "${assume_yes}" == "1" ]]; then
    return 0
  fi

  local answer
  read -r -p "${prompt} [y/N]: " answer
  [[ "${answer}" == "y" || "${answer}" == "Y" ]]
}

require_command() {
  local name="$1"
  command -v "${name}" >/dev/null 2>&1 || fail "${name} が見つかりません。先にインストールしてください。"
}

expand_path() {
  local path="$1"
  if [[ "${path}" == "~" ]]; then
    printf '%s\n' "${HOME}"
  elif [[ "${path}" == "~/"* ]]; then
    printf '%s/%s\n' "${HOME}" "${path#~/}"
  elif [[ "${path}" == /* ]]; then
    printf '%s\n' "${path}"
  else
    printf '%s/%s\n' "${repo_root}" "${path}"
  fi
}

relative_to_profile_dir() {
  local target_path="$1"
  local profile_dir="$2"
  python3 - "$target_path" "$profile_dir" <<'PY'
import os
import sys

target = os.path.abspath(sys.argv[1])
base = os.path.abspath(sys.argv[2])
print(os.path.relpath(target, base))
PY
}

json_escape() {
  local value="$1"
  value=${value//\\/\\\\}
  value=${value//\"/\\\"}
  value=${value//$'\n'/\\n}
  value=${value//$'\r'/\\r}
  value=${value//$'\t'/\\t}
  printf '%s' "${value}"
}

parse_args() {
  while [[ "$#" -gt 0 ]]; do
    case "$1" in
      --venue)
        [[ "$#" -ge 2 ]] || fail "--venue requires a value."
        venue="$2"
        shift 2
        ;;
      --identity)
        [[ "$#" -ge 2 ]] || fail "--identity requires a value."
        identity_path="$2"
        shift 2
        ;;
      --new-identity)
        [[ "$#" -ge 2 ]] || fail "--new-identity requires a value."
        new_identity_path="$2"
        identity_path="$2"
        shift 2
        ;;
      --output)
        [[ "$#" -ge 2 ]] || fail "--output requires a value."
        output_path="$2"
        shift 2
        ;;
      --profile)
        [[ "$#" -ge 2 ]] || fail "--profile requires a value."
        profile_path="$2"
        shift 2
        ;;
      --label)
        [[ "$#" -ge 2 ]] || fail "--label requires a value."
        label="$2"
        shift 2
        ;;
      --yes)
        assume_yes=1
        shift
        ;;
      -h|--help)
        usage
        exit 0
        ;;
      *)
        fail "unknown argument: $1"
        ;;
    esac
  done
}

validate_args() {
  [[ -n "${venue}" ]] || fail "--venue is required. bitflyer または binance を指定してください。"
  case "${venue}" in
    bitflyer|binance)
      ;;
    *)
      fail "unsupported venue: ${venue}. bitflyer または binance を指定してください。"
      ;;
  esac

  if [[ -n "${new_identity_path}" && ! "${new_identity_path}" == "${identity_path}" ]]; then
    fail "--identity and --new-identity cannot point to different files."
  fi
}

print_start_explanation() {
  cat <<EOF
ExchangeAPI age credential file 作成支援

このスクリプトは、ExchangeAPI が private API 呼び出しで使用する
API key / API secret を、age で暗号化された credentials file として作成します。

このスクリプトが行うこと:
- API key / API secret を対話入力で受け取ります。
- 入力値から ExchangeAPI 用の credentials JSON をメモリ上で作成します。
- 平文ファイルは作成せず、JSON をそのまま age に渡して暗号化します。
- 暗号化済みファイルを local/credentials/current/<venue>.age に保存します。
- 必要に応じて age identity key と credential profile / symlink を作成します。

このスクリプトが行わないこと:
- API key / API secret をコマンドライン引数で受け取りません。
- API key / API secret を画面に表示しません。
- 平文の credentials JSON をファイルに保存しません。
- 取引所 API に接続しません。
- 入力された API key / API secret の有効性確認は行いません。
- secret manager / keychain / 外部サービスには接続しません。

作成・更新される可能性があるファイル:
- ${identity_path}
- local/credentials/current/age-identity.txt
- ${output_path}
- ${profile_path}
EOF

  confirm "続行しますか？" || exit 1
}

print_input_explanation() {
  cat <<'EOF'

API key / API secret の入力

これから API key と API secret を入力します。

入力された値は次の目的だけに使われます:
- ExchangeAPI 用の credentials JSON をメモリ上で作成する
- その JSON を age に渡して暗号化する
- 暗号化済み credentials file を作成する

入力された値について:
- 画面には表示されません
- コマンドライン履歴には残りません
- 平文ファイルとして保存されません
- このスクリプト内で取引所 API へ送信されません
- ネットワーク送信されません
- 暗号化処理が終わったら script 変数から破棄します

注意:
- 端末の録画、監査ツール、特殊な shell 設定がある場合は、その環境側の記録までは制御できません
- 入力中に貼り付けた内容は端末や OS の clipboard 履歴に残る場合があります
- 不安がある場合は、この処理を中止し、スクリプトの内容を確認してから再実行してください
EOF

  confirm "API key / secret の入力に進みますか？" || exit 1
}

read_secret_inputs() {
  echo
  echo "API key を入力してください。入力内容は表示されません。Enter で確定します。"
  read -r -s -p "API key: " api_key
  printf '\n'
  echo "API secret を入力してください。入力内容は表示されません。Enter で確定します。"
  read -r -s -p "API secret: " api_secret
  printf '\n'

  [[ -n "${api_key//[[:space:]]/}" ]] || fail "API key is empty."
  [[ -n "${api_secret//[[:space:]]/}" ]] || fail "API secret is empty."

  echo
  echo "API key / API secret を受け取りました。値は表示しません。"
  echo "これから age で暗号化ファイルを作成します。"
}

ensure_identity() {
  local identity_dir
  identity_dir="$(dirname "${identity_path}")"
  mkdir -p "${identity_dir}"

  if [[ -n "${new_identity_path}" && -e "${identity_path}" ]]; then
    fail "new identity already exists: ${identity_path}"
  fi

  if [[ ! -e "${identity_path}" ]]; then
    confirm "age identity が見つかりません: ${identity_path}。新規作成しますか？" || exit 1
    age-keygen -o "${identity_path}" >/dev/null 2>&1
  fi

  [[ -f "${identity_path}" ]] || fail "age identity file does not exist: ${identity_path}"
  chmod 600 "${identity_path}"
}

encrypt_credentials() {
  local recipient
  local generated_at

  recipient="$(age-keygen -y "${identity_path}")"
  [[ -n "${recipient}" ]] || fail "failed to derive age recipient."

  generated_at="$(date --iso-8601=seconds)"
  credentials_json="$(printf '{\n  "version": 1,\n  "venue": "%s",\n  "apiKey": "%s",\n  "apiSecret": "%s",\n  "label": "%s",\n  "generatedAt": "%s"\n}\n' \
    "$(json_escape "${venue}")" \
    "$(json_escape "${api_key}")" \
    "$(json_escape "${api_secret}")" \
    "$(json_escape "${label}")" \
    "$(json_escape "${generated_at}")")"

  mkdir -p "$(dirname "${output_path}")"
  if [[ -e "${output_path}" ]]; then
    confirm "既存の encrypted credentials file を上書きします: ${output_path}" || exit 1
  fi

  printf '%s' "${credentials_json}" | age -r "${recipient}" -o "${output_path}"
  chmod 600 "${output_path}"
}

write_symlink() {
  local link_path="${repo_root}/local/credentials/current/age-identity.txt"
  mkdir -p "$(dirname "${link_path}")"

  if [[ -e "${link_path}" || -L "${link_path}" ]]; then
    local current_target
    current_target="$(readlink "${link_path}" 2>/dev/null || true)"
    if [[ "${current_target}" != "${identity_path}" ]]; then
      confirm "既存の age identity symlink を更新します: ${link_path}" || exit 1
      ln -sfn "${identity_path}" "${link_path}"
    fi
  else
    ln -s "${identity_path}" "${link_path}"
  fi
}

write_profile() {
  local profile_dir
  local identity_profile_path
  local credentials_profile_path
  profile_dir="$(dirname "${profile_path}")"
  mkdir -p "${profile_dir}"

  if [[ -e "${profile_path}" ]]; then
    confirm "credential profile の ${venue} entry を作成または更新します: ${profile_path}" || exit 1
  fi

  identity_profile_path="$(relative_to_profile_dir "${repo_root}/local/credentials/current/age-identity.txt" "${profile_dir}")"
  credentials_profile_path="$(relative_to_profile_dir "${output_path}" "${profile_dir}")"

  python3 - "${profile_path}" "${venue}" "${identity_profile_path}" "${credentials_profile_path}" <<'PY'
import json
import os
import sys

profile_path, venue, identity_path, credentials_path = sys.argv[1:5]

if os.path.exists(profile_path):
    with open(profile_path, "r", encoding="utf-8") as handle:
        profile = json.load(handle)
else:
    profile = {"version": 1, "credentials": {}}

if not isinstance(profile, dict):
    raise SystemExit("credential profile must be a JSON object")

profile["version"] = int(profile.get("version", 1))
credentials = profile.get("credentials")
if not isinstance(credentials, dict):
    credentials = {}
    profile["credentials"] = credentials

credentials[venue] = {
    "provider": "age-file",
    "identityFilePath": identity_path,
    "credentialsFilePath": credentials_path,
}

with open(profile_path, "w", encoding="utf-8") as handle:
    json.dump(profile, handle, ensure_ascii=False, indent=2)
    handle.write("\n")
PY
}

print_completion() {
  local link_path="${repo_root}/local/credentials/current/age-identity.txt"

  cat <<EOF

完了しました。

生成・更新したファイル:

1. age identity key
   path:
     ${identity_path}
   内容:
     age の復号用秘密鍵です。
     暗号化済み credentials file を復号するために使います。
   注意:
     API key / API secret そのものではありません。
     ただし、この鍵を持つ人は .age ファイルを復号できます。
     他人に渡さないでください。

2. age identity symlink
   path:
     ${link_path}
   内容:
     ${identity_path} への symlink です。
   目的:
     ExchangeAPI の credential profile から repo-local path として参照するためです。
   注意:
     local/ 配下なので git commit 対象ではありません。

3. encrypted credentials file
   path:
     ${output_path}
   内容:
     API key / API secret を含む credentials JSON を age で暗号化したものです。
   平文として復号される情報:
     - venue: ${venue}
     - apiKey: 入力された API key
     - apiSecret: 入力された API secret
     - label: ${label}
     - generatedAt
   注意:
     ファイル自体は暗号化されています。
     ただし、復号すると API key / API secret を含みます。
     平文 credentials JSON は保存していません。

4. credential profile
   path:
     ${profile_path}
   内容:
     ExchangeAPI がどの encrypted credentials file と identity key を使うかを示す設定です。
   例:
     ${venue} -> local/credentials/current/${venue}.age
     identity -> local/credentials/current/age-identity.txt
   注意:
     API key / API secret は含みません。
     local/ 配下なので git commit 対象ではありません。

作成していないもの:
- 平文 credentials JSON file
- API key / API secret を含む log file
- API key / API secret を含む shell command history

次に ExchangeAPI を実行すると、この profile を通じて encrypted credentials file が読み込まれます。
EOF
}

main() {
  if [[ "$-" == *x* ]]; then
    fail "shell xtrace(set -x) が有効です。secret 入力を扱うため、set +x してから再実行してください。"
  fi

  parse_args "$@"
  validate_args

  identity_path="$(expand_path "${identity_path}")"
  profile_path="$(expand_path "${profile_path}")"
  if [[ -z "${output_path}" ]]; then
    output_path="${repo_root}/local/credentials/current/${venue}.age"
  else
    output_path="$(expand_path "${output_path}")"
  fi

  require_command age
  require_command age-keygen
  require_command python3

  print_start_explanation
  ensure_identity
  print_input_explanation
  read_secret_inputs
  encrypt_credentials
  cleanup_secret_variables
  write_symlink
  write_profile
  print_completion
}

main "$@"
