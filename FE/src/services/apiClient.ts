import { env, isMockApi } from "../config/env";
import { parseApiErrorMessage } from "../types/apiError";

let getAccessToken: () => string | null = () => null;
let onUnauthorized: (() => void) | null = null;

export function configureApiClient(opts: {
  getAccessToken: () => string | null;
  onUnauthorized: () => void;
}) {
  getAccessToken = opts.getAccessToken;
  onUnauthorized = opts.onUnauthorized;
}

function baseUrl() {
  return (env.VITE_API_BASE_URL || "/api").replace(/\/$/, "");
}

export function getUrl(path: string) {
  if (!path.startsWith("/")) {
    return `${baseUrl()}/${path}`;
  }
  return `${baseUrl()}${path}`;
}

export async function apiFetch(
  path: string,
  init: RequestInit = {},
): Promise<Response> {
  if (isMockApi) {
    throw new Error("apiFetch should not be used in mock mode");
  }
  const headers = new Headers(init.headers);
  const t = getAccessToken();
  if (t) {
    headers.set("Authorization", `Bearer ${t}`);
  }
  headers.set("Accept", "application/json");
  const res = await fetch(getUrl(path), { ...init, headers });
  if (res.status === 401) {
    onUnauthorized?.();
  }
  return res;
}

export async function apiJson<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await apiFetch(path, init);
  if (!res.ok) {
    const msg = await parseApiErrorMessage(res, "Request failed");
    throw new Error(msg);
  }
  if (res.status === 204) {
    return undefined as T;
  }
  return (await res.json()) as T;
}
