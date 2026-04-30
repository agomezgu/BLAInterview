import type { ReactNode, SelectHTMLAttributes } from "react";

type Option = { value: string; label: string };

type Props = SelectHTMLAttributes<HTMLSelectElement> & {
  id: string;
  label: string;
  options: readonly Option[];
  emptyLabel?: string;
  error?: string;
  hint?: ReactNode;
};

export function SelectField({
  id,
  label,
  options,
  emptyLabel = "—",
  error,
  hint,
  className,
  ...sel
}: Props) {
  return (
    <div className={`form-field ${className ?? ""}`.trim()}>
      <label htmlFor={id}>{label}</label>
      <select id={id} aria-invalid={error ? "true" : "false"} {...sel}>
        <option value="">{emptyLabel}</option>
        {options.map((o) => (
          <option key={o.value} value={o.value}>
            {o.label}
          </option>
        ))}
      </select>
      {error ? <p className="field-error">{error}</p> : null}
      {hint && !error ? <p className="field-hint">{hint}</p> : null}
    </div>
  );
}
