import { z } from "zod";

const idCoerce = z.union([z.string(), z.number()]).transform(String);

export const taskSchema = z.object({
  id: idCoerce,
  title: z.string(),
  ownerId: z.string(),
  created: z.union([z.string(), z.number()]).transform((v) => String(v)),
  /** Not always returned by the API; UI can fall back to ownerId. */
  createdBy: z.string().optional().nullable().default(""),
  description: z.string().nullish().optional().catch(null),
  priority: z.string().nullish().optional().catch(null),
  status: z.string().nullish().optional().catch(null),
});

export const taskListSchema = z.array(taskSchema);

export type Task = {
  id: string;
  title: string;
  ownerId: string;
  created: string;
  createdBy: string;
  description?: string | null;
  priority?: string | null;
  status?: string | null;
};

export type CreateTaskPayload = {
  title: string;
  // Optional in forms until backend extends POST /tasks:
  description?: string | null;
  priority?: string | null;
  status?: string | null;
};

export type UpdateTaskPayload = {
  title: string;
  description: string | null;
  priority: string | null;
  status: string | null;
};
