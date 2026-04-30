import type { InputHTMLAttributes, ReactNode } from "react";

type Props = InputHTMLAttributes<HTMLInputElement> & {
  id: string;
  label: string;
  error?: string;
  hint?: ReactNode;
};

export function TextField({ id, label, error, hint, className, ...input }: Props) {
  return (
    <div className={`form-field ${className ?? ""}`.trim()}>
      <label htmlFor={id}>{label}</label>
      <input id={id} aria-invalid={error ? "true" : "false"} {...input} />
      {error ? <p className="field-error">{error}</p> : null}
      {hint && !error ? <p className="field-hint">{hint}</p> : null}
    </div>
  );
}
