/* eslint-disable react-refresh/only-export-components -- context + hook pattern */
import { createContext, useContext, type ReactNode } from "react";
import { useAuth } from "react-oidc-context";

const AccessTokenContext = createContext<string | null>(null);

/**
 * Injects OIDC `access_token` for API calls. Only mount when wrapped by AuthProvider
 * and VITE_MOCK_API is false.
 */
export function OidcAccessTokenBridge({ children }: { children: ReactNode }) {
  const auth = useAuth();
  const t = auth.user?.access_token ?? null;
  return (
    <AccessTokenContext.Provider value={t}>{children}</AccessTokenContext.Provider>
  );
}

export function useAccessTokenValue() {
  return useContext(AccessTokenContext);
}

export function MockAccessTokenProvider({ children }: { children: ReactNode }) {
  return <AccessTokenContext.Provider value={null}>{children}</AccessTokenContext.Provider>;
}
