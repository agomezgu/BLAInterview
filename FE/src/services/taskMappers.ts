import { taskSchema, type Task } from "../types/task";

export type RawTask = Record<string, unknown>;

export function parseTaskFromApi(json: unknown): Task {
  const t = taskSchema.parse(json);
  return {
    ...t,
    createdBy: t.createdBy || t.ownerId,
  };
}
