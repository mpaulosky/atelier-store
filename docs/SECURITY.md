# Security Policy

## Supported versions

Atelier Store is an early-stage project, and releases are tagged `v0.0.x`.
Only the latest release receives security fixes.
Fixes land on `main` and ship in the next release.

| Version | Supported |
| --- | --- |
| Latest `v0.0.x` release | :white_check_mark: |
| Older releases | :x: |

## Reporting a vulnerability

Please don't open a public issue, discussion, or pull request for a security vulnerability.

Report it privately instead, in one of these ways:

- **Recommended:** open a private report from the repository's **Security** tab (**Report a vulnerability**).
  Only the maintainers can see it, and we can work on a fix together in a private advisory.
- **Email:** <matthew.paulosky@outlook.com>, with the subject `[SECURITY] Atelier Store vulnerability report`.

### What to include

1. A description of the vulnerability
1. Its impact, and how severe you think it is
1. Steps to reproduce it, or a proof of concept
1. The affected release or commit
1. A suggested fix, if you have one
1. How we can contact you for follow-up

### What to expect

- **First response:** within 48 hours of your report
- **Assessment:** within 7 days, with a plan and a timeline
- **Fix:**
  - Critical: within 7 days
  - High: within 14 days
  - Medium or low: within 30 days

### Disclosure

- We'll confirm the vulnerability with you and keep you updated while we fix it.
- We'll publish the fix before disclosing any details.
- We'll credit you in the advisory, unless you'd rather stay anonymous.
- Please don't disclose the vulnerability publicly until a fixed release is out.

Fixes are announced in a [GitHub Security Advisory](https://github.com/mpaulosky/atelier-store/security/advisories) and in the release notes.

## How the app is protected

### Sign-in

- Sign-in is handled by Auth0 using OpenID Connect.
  The app never sees or stores passwords.
- After sign-in, the session is kept in an ASP.NET Core authentication cookie.
- Signing out ends both the app's cookie session and the Auth0 session.
- `/account/login` only accepts a relative `returnUrl`, so it can't be used to redirect people to another site.

### Requests and data

- HTTPS is enforced through HTTPS redirection, with HSTS outside Development.
- Forms are protected by ASP.NET Core anti-forgery tokens.
- Blazor HTML-encodes rendered content.
- Database access goes through EF Core, which sends parameterized queries.
- Database check constraints reject invalid catalog data, such as negative prices or stock, and slugs that aren't lowercase.
- Outside Development, unhandled errors show a generic error page instead of details.

### Secrets

- Secrets come from a local `.env` file or from real environment variables.
  `.env` is in `.gitignore`, and `appsettings.json` deliberately leaves secret keys blank.
- GitHub secret scanning and push protection are turned on, so a push that contains a known secret format is blocked.
- The integration and E2E tests use a fake Auth0 tenant and a throwaway Postgres container, never real credentials.

### Dependencies and CI

- Dependabot keeps NuGet packages, the .NET SDK, npm packages (the Tailwind CLI), and GitHub Actions up to date, and opens security updates.
- NuGet versions are managed centrally in `Directory.Packages.props`.
- CodeQL scans the C# code and the GitHub Actions workflows.
- zizmor, actionlint, and shellcheck lint the workflows and scripts.
- Every action a workflow uses is pinned to a commit SHA or an image digest.

## Known limitations

These are known gaps, not vulnerabilities.
Suggestions and pull requests are welcome.

- **No security headers:** there is no Content Security Policy, `X-Frame-Options`, or similar header yet.
- **No rate limiting:** the sign-in endpoints and pages have no rate limits.
- **Sign-out is a GET request:** another site could sign a visitor out by linking to `/account/logout`.
  That's a nuisance, but it doesn't expose any data.
- **Public health check:** `/health` is public and reports whether the database is reachable.
- **No audit logging:** sign-ins and other user actions aren't logged for audit.
- **HSTS lasts 30 days:** this is the ASP.NET Core default, and production may want longer.

## For contributors

- Never commit secrets, API keys, tokens, or a filled-in `.env`.
- Keep configuration values that must stay private in environment variables, not in `appsettings*.json`.
- Add tests for security-relevant behavior, such as redirects, sign-out, and authorization.
- Check for vulnerable packages with `dotnet list package --vulnerable`.
- See [CONTRIBUTING.md](CONTRIBUTING.md) for how changes are reviewed and merged.

## Resources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [ASP.NET Core security](https://learn.microsoft.com/aspnet/core/security/)
- [Blazor security](https://learn.microsoft.com/aspnet/core/blazor/security/)
- [Auth0 ASP.NET Core SDK](https://github.com/auth0/auth0-aspnetcore-authentication)
