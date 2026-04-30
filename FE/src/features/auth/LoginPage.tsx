import { isMockApi } from "../../config/env";
import { useMemo, useState } from "react";
import { useAuth } from "react-oidc-context";
import { Link, useSearchParams } from "react-router";
import { Button } from "../../components/Button";
import { ErrorAlert } from "../../components/ErrorAlert";
import { MockModeBanner } from "../../components/MockModeBanner";
import { Loading } from "../../components/Loading";

export function LoginPage() {
  if (isMockApi) {
    return (
      <>
        <MockModeBanner />
        <div className="page auth-page">
          <h1>Sign in</h1>
          <p>Mock mode is on; the IDP is not used. Open tasks directly.</p>
          <p>
            <Link to="/tasks" className="link-button">
              Go to tasks
            </Link>
          </p>
        </div>
      </>
    );
  }
  return <OidcLoginContent />;
}

function OidcLoginContent() {
  const auth = useAuth();
  const [err, setErr] = useState<string | null>(null);
  const [sp] = useSearchParams();
  const returnTo = sp.get("returnTo") || "/tasks";
  const oidcUrlError = useMemo(() => {
    const c = sp.get("error");
    if (!c) {
      return null;
    }
    return sp.get("error_description") || c;
  }, [sp]);
  if (auth.isLoading) {
    return <Loading label="Loading sign-in…" />;
  }
  if (auth.isAuthenticated) {
    return (
      <div className="page auth-page">
        <p>You are already signed in.</p>
        <p>
          <Link to={returnTo} className="link-button">
            Continue
          </Link>
        </p>
      </div>
    );
  }
  return (
    <div className="page auth-page">
      <h1>Sign in</h1>
      <p>Sign in with your organization account to manage tasks.</p>
      <ErrorAlert message={oidcUrlError || err} />
      <p>
        <Button
          type="button"
          onClick={async () => {
            setErr(null);
            try {
              sessionStorage.setItem("postLoginRedirect", returnTo);
              await auth.signinRedirect();
            } catch (e) {
              setErr(e instanceof Error ? e.message : "Sign-in failed");
            }
          }}
        >
          Sign in with IDP
        </Button>
      </p>
    </div>
  );
}
