import type { ReactNode } from "react";
import { Link } from "react-router";

const styles = { marginBottom: "1.5rem" } as const;

type Props = {
  title: string;
  backTo?: { label: string; to: string };
  actions?: ReactNode;
  children: ReactNode;
};

export function PageLayout({ title, backTo, actions, children }: Props) {
  return (
    <div className="page">
      <div className="page__head" style={styles}>
        {backTo ? (
          <Link to={backTo.to} className="back-link">
            ← {backTo.label}
          </Link>
        ) : null}
        <div className="page__row">
          <h1>{title}</h1>
          {actions}
        </div>
      </div>
      {children}
    </div>
  );
}
