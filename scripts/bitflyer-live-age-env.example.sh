#!/usr/bin/env bash

# Source this file from bash:
#   source scripts/bitflyer-live-age-env.example.sh

export EXCHANGEAPI_AGE_IDENTITY_FILE_PATH="/path/to/age.key"
export EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH="/path/to/credentials.enc.json"

# Live protocol debug logs are written under:
#   local/logs/bitflyer/live-tests/
#
# Enable write live tests explicitly by creating this local marker file:
#   touch local/bitflyer-live-write-enabled
#
# Enable CancelAllChildOrders live tests explicitly:
#   touch local/bitflyer-live-cancel-all-enabled
#
# Enable Withdraw negative live tests explicitly:
#   touch local/bitflyer-live-withdraw-negative-enabled
