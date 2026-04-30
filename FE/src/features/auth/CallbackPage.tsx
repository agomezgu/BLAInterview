import { isMockApi } from "../../config/env";
import { useEffect } from "react";
import { useAuth } from "react-oidc-context";
import { useNavigate } from "react-router";
import { Loading } from "../../components/Loading";

/**
 * `redirect_uri` for the OIDC code flow. `AuthProvider` completes the exchange; we then
 * return the user to `postLoginRedirect` in sessionStorage or `/tasks`.
 */
export function CallbackPage() {
  const nav = useNavigate();
  if (isMockApi) {
    return (
      <div className="page">
        <p>Callback is not used in mock mode.</p>
      </div>
    );
  }
  return <CallbackOidc nav={nav} />;
}

function CallbackOidc({ nav }: { nav: (to: string, o?: { replace: boolean }) => void }) {
  const auth = useAuth();
  useEffect(() => {
    if (auth.isLoading) return;
    if (auth.error) return;
    if (auth.isAuthenticated) {
      const p = sessionStorage.getItem("postLoginRedirect");
      if (p) {
        sessionStorage.removeItem("postLoginRedirect");
        void nav(p, { replace: true });
        return;
      }
      void nav("/tasks", { replace: true });
    }
  }, [auth.isAuthenticated, auth.isLoading, auth.error, nav]);
  if (auth.error) {
    return (
      <div className="page">
        <p className="field-error" role="alert">
          {auth.error.message}
        </p>
      </div>
    );
  }
  if (!auth.isLoading && !auth.isAuthenticated) {
    return <Loading label="Finishing sign-in…" />;
  }
  if (auth.isLoading) {
    return <Loading label="Finishing sign-in…" />;
  }
  return <Loading label="Redirecting…" />;
}
