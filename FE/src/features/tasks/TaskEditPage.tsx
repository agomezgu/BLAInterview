import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate, useParams } from "react-router";
import { z } from "zod";
import { useAccessTokenValue } from "../../app/context/AccessTokenContext";
import { useAppUi } from "../../app/ui/AppUiContext";
import { Button } from "../../components/Button";
import { ErrorAlert } from "../../components/ErrorAlert";
import { Loading } from "../../components/Loading";
import { PageLayout } from "../../components/PageLayout";
import { SelectField } from "../../components/SelectField";
import { TextField } from "../../components/TextField";
import { taskApi } from "../../services/taskApi";
import { useTaskQueryEnabled } from "./useTaskQueryEnabled";
import { TaskPriorities, TaskStatuses } from "./constants";
import { DeleteTaskControl } from "./DeleteTaskControl";

const editSchema = z.object({
  title: z.string().min(1, "Title is required"),
  description: z.string().optional(),
  priority: z.string().optional(),
  status: z.string().optional(),
});

type Form = z.infer<typeof editSchema>;

export function TaskEditPage() {
  const { id = "" } = useParams();
  const token = useAccessTokenValue();
  const qEn = useTaskQueryEnabled();
  const nav = useNavigate();
  const qc = useQueryClient();
  const { setFlash } = useAppUi();
  const q = useQuery({
    queryKey: ["task", id, token, qEn],
    queryFn: () => taskApi.getTask(token, id),
    enabled: qEn && !!id,
  });
  const form = useForm<Form>({ resolver: zodResolver(editSchema) });
  useEffect(() => {
    if (q.data) {
      const t = q.data;
      form.reset({
        title: t.title,
        description: t.description ?? "",
        priority: t.priority ?? "",
        status: t.status ?? "",
      });
    }
  }, [q.data, form]);
  const m = useMutation({
    mutationFn: (f: Form) =>
      taskApi.updateTask(token, id, {
        title: f.title,
        description: f.description?.length ? f.description : null,
        priority: f.priority?.length ? f.priority : null,
        status: f.status?.length ? f.status : null,
      }),
    onSuccess: async (updated) => {
      setFlash({ message: "Task updated", variant: "info" });
      await qc.invalidateQueries({ queryKey: ["tasks"] });
      await qc.setQueryData(["task", id], updated);
      void nav(`/tasks/${id}`, { replace: true });
    },
    onError: (e: Error) => setFlash({ message: e.message, variant: "error" }),
  });
  if (q.isLoading) {
    return <Loading label="Loading task…" />;
  }
  if (q.isError || !q.data) {
    return (
      <PageLayout
        title="Edit task"
        backTo={{ label: "All tasks", to: "/tasks" }}
      >
        <ErrorAlert message="Could not load this task" />
        <p>
          <Link to="/tasks" className="link-button">
            All tasks
          </Link>
        </p>
      </PageLayout>
    );
  }
  const t = q.data;
  return (
    <PageLayout
      title="Edit task"
      backTo={{ label: "Back to task", to: `/tasks/${id}` }}
      actions={<DeleteTaskControl taskId={id} />}
    >
      <p className="field-hint">
        Owner: <code>{t.ownerId}</code> (read-only — the API does not support changing
        the owner in <code>PUT /tasks/&#x7B;id&#x7D;</code>).
      </p>
      <form
        onSubmit={form.handleSubmit((v) => m.mutate(v))}
        className="stacked-form"
        noValidate
      >
        <TextField
          id="e-title"
          label="Title *"
          error={form.formState.errors.title?.message}
          {...form.register("title")}
        />
        <TextField
          id="e-desc"
          label="Description"
          type="text"
          error={form.formState.errors.description?.message}
          {...form.register("description")}
        />
        <SelectField
          id="e-pri"
          label="Priority"
          error={form.formState.errors.priority?.message}
          options={TaskPriorities.map((p) => ({ value: p, label: p }))}
          {...form.register("priority")}
        />
        <SelectField
          id="e-st"
          label="Status"
          error={form.formState.errors.status?.message}
          options={TaskStatuses.map((s) => ({ value: s, label: s }))}
          {...form.register("status")}
        />
        {m.isError && <ErrorAlert message="Update failed" />}
        <p>
          <Button type="submit" disabled={m.isPending}>
            {m.isPending ? "Saving…" : "Save changes"}
          </Button>{" "}
          <Link to={`/tasks/${id}`} className="link-button">
            Cancel
          </Link>
        </p>
      </form>
    </PageLayout>
  );
}
