import { createBrowserRouter, Navigate } from "react-router";
import { AppLayout } from "../app/AppLayout";
import { RequireAuth } from "../features/auth/RequireAuth";
import { LoginPage } from "../features/auth/LoginPage";
import { RegisterPage } from "../features/auth/RegisterPage";
import { CallbackPage } from "../features/auth/CallbackPage";
import { TaskListPage } from "../features/tasks/TaskListPage";
import { TaskCreatePage } from "../features/tasks/TaskCreatePage";
import { TaskDetailPage } from "../features/tasks/TaskDetailPage";
import { TaskEditPage } from "../features/tasks/TaskEditPage";

export const appRouter = createBrowserRouter(
  [
    { path: "/", element: <Navigate to="/tasks" replace /> },
    { path: "login", element: <LoginPage /> },
    { path: "register", element: <RegisterPage /> },
    { path: "auth/callback", element: <CallbackPage /> },
    {
      path: "tasks",
      element: <RequireAuth />,
      children: [
        {
          element: <AppLayout />,
          children: [
            { index: true, element: <TaskListPage /> },
            { path: "new", element: <TaskCreatePage /> },
            { path: ":id", element: <TaskDetailPage /> },
            { path: ":id/edit", element: <TaskEditPage /> },
          ],
        },
      ],
    },
  ],
);
