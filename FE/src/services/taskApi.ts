import { env, isMockApi } from "../config/env";
import type { CreateTaskPayload, Task, UpdateTaskPayload } from "../types/task";
import { parseApiErrorMessage } from "../types/apiError";
import { apiFetch, getUrl } from "./apiClient";
import { apiPaths } from "./apiPaths";
import { mockTaskApi } from "./mockTaskStore";
import { parseTaskFromApi } from "./taskMappers";

type CreateResponse = { taskId: number; code: string };

/** Real API: `POST /tasks` accepts `{ title: string }` only. */
function buildCreateBody(payload: CreateTaskPayload) {
  return { title: payload.title };
}

export const taskApi = {
  async getTasks(accessToken: string | null): Promise<Task[]> {
    if (isMockApi) {
      return mockTaskApi.getTasks(accessToken);
    }
    const res = await apiFetch(apiPaths.tasks());
    if (!res.ok) {
      throw new Error(await parseApiErrorMessage(res, "Failed to list tasks"));
    }
    const data = (await res.json()) as unknown[];
    return data.map((row) => parseTaskFromApi(row));
  },

  async getTask(accessToken: string | null, id: string): Promise<Task> {
    if (isMockApi) {
      return mockTaskApi.getTask(accessToken, id);
    }
    const res = await apiFetch(apiPaths.task(id));
    if (res.status === 404) {
      throw new Error("NOT_FOUND");
    }
    if (!res.ok) {
      throw new Error(await parseApiErrorMessage(res, "Failed to load task"));
    }
    return parseTaskFromApi(await res.json());
  },

  async createTask(
    accessToken: string | null,
    payload: CreateTaskPayload,
  ): Promise<{ taskId: string }> {
    if (isMockApi) {
      return mockTaskApi.createTask(accessToken, payload);
    }
    const res = await apiFetch(apiPaths.tasks(), {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(buildCreateBody(payload)),
    });
    if (res.status === 201) {
      const body = (await res.json()) as CreateResponse;
      return { taskId: String(body.taskId) };
    }
    if (!res.ok) {
      throw new Error(await parseApiErrorMessage(res, "Create failed"));
    }
    const loc = res.headers.get("Location");
    if (loc) {
      const m = /\/(\d+)\s*$/.exec(loc);
      if (m) return { taskId: m[1] };
    }
    throw new Error("Create succeeded but task id missing from response");
  },

  async updateTask(
    accessToken: string | null,
    id: string,
    body: UpdateTaskPayload,
  ): Promise<Task> {
    if (isMockApi) {
      return mockTaskApi.updateTask(accessToken, id, body);
    }
    const res = await apiFetch(apiPaths.task(id), {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        title: body.title,
        description: body.description,
        priority: body.priority,
        status: body.status,
      }),
    });
    if (res.status === 404) {
      throw new Error("NOT_FOUND");
    }
    if (!res.ok) {
      throw new Error(await parseApiErrorMessage(res, "Update failed"));
    }
    return parseTaskFromApi(await res.json());
  },

  async deleteTask(accessToken: string | null, id: string): Promise<void> {
    if (isMockApi) {
      return mockTaskApi.deleteTask(accessToken, id);
    }
    const res = await apiFetch(apiPaths.task(id), { method: "DELETE" });
    if (res.status === 404) {
      throw new Error("NOT_FOUND");
    }
    if (!res.ok) {
      throw new Error(await parseApiErrorMessage(res, "Delete failed"));
    }
  },
};

/**
 * Exposed for registration flow if you add a public endpoint.
 * Example: `fetch(env.VITE_REGISTRATION_API_URL, { method: 'POST', body: JSON.stringify(...) })`
 */
export { env, getUrl };
