export type FlashState =
  | { message: string; variant: "info" | "error" }
  | null;

export type AppUiState = {
  flash: FlashState;
  /** Mobile nav or shell drawer, optional. */
  navOpen: boolean;
};

export const initialAppUi: AppUiState = {
  flash: null,
  navOpen: false,
};

type AppUiAction =
  | { type: "flash/set"; payload: NonNullable<FlashState> }
  | { type: "flash/clear" }
  | { type: "nav/set"; payload: boolean }
  | { type: "nav/toggle" };

export function appUiReducer(state: AppUiState, action: AppUiAction): AppUiState {
  switch (action.type) {
    case "flash/set":
      return { ...state, flash: action.payload };
    case "flash/clear":
      return { ...state, flash: null };
    case "nav/set":
      return { ...state, navOpen: action.payload };
    case "nav/toggle":
      return { ...state, navOpen: !state.navOpen };
    default:
      return state;
  }
}
