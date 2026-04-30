import { useAppUi } from "../app/ui/AppUiContext";

export function FlashBar() {
  const { state, clearFlash } = useAppUi();
  if (!state.flash) return null;
  const c = state.flash;
  return (
    <div
      className={
        c.variant === "error"
          ? "flash flash--error"
          : "flash flash--info"
      }
      role="status"
    >
      <span>{c.message}</span>
      <button
        type="button"
        className="link-button"
        onClick={clearFlash}
        aria-label="Dismiss"
      >
        Dismiss
      </button>
    </div>
  );
}
