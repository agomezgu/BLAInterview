/** Central place for API path segments (add `/api/v1` here if the backend moves). */
const TASKS = "/tasks";

export const apiPaths = {
  tasks: () => `${TASKS}`,
  task: (id: string | number) => `${TASKS}/${id}`,
} as const;
