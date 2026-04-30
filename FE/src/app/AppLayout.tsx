import { env, isMockApi } from "../config/env";
import { useAuth } from "react-oidc-context";
import { Link, Outlet, useNavigate } from "react-router";
import { Button } from "../components/Button";
import { FlashBar } from "../components/FlashBar";
import { Loading } from "../components/Loading";
import { MockModeBanner } from "../components/MockModeBanner";

export function AppLayout() {
  if (isMockApi) {
    return (
      <div className="app-shell">
        <MockModeBanner />
        <FlashBar />
        <header className="app-header">
          <div className="app-header__row">
            <Link to="/tasks" className="app-brand">
              Task app
            </Link>
          </div>
          <nav className="app-nav">
            <Link to="/tasks">Tasks</Link>
            <Link to="/tasks/new">New task</Link>
          </nav>
          <div className="app-header__user">
            <span title="mock-user">Mock user (mock-user)</span>
            <span className="app-header__sep" aria-hidden />
            <span className="app-header__hint">VITE_MOCK_API is on</span>
          </div>
        </header>
        <main className="app-main">
          <Outlet />
        </main>
      </div>
    );
  }
  return <AppLayoutOidc />;
}

function AppLayoutOidc() {
  const auth = useAuth();
  const nav = useNavigate();
  if (auth.isLoading) {
    return <Loading label="Session…" />;
  }
  const p = auth.user?.profile;
  const name =
    (typeof p?.name === "string" && p.name) ||
    (typeof p?.preferred_username === "string" && p.preferred_username) ||
    (typeof p?.sub === "string" && p.sub) ||
    "User";
  const sub = (typeof p?.sub === "string" && p.sub) || "—";
  return (
    <div className="app-shell">
      <FlashBar />
      <header className="app-header">
        <div className="app-header__row">
          <Link to="/tasks" className="app-brand">
            Task app
          </Link>
        </div>
        <nav className="app-nav">
          <Link to="/tasks">Tasks</Link>
          <Link to="/tasks/new">New task</Link>
        </nav>
        <div className="app-header__user" title={sub}>
          <span>
            {name} <span className="app-header__sub">({sub})</span>
          </span>
          <span className="app-header__sep" aria-hidden />
          <Button
            type="button"
            variant="ghost"
            onClick={async () => {
              try {
                const redirect =
                  env.VITE_OIDC_POST_LOGOUT_REDIRECT_URI || `${window.location.origin}/login`;
                await auth.signoutRedirect({ post_logout_redirect_uri: redirect });
              } catch {
                void nav("/login", { replace: true });
              }
            }}
          >
            Log out
          </Button>
        </div>
      </header>
      <main className="app-main">
        <Outlet />
      </main>
    </div>
  );
}
