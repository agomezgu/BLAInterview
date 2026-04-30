import { isMockApi } from "../../config/env";
import { useAccessTokenValue } from "../../app/context/AccessTokenContext";

/**
 * In mock mode we can query immediately. With OIDC, wait until a token is available
 * to avoid a burst of 401s (handled, but noisier than necessary).
 */
export function useTaskQueryEnabled() {
  const t = useAccessTokenValue();
  if (isMockApi) {
    return true;
  }
  return !!t;
}
