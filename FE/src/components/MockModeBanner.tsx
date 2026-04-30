import { isMockApi } from "../config/env";

export function MockModeBanner() {
  if (!isMockApi) return null;
  return (
    <div className="mock-banner" role="status">
      Mock API mode: tasks are stored in memory. Set <code>VITE_MOCK_API=false</code> and
      run the real backend + IDP to use your API.
    </div>
  );
}
