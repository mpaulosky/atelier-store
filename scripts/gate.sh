#!/usr/bin/env bash
# Local quality gate, run by .github/hooks/pre-push and safe to run by hand.
# Lints the Markdown and YAML files changed since origin/main (every unpushed
# commit, with the same configs as CI), builds the solution, then runs each
# test project under tests/. Exits non-zero on the first failing gate.
set -euo pipefail

ROOT="$(git rev-parse --show-toplevel)"
cd "$ROOT"

RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'; CYAN='\033[0;36m'; RESET='\033[0m'

step() { echo -e "\n${CYAN}▶ $1${RESET}"; }

# Files added, copied, modified or renamed since this branch left origin/main.
# Without an origin/main (a fresh clone of a fork, say) lint every tracked file.
if BASE="$(git merge-base HEAD origin/main 2>/dev/null)"; then
  CHANGED="$(git diff --name-only --diff-filter=ACMR "$BASE" HEAD)"
else
  CHANGED="$(git ls-files)"
fi

mapfile -t MD_FILES < <(grep -E '\.md$' <<< "$CHANGED" | grep -Ev '^docs/blogs/' || true)
mapfile -t YAML_FILES < <(grep -E '\.ya?ml$' <<< "$CHANGED" | grep -Ev 'pnpm-lock\.yaml$' || true)

step "Markdown lint (${#MD_FILES[@]} changed file(s))"
if [[ ${#MD_FILES[@]} -gt 0 ]]; then
  npx --yes markdownlint-cli2 "${MD_FILES[@]}"
fi

step "YAML lint (${#YAML_FILES[@]} changed file(s))"
if [[ ${#YAML_FILES[@]} -gt 0 ]]; then
  if command -v yamllint &>/dev/null; then
    YAMLLINT_ARGS=()
    [[ -f .yamllint.yml ]] && YAMLLINT_ARGS=(-c .yamllint.yml)
    yamllint "${YAMLLINT_ARGS[@]}" "${YAML_FILES[@]}"
  else
    echo -e "${YELLOW}⚠️  yamllint not found — skipping. CI's Lint YAML workflow still checks these files.${RESET}"
    echo -e "   To enable: ${CYAN}pipx install yamllint${RESET}"
  fi
fi

step "Build"
mapfile -t SOLUTIONS < <(find . -maxdepth 1 -name '*.slnx')
dotnet build "${SOLUTIONS[@]}" --configuration Release -warnaserror

step "Tests"
mapfile -t TEST_PROJECTS < <(find tests -mindepth 2 -maxdepth 2 -name '*.csproj' 2>/dev/null | sort)
if [[ ${#TEST_PROJECTS[@]} -eq 0 ]]; then
  echo -e "${YELLOW}No test projects under tests/ — skipping.${RESET}"
fi
for project in "${TEST_PROJECTS[@]}"; do
  dotnet test "$project" --configuration Release
done

echo -e "\n${GREEN}✅ Gate passed.${RESET}"
