#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if ! command -v dotnet >/dev/null 2>&1; then
  if [ -x "$HOME/.dotnet/dotnet" ]; then
    export DOTNET_ROOT="$HOME/.dotnet"
    export PATH="$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"
    sudo ln -sf "$HOME/.dotnet/dotnet" /usr/local/bin/dotnet 2>/dev/null || true
  else
    echo "dotnet was not found. Installing .NET 8 SDK into \$HOME/.dotnet ..."
    curl -fsSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0 --install-dir "$HOME/.dotnet"
    export DOTNET_ROOT="$HOME/.dotnet"
    export PATH="$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"
    sudo ln -sf "$HOME/.dotnet/dotnet" /usr/local/bin/dotnet 2>/dev/null || true
  fi
fi

cd "$ROOT/backend/CosmeticShop.Api"
exec dotnet run --launch-profile http
