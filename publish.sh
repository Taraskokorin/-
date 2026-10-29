#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

for rid in win-x64 osx-arm64 linux-x64; do
  echo "== $rid"
  dotnet publish HomeBudget.App -c Release -r "$rid" \
    --self-contained true -o "publish/$rid"
done

# Право на виконання губиться в zip і у файлообмінниках.
chmod +x publish/linux-x64/budget \
         publish/osx-arm64/budget 2>/dev/null || true
