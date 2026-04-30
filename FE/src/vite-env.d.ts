/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL: string;
  readonly VITE_MOCK_API: string;
  readonly VITE_OIDC_AUTHORITY: string;
  readonly VITE_OIDC_CLIENT_ID: string;
  readonly VITE_OIDC_REDIRECT_URI: string;
  readonly VITE_OIDC_POST_LOGOUT_REDIRECT_URI: string;
  readonly VITE_OIDC_SCOPE: string;
  readonly VITE_OIDC_REGISTRATION_URL: string;
  readonly VITE_USE_HOSTED_REGISTRATION: string;
  readonly VITE_REGISTRATION_API_URL: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
