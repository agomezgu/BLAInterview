import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router";
import { z } from "zod";
import { useAccessTokenValue } from "../../app/context/AccessTokenContext";
import { useAppUi } from "../../app/ui/AppUiContext";
import { Button } from "../../components/Button";
import { ErrorAlert } from "../../components/ErrorAlert";
import { PageLayout } from "../../components/PageLayout";
import { TextField } from "../../components/TextField";
import { isMockApi } from "../../config/env";
import { taskApi } from "../../services/taskApi";
import { useAuth } from "react-oidc-context";

const schema = z.object({
  title: z.string().min(1, "Title is required"),
});

type Form = z.infer<typeof schema>;

const defaults: Form = {
  title: "",
};

function OidcSessionOwner() {
  const a = useAuth();
  const sub = typeof a.user?.profile?.sub === "string" ? a.user.profile.sub : "—";
  return (
    <p className="field-hint">
      <strong>Owner (from your session / sub):</strong> <code>{sub}</code> — the API
      does not take <code>ownerId</code> in the body; it is derived from the token
      on the server.
    </p>
  );
}

export function TaskCreatePage() {
  const token = useAccessTokenValue();
  const nav = useNavigate();
  const qc = useQueryClient();
  const { setFlash } = useAppUi();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Form>({ resolver: zodResolver(schema), defaultValues: defaults });
  const m = useMutation({
    mutationFn: (v: Form) => taskApi.createTask(token, { title: v.title }),
    onSuccess: async (res) => {
      setFlash({ message: "Task created", variant: "info" });
      await qc.invalidateQueries({ queryKey: ["tasks"] });
      void nav(`/tasks/${res.taskId}`, { replace: true });
    },
    onError: (e: Error) => setFlash({ message: e.message, variant: "error" }),
  });
  return (
    <PageLayout
      title="New task"
      backTo={{ label: "Back to list", to: "/tasks" }}
    >
      {isMockApi ? (
        <p className="field-hint">
          <strong>Owner (mock):</strong> <code>mock-user</code>
        </p>
      ) : (
        <OidcSessionOwner />
      )}
      <form
        onSubmit={handleSubmit((v) => m.mutate(v))}
        className="stacked-form"
        noValidate
      >
        <TextField
          id="title"
          label="Title *"
          error={errors.title?.message}
          {...register("title")}
        />
        {m.isError && (
          <ErrorAlert
            message={m.error instanceof Error ? m.error.message : "Error"}
          />
        )}
        <p>
          <Button type="submit" disabled={m.isPending}>
            {m.isPending ? "Saving…" : "Create task"}
          </Button>{" "}
          <Link to="/tasks" className="link-button">
            Cancel
          </Link>
        </p>
      </form>
    </PageLayout>
  );
}
