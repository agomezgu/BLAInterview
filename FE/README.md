# Task app (React + Vite)

Production-oriented SPA for task management with **OIDC/OAuth2 Authorization Code + PKCE** and a **.NET Web API** (`GET/POST/PUT/DELETE /tasks`).

## Quick start

```bash
cd FE
npm install
cp .env.example .env
# For UI-only dev without a real IDP, set in .env:
# VITE_MOCK_API=true
# VITE_API_BASE_URL=/api
npm run dev
```

Open `http://localhost:5173` (Vite default). The home route redirects to `/tasks`.

## Environment variables

| Variable | Purpose |
|----------|---------|
| `VITE_API_BASE_URL` | API origin or `/api` (use with Vite proxy in dev) |
| `VITE_MOCK_API` | `true` = in-memory tasks, no real HTTP/IDP |
| `VITE_OIDC_*` | Authority, client id, redirect URIs, **scope** (must include your API audience, e.g. `bla-interview-api`) |
| `VITE_OIDC_REGISTRATION_URL` | Hosted “create account” page (preferred) |
| `VITE_USE_HOSTED_REGISTRATION` | `true` = link to hosted URL; `false` = optional public `VITE_REGISTRATION_API_URL` |
| `VITE_REGISTRATION_API_URL` | Public register endpoint if your IDP exposes one—**customize the request body** in `RegisterPage.tsx` |

**Never** put client secrets in the frontend. The SPA uses a public client with PKCE.

## Dev: API without CORS pain

1. Run the Web API (e.g. Kestrel on `https://localhost:7205`).
2. In `vite.config.ts`, the dev server proxies `/api` to that target.
3. Set `VITE_API_BASE_URL=/api` in `.env` so the browser calls the same origin.

**Production:** set `VITE_API_BASE_URL` to your API’s public URL and **enable CORS** on the API for your SPA origin (the repo’s Web API did not add CORS by default).

## OIDC client (IDP) checklist

- Register a **public** SPA client with **PKCE** (S256).
- **Redirect URI:** `http://localhost:5173/auth/callback` (and production URL).
- **Post-logout redirect:** e.g. `http://localhost:5173/login`.
- **Scopes:** `openid`, `profile`, and the **API resource scope** your backend expects (default in this repo: `bla-interview-api` in `Program.cs`).

If the access token does not include that scope, the API returns **403**.

## Backend contract (this repo’s API)

- `GET /tasks` — list; `GET /tasks/{id}`; `POST /tasks` with `{ "title": string }` only today; `PUT` with optional title/description/priority/status; `DELETE` → 204.
- The create form can collect more fields, but only **`title` is sent** on `POST` until the server extends `CreateTaskDto`. See `src/services/taskApi.ts` and the note on the create page.

## Customization map

1. **IDP:** all `VITE_OIDC_*` in `.env`, plus client registration in the IDP admin UI.
2. **API base URL and paths:** `src/config/env.ts`, `src/services/apiPaths.ts`, `src/services/taskApi.ts`.
3. **Priority/status labels:** `src/features/tasks/constants.ts` (replace with OpenAPI or shared types when available).
4. **Public registration body:** `src/features/auth/RegisterPage.tsx` if you use `VITE_REGISTRATION_API_URL`.
5. **App-wide UI state (flash, etc.):** `src/app/ui/AppUiContext.tsx`.
6. **CORS (production):** your ASP.NET `Program.cs` — `AddCors` for the SPA origin.

## Scripts

- `npm run dev` — Vite dev server
- `npm run build` — typecheck + production bundle
- `npm run preview` — serve `dist/`

## Folder layout (key files)

- `src/app` — `AppProviders`, `AppLayout`, `AppUiContext`, `globals.css`
- `src/config` — `env`, `oidcConfig`
- `src/features/auth` — login, register, callback, `RequireAuth`
- `src/features/tasks` — list, detail, create, edit, delete
- `src/routes` — `router.tsx` (`/login`, `/register`, `/auth/callback`, `/tasks`, …)
- `src/services` — `apiClient`, `taskApi`, `mockTaskStore`
- `src/types` — `task`, `apiError`
- `src/components` — shared form controls, `FlashBar`, etc.
