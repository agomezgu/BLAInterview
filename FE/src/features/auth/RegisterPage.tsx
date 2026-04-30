import { env, isMockApi } from "../../config/env";
import { useState } from "react";
import { Link } from "react-router";
import { Button } from "../../components/Button";
import { ErrorAlert } from "../../components/ErrorAlert";
import { TextField } from "../../components/TextField";
import { PageLayout } from "../../components/PageLayout";
import { MockModeBanner } from "../../components/MockModeBanner";

type Form = { name: string; email: string; password: string };
const empty: Form = { name: "", email: "", password: "" };

/**
 * Two modes: hosted registration (redirect, no password in this SPA) or
 * a public registration endpoint — adjust `VITE_*` in `.env`.
 * Prefer hosted: `VITE_USE_HOSTED_REGISTRATION=true` and a real
 * `VITE_OIDC_REGISTRATION_URL` from your IDP.
 */
export function RegisterPage() {
  if (isMockApi) {
    return (
      <>
        <MockModeBanner />
        <div className="page auth-page">
          <h1>Register</h1>
          <p>In mock mode, registration to your real IDP is not called.</p>
          <p>
            <Link to="/tasks" className="link-button">
              Go to tasks
            </Link>
          </p>
        </div>
      </>
    );
  }
  if (env.VITE_USE_HOSTED_REGISTRATION && env.VITE_OIDC_REGISTRATION_URL) {
    return (
      <PageLayout
        title="Create account"
        backTo={{ label: "Back to sign in", to: "/login" }}
      >
        <p>
          Your account is created on the identity provider&rsquo;s registration page; this app
          does not handle your password.
        </p>
        <p>
          <Button
            type="button"
            onClick={() => {
              window.location.assign(env.VITE_OIDC_REGISTRATION_URL);
            }}
          >
            Open registration
          </Button>
        </p>
      </PageLayout>
    );
  }
  return <PublicRegisterForm />;
}

/**
 * `VITE_USE_HOSTED_REGISTRATION=false` and set `VITE_REGISTRATION_API_URL` to
 * a public register endpoint. Adjust the JSON body to your IDP contract.
 * Example dev IDP: `POST /connect/register` with { name, email, password } — see
 * your backend/IDP docs, not a guarantee for your environment.
 */
function PublicRegisterForm() {
  const [form, setForm] = useState<Form>(empty);
  const [err, setErr] = useState<string | null>(null);
  const [ok, setOk] = useState(false);
  const [busy, setBusy] = useState(false);
  const regUrl = env.VITE_REGISTRATION_API_URL;
  if (!regUrl) {
    return (
      <div className="page auth-page">
        <h1>Register</h1>
        <p className="field-hint" role="alert">
          Set <code>VITE_USE_HOSTED_REGISTRATION=true</code> and
          <code> VITE_OIDC_REGISTRATION_URL</code> for a hosted sign-up page, or set
          <code> VITE_REGISTRATION_API_URL</code> to a public registration API URL
          in <code>.env</code>.
        </p>
        <p>
          <Link to="/login" className="link-button">
            Back to sign in
          </Link>
        </p>
      </div>
    );
  }
  return (
    <PageLayout
      title="Create account"
      backTo={{ label: "Back to sign in", to: "/login" }}
    >
      {ok ? (
        <p>Registration request completed. You can now sign in.</p>
      ) : (
        <form
          onSubmit={async (e) => {
            e.preventDefault();
            setErr(null);
            setBusy(true);
            try {
              const r = await fetch(regUrl, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                  name: form.name,
                  email: form.email,
                  password: form.password,
                }),
              });
              if (!r.ok) {
                const t = await r.text();
                throw new Error(t || r.statusText);
              }
              setOk(true);
            } catch (e2) {
              setErr(e2 instanceof Error ? e2.message : "Registration failed");
            } finally {
              setBusy(false);
            }
          }}
        >
          <p className="field-hint">
            Only use an endpoint you trust. This payload is a **placeholder** — align it
            with your IDP&rsquo;s public registration contract.
          </p>
          <TextField
            id="r-name"
            label="Name"
            value={form.name}
            onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
            required
          />
          <TextField
            id="r-email"
            label="Email"
            type="email"
            autoComplete="email"
            value={form.email}
            onChange={(e) => setForm((f) => ({ ...f, email: e.target.value }))}
            required
          />
          <TextField
            id="r-pw"
            label="Password"
            type="password"
            autoComplete="new-password"
            value={form.password}
            onChange={(e) => setForm((f) => ({ ...f, password: e.target.value }))}
            required
            hint="Do not use production credentials in local dev."
          />
          <ErrorAlert message={err} />
          <p>
            <Button type="submit" disabled={busy}>
              {busy ? "Submitting…" : "Register"}
            </Button>
          </p>
        </form>
      )}
    </PageLayout>
  );
}
