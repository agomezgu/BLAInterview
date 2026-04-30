import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router";
import { useAccessTokenValue } from "../../app/context/AccessTokenContext";
import { Button } from "../../components/Button";
import { ErrorAlert } from "../../components/ErrorAlert";
import { Loading } from "../../components/Loading";
import { PageLayout } from "../../components/PageLayout";
import { taskApi } from "../../services/taskApi";
import { useTaskQueryEnabled } from "./useTaskQueryEnabled";
import { formatDate } from "./formatDate";
import { DeleteTaskControl } from "./DeleteTaskControl";

export function TaskListPage() {
  const token = useAccessTokenValue();
  const qEnabled = useTaskQueryEnabled();
  const { data, isLoading, isError, error, refetch } = useQuery({
    queryKey: ["tasks", token, qEnabled],
    queryFn: () => taskApi.getTasks(token),
    enabled: qEnabled,
  });
  if (isLoading) {
    return <Loading label="Loading tasks…" />;
  }
  if (isError) {
    return (
      <PageLayout title="Tasks" actions={undefined} backTo={undefined}>
        <ErrorAlert
          message={error instanceof Error ? error.message : "Failed to load tasks"}
        />
        <p>
          <Button type="button" onClick={() => void refetch()}>
            Retry
          </Button>
        </p>
      </PageLayout>
    );
  }
  const list = data ?? [];
  if (list.length === 0) {
    return (
      <PageLayout
        title="Tasks"
        actions={
          <Link to="/tasks/new" className="btn btn--secondary">
            New task
          </Link>
        }
        backTo={undefined}
      >
        <p className="field-hint">No tasks yet. Create one to get started.</p>
        <p>
          <Link to="/tasks/new" className="link-button">
            Create a task
          </Link>
        </p>
      </PageLayout>
    );
  }
  return (
    <PageLayout
      title="Tasks"
      actions={
        <Link to="/tasks/new" className="btn btn--secondary">
          New task
        </Link>
      }
      backTo={undefined}
    >
      <div className="table-wrap">
        <table className="data-table data-table--desktop">
          <thead>
            <tr>
              <th>Title</th>
              <th>Owner</th>
              <th>Priority</th>
              <th>Status</th>
              <th>Created</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {list.map((t) => (
              <tr key={t.id}>
                <td>{t.title}</td>
                <td>
                  <code className="mono">{t.ownerId}</code>
                </td>
                <td>{t.priority ?? "—"}</td>
                <td>{t.status ?? "—"}</td>
                <td className="nowrap">{formatDate(t.created)}</td>
                <td>
                  <div className="row-actions">
                    <Link to={`/tasks/${t.id}`} className="link-button">
                      View
                    </Link>
                    <span className="row-actions__sep" />
                    <Link to={`/tasks/${t.id}/edit`} className="link-button">
                      Edit
                    </Link>
                    <span className="row-actions__sep" />
                    <div className="row-actions__del">
                      <DeleteTaskControl
                        taskId={t.id}
                        label="Delete"
                        navigateAfter={false}
                      />
                    </div>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        <ul className="card-list" aria-label="Tasks (compact)">
          {list.map((t) => (
            <li key={t.id} className="card">
              <h3>
                <Link to={`/tasks/${t.id}`}>{t.title}</Link>
              </h3>
              <p>
                {t.status ?? "—"} · {t.priority ?? "—"} · {formatDate(t.created)}
              </p>
              <p className="card__actions">
                <Link to={`/tasks/${t.id}/edit`} className="link-button">
                  Edit
                </Link>
                <DeleteTaskControl taskId={t.id} label="Delete" navigateAfter={false} />
              </p>
            </li>
          ))}
        </ul>
      </div>
    </PageLayout>
  );
}
