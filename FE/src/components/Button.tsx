import type { ButtonHTMLAttributes, ReactNode } from "react";

type Props = ButtonHTMLAttributes<HTMLButtonElement> & {
  children: ReactNode;
  variant?: "primary" | "secondary" | "ghost" | "danger";
};

export function Button({ variant = "primary", className = "", ...rest }: Props) {
  const v = `btn btn--${variant}`;
  return <button className={`${v} ${className}`.trim()} {...rest} />;
}
