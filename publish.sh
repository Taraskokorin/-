#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

for rid in win-x64 osx-arm64 linux-x64; do
  for project in HomeBudget.App HomeBudget.Desktop; do
    # Кожна оболонка – у власний каталог: дві самодостатні збірки
    # в одному каталозі перезаписали б файли середовища виконання.
    echo "== $rid / $project"
    dotnet publish "$project" -c Release -r "$rid" \
      --self-contained true -o "publish/$rid/$project"
  done
done

# Каталоги змінилися – право на виконання ставимо вже в нових.
chmod +x publish/*/HomeBudget.App/budget \
         publish/*/HomeBudget.Desktop/budget-ui 2>/dev/null || true
