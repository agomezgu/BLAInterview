import { useEffect } from "react";
import { useAuth } from "react-oidc-context";
import { configureApiClient } from "../services/apiClient";

export function ApiAuthBridge() {
  const auth = useAuth();
  useEffect(() => {
    configureApiClient({
      getAccessToken: () => auth.user?.access_token ?? null,
      onUnauthorized: () => {
        void auth.signinRedirect();
      },
    });
  }, [auth, auth.user?.access_token]);
  return null;
}
