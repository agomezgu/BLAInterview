export function ErrorAlert({ message }: { message: string | null | undefined }) {
  if (!message) return null;
  return <div className="alert alert--error">{message}</div>;
}
