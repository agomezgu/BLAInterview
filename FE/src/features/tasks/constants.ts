/**
 * Temporary frontend enums — replace with OpenAPI-generated types or a shared
 * contract package when the backend enum list is available as an API.
 */
export const TaskPriorities = ["Low", "Normal", "High", "Urgent"] as const;
export type TaskPriority = (typeof TaskPriorities)[number];

/** Align with BLAInterview.Domain: Pending → InProgress | Cancelled, etc. */
export const TaskStatuses = [
  "Pending",
  "InProgress",
  "Completed",
  "Cancelled",
] as const;
export type TaskStatus = (typeof TaskStatuses)[number];
