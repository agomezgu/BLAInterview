import { isMockApi } from "../../config/env";
import { useAuth } from "react-oidc-context";
import { Navigate, Outlet, useLocation } from "react-router";
import { Loading } from "../../components/Loading";

export function RequireAuth() {
  if (isMockApi) {
    return <Outlet />;
  }
  return <OidcOnly />;
}

function OidcOnly() {
  const auth = useAuth();
  const loc = useLocation();
  if (auth.isLoading) {
    return <Loading label="Checking your session…" />;
  }
  if (!auth.isAuthenticated) {
    return (
      <Navigate
        to={`/login?returnTo=${encodeURIComponent(loc.pathname + loc.search)}`}
        replace
      />
    );
  }
  return <Outlet />;
}
