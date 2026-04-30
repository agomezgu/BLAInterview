import { useQuery } from "@tanstack/react-query";
import { Link, useParams } from "react-router";
import { useAccessTokenValue } from "../../app/context/AccessTokenContext";
import { Button } from "../../components/Button";
import { ErrorAlert } from "../../components/ErrorAlert";
import { Loading } from "../../components/Loading";
import { PageLayout } from "../../components/PageLayout";
import { taskApi } from "../../services/taskApi";
import { useTaskQueryEnabled } from "./useTaskQueryEnabled";
import { formatDate } from "./formatDate";
import { DeleteTaskControl } from "./DeleteTaskControl";

export function TaskDetailPage() {
  const { id = "" } = useParams();
  const token = useAccessTokenValue();
  const qEnabled = useTaskQueryEnabled();
  const { data, isLoading, isError, error, refetch } = useQuery({
    queryKey: ["task", id, token, qEnabled],
    queryFn: () => taskApi.getTask(token, id),
    enabled: qEnabled && !!id,
  });
  if (!id) {
    return <ErrorAlert message="Missing task id" />;
  }
  if (isLoading) {
    return <Loading label="Loading task…" />;
  }
  if (isError || !data) {
    const msg =
      error instanceof Error
        ? error.message === "NOT_FOUND"
          ? "Task not found"
          : error.message
        : "Failed to load";
    return (
      <PageLayout
        title="Task"
        backTo={{ label: "Back to list", to: "/tasks" }}
        actions={undefined}
      >
        <ErrorAlert message={msg} />
        <p>
          <Button type="button" onClick={() => void refetch()}>
            Retry
          </Button>{" "}
          <Link to="/tasks" className="link-button">
            All tasks
          </Link>
        </p>
      </PageLayout>
    );
  }
  const t = data;
  const createdBy = t.createdBy || t.ownerId;
  return (
    <PageLayout
      title={t.title}
      backTo={{ label: "All tasks", to: "/tasks" }}
      actions={
        <>
          <Link to={`/tasks/${t.id}/edit`} className="btn btn--secondary">
            Edit
          </Link>{" "}
          <DeleteTaskControl taskId={t.id} />
        </>
      }
    >
      <dl className="field-grid">
        <div>
          <dt>Id</dt>
          <dd>
            <code>{t.id}</code>
          </dd>
        </div>
        <div>
          <dt>Title</dt>
          <dd>{t.title}</dd>
        </div>
        <div>
          <dt>Owner (user id)</dt>
          <dd>
            <code>{t.ownerId}</code>
          </dd>
        </div>
        <div>
          <dt>Description</dt>
          <dd>{t.description || "—"}</dd>
        </div>
        <div>
          <dt>Priority</dt>
          <dd>{t.priority ?? "—"}</dd>
        </div>
        <div>
          <dt>Status</dt>
          <dd>{t.status ?? "—"}</dd>
        </div>
        <div>
          <dt>Created (read-only)</dt>
          <dd className="readonly">{formatDate(t.created)}</dd>
        </div>
        <div>
          <dt>Created by (read-only)</dt>
          <dd className="readonly">
            <code>{createdBy}</code>
          </dd>
        </div>
      </dl>
    </PageLayout>
  );
}
