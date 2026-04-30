import type { AuthProviderProps } from "react-oidc-context";
import { env } from "./env";

/**
 * OIDC client settings for public SPA (PKCE is default in oidc-client-ts).
 * Customize: VITE_OIDC_AUTHORITY, client_id, redirect URIs, and scope
 * (must request your API resource scope, e.g. `bla-interview-api`).
 */
export function buildOidcConfig(): AuthProviderProps {
  const { origin } = window.location;
  return {
    authority: env.VITE_OIDC_AUTHORITY,
    client_id: env.VITE_OIDC_CLIENT_ID,
    redirect_uri: env.VITE_OIDC_REDIRECT_URI || `${origin}/auth/callback`,
    post_logout_redirect_uri:
      env.VITE_OIDC_POST_LOGOUT_REDIRECT_URI || origin + "/",
    scope: env.VITE_OIDC_SCOPE,
    onSigninCallback: () => {
      window.history.replaceState({}, document.title, window.location.pathname);
    },
  } satisfies AuthProviderProps;
}
