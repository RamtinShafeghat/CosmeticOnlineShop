#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT/frontend"

if [ ! -d node_modules ]; then
  npm install
fi

# Bind on 0.0.0.0 so Cursor Desktop port forwarding can reach the app.
exec npx ng serve --host 0.0.0.0 --port 4200
