#!/usr/bin/env bash

# Source this file from bash:
#   source scripts/bitflyer-live-age-env.example.sh

export BITFLYER_STAGE10_LIVE=1
unset BITFLYER_STAGE10_ALLOW_WRITE

export EXCHANGEAPI_AGE_IDENTITY_FILE_PATH="/path/to/age.key"
export EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH="/path/to/credentials.enc.json"

# Enable this explicitly only when you intend to run write live tests.
# export BITFLYER_STAGE10_ALLOW_WRITE=1
