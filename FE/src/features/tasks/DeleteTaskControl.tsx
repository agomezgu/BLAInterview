import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { useAccessTokenValue } from "../../app/context/AccessTokenContext";
import { useAppUi } from "../../app/ui/AppUiContext";
import { Button } from "../../components/Button";
import { ConfirmDialog } from "../../components/ConfirmDialog";
import { useNavigate } from "react-router";
import { taskApi } from "../../services/taskApi";

type Props = {
  taskId: string;
  label?: string;
  afterDeleteTo?: string;
  /** If false, stay on the current page (e.g. list). */
  navigateAfter?: boolean;
};

/**
 * Reusable delete with confirm + cache invalidation + global flash.
 */
export function DeleteTaskControl({
  taskId,
  label = "Delete",
  afterDeleteTo = "/tasks",
  navigateAfter = true,
}: Props) {
  const [open, setOpen] = useState(false);
  const token = useAccessTokenValue();
  const qc = useQueryClient();
  const nav = useNavigate();
  const { setFlash } = useAppUi();
  const m = useMutation({
    mutationFn: () => taskApi.deleteTask(token, taskId),
    onSuccess: async () => {
      setFlash({ message: "Task deleted", variant: "info" });
      await qc.invalidateQueries({ queryKey: ["tasks"] });
      await qc.removeQueries({ queryKey: ["task", taskId] });
      if (navigateAfter) {
        void nav(afterDeleteTo, { replace: true });
      }
    },
    onError: (e: Error) => {
      setFlash({ message: e.message, variant: "error" });
    },
  });
  return (
    <>
      <Button type="button" variant="danger" onClick={() => setOpen(true)}>
        {label}
      </Button>
      <ConfirmDialog
        open={open}
        onClose={() => setOpen(false)}
        title="Delete task?"
        message="This action cannot be undone."
        confirmLabel="Delete"
        danger
        onConfirm={() => {
          m.mutate();
        }}
      />
    </>
  );
}
