/* eslint-disable react-refresh/only-export-components -- context + hook pattern */
import {
  createContext,
  useContext,
  useReducer,
  type ReactNode,
} from "react";
import { appUiReducer, initialAppUi, type AppUiState, type FlashState } from "./appUiReducer";

type Ctx = {
  state: AppUiState;
  setFlash: (f: NonNullable<FlashState>) => void;
  clearFlash: () => void;
  setNavOpen: (open: boolean) => void;
  toggleNav: () => void;
};

const AppUiContext = createContext<Ctx | null>(null);

export function AppUiProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(appUiReducer, initialAppUi);
  const value: Ctx = {
    state,
    setFlash: (payload) => dispatch({ type: "flash/set", payload }),
    clearFlash: () => dispatch({ type: "flash/clear" }),
    setNavOpen: (open) => dispatch({ type: "nav/set", payload: open }),
    toggleNav: () => dispatch({ type: "nav/toggle" }),
  };
  return <AppUiContext.Provider value={value}>{children}</AppUiContext.Provider>;
}

export function useAppUi() {
  const c = useContext(AppUiContext);
  if (!c) {
    throw new Error("useAppUi must be used within AppUiProvider");
  }
  return c;
}
