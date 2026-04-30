import { useEffect, useId } from "react";
import { createPortal } from "react-dom";
import { Button } from "./Button";

type Props = {
  open: boolean;
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  danger?: boolean;
  onConfirm: () => void;
  onClose: () => void;
};

export function ConfirmDialog({
  open,
  title,
  message,
  confirmLabel = "OK",
  cancelLabel = "Cancel",
  danger,
  onConfirm,
  onClose,
}: Props) {
  const t = useId();
  useEffect(() => {
    if (!open) return;
    const onK = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    window.addEventListener("keydown", onK);
    return () => window.removeEventListener("keydown", onK);
  }, [open, onClose]);
  if (!open) return null;
  return createPortal(
    <div
      className="dialog-backdrop"
      role="dialog"
      aria-modal="true"
      aria-labelledby={t}
    >
      <div className="dialog">
        <h2 id={t} className="dialog__title">
          {title}
        </h2>
        <p className="dialog__body">{message}</p>
        <div className="dialog__actions">
          <Button type="button" variant="secondary" onClick={onClose}>
            {cancelLabel}
          </Button>
          <Button
            type="button"
            variant={danger ? "danger" : "primary"}
            onClick={() => {
              onConfirm();
              onClose();
            }}
          >
            {confirmLabel}
          </Button>
        </div>
      </div>
    </div>,
    document.body,
  );
}
