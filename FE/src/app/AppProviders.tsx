import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { type ReactNode } from "react";
import { AuthProvider } from "react-oidc-context";
import { isMockApi } from "../config/env";
import { buildOidcConfig } from "../config/oidcConfig";
import {
  MockAccessTokenProvider,
  OidcAccessTokenBridge,
} from "./context/AccessTokenContext";
import { AppUiProvider } from "./ui/AppUiContext";
import { ApiAuthBridge } from "./ApiAuthBridge";

const client = new QueryClient({
  defaultOptions: {
    queries: { retry: 1, refetchOnWindowFocus: false },
  },
});

export function AppProviders({ children }: { children: ReactNode }) {
  if (isMockApi) {
    return (
      <AppUiProvider>
        <QueryClientProvider client={client}>
          <MockAccessTokenProvider>{children}</MockAccessTokenProvider>
        </QueryClientProvider>
      </AppUiProvider>
    );
  }
  return (
    <AppUiProvider>
      <QueryClientProvider client={client}>
        <AuthProvider {...buildOidcConfig()}>
          <OidcAccessTokenBridge>
            <ApiAuthBridge />
            {children}
          </OidcAccessTokenBridge>
        </AuthProvider>
      </QueryClientProvider>
    </AppUiProvider>
  );
}
