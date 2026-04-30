/** Map backend / FluentResult error DTOs when the shape is known. */
export type ErrorDto = { message: string; code?: string; metadata?: Record<string, string> };
export type ApiErrorBody = { errors?: ErrorDto[]; message?: string } | string | null;

export async function parseApiErrorMessage(res: Response, fallback: string) {
  try {
    const text = await res.text();
    if (!text) return `${fallback} (${res.status})`;
    const j = JSON.parse(text) as ApiErrorBody;
    if (j && typeof j === "object" && "errors" in j && Array.isArray(j.errors)) {
      return j.errors?.map((e) => e.message).join(" ") || fallback;
    }
    if (j && typeof j === "object" && "message" in j && typeof (j as { message?: string }).message === "string") {
      return (j as { message: string }).message;
    }
    return text;
  } catch {
    return `${fallback} (${res.status})`;
  }
}
