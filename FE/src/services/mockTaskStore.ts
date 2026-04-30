import { taskListSchema, type CreateTaskPayload, type Task, type UpdateTaskPayload } from "../types/task";

let idSeq = 1;
const store: Task[] = [
  {
    id: "1",
    title: "Sample (mock) task",
    ownerId: "mock-user",
    created: new Date().toISOString(),
    createdBy: "mock-user",
    description: "Set VITE_MOCK_API=false and configure the real API to use the backend.",
    priority: "High",
    status: "Pending",
  },
];

export const mockTaskApi = {
  async getTasks(_: string | null): Promise<Task[]> {
    return taskListSchema
      .parse(structuredClone(store))
      .map((t) => ({ ...t, createdBy: t.createdBy || t.ownerId }));
  },
  async getTask(_: string | null, id: string): Promise<Task> {
    const t = store.find((x) => x.id === id);
    if (!t) {
      return Promise.reject(new Error("NOT_FOUND"));
    }
    return { ...t };
  },
  /** Matches real API: POST only accepts `{ title }`. */
  async createTask(_: string | null, payload: CreateTaskPayload): Promise<{ taskId: string }> {
    const id = String(idSeq++);
    const now = new Date().toISOString();
    const task: Task = {
      id,
      title: payload.title,
      ownerId: "mock-user",
      created: now,
      createdBy: "mock-user",
      description: null,
      priority: null,
      status: null,
    };
    store.push(task);
    return { taskId: id };
  },
  async updateTask(
    _: string | null,
    id: string,
    body: UpdateTaskPayload,
  ): Promise<Task> {
    const i = store.findIndex((x) => x.id === id);
    if (i < 0) {
      return Promise.reject(new Error("NOT_FOUND"));
    }
    const prev = store[i];
    const next: Task = {
      ...prev,
      title: body.title,
      description: body.description,
      priority: body.priority,
      status: body.status,
    };
    store[i] = next;
    return next;
  },
  async deleteTask(_: string | null, id: string): Promise<void> {
    const i = store.findIndex((x) => x.id === id);
    if (i < 0) {
      return Promise.reject(new Error("NOT_FOUND"));
    }
    store.splice(i, 1);
  },
};
