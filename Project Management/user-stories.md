# Task Management System User Stories

## Overview

This document defines the user stories for the Task Management System based on `Project Management/requirements.md`.

The stories are organized in the requested implementation order:

1. BE API
2. UI Create User
3. Tasks UI

Each story includes acceptance criteria and requirement traceability to the related functional requirements.

## 1. BE API

### BE-API-001: Validate IDP-Issued Tokens

As the Task Management API, I want to validate tokens issued by the IDP server so that only authenticated users can access protected task endpoints.

Acceptance Criteria:

- Given a request without an authentication token, when the request targets a protected task endpoint, then the API returns an unauthorized response.
- Given a request with an invalid, expired, or incorrectly issued token, when the request targets a protected task endpoint, then the API returns an unauthorized response.
- Given a request with a valid IDP-issued token, when the token includes the required user identity claim, then the API allows the request to continue.
- Given a valid token, when the API processes a task request, then the API uses the IDP-issued user identifier as the task owner identity.

Traceability: FR-013, FR-014, FR-015, FR-016, NFR-019, NFR-020, NFR-022.

### BE-API-002: Provide Public Health Endpoint

As a reviewer, I want a public API health endpoint so that I can confirm the Task Management API is running without needing to authenticate.

Acceptance Criteria:

- Given the Task Management API is running, when a caller requests the public health endpoint, then the API returns a successful response.
- Given the caller is unauthenticated, when the caller requests the public health endpoint, then the API does not require a token.
- Given the API is reachable, when the health endpoint responds, then the response indicates the service is available.

Traceability: FR-012, FR-035, NFR-042.

### BE-API-003: Create Task

As an authenticated user, I want to create a task so that I can track work I need to complete.

Acceptance Criteria:

- Given an authenticated user, when the user submits a valid task with a title, then the API creates the task.
- Given a created task, when the API stores it, then the task is associated with the authenticated user's IDP-issued user identifier.
- Given a task creation request without a title, when the API validates the request, then the API returns a validation failure.
- Given a successful task creation, when the API responds, then the response includes the created task details without exposing storage implementation details.

Traceability: FR-017, FR-018, FR-019, FR-020, FR-021, FR-022, FR-023, FR-032, FR-033, FR-034.

### BE-API-004: List Owned Tasks

As an authenticated user, I want to list my own tasks so that I can review the work assigned to my account.

Acceptance Criteria:

- Given an authenticated user with tasks, when the user requests the task list, then the API returns only tasks owned by that user's IDP-issued identifier.
- Given an authenticated user without tasks, when the user requests the task list, then the API returns an empty list.
- Given tasks owned by other users, when the authenticated user requests the task list, then those tasks are not included.

Traceability: FR-016, FR-024, FR-031, FR-037, FR-039, NFR-034.

### BE-API-005: Retrieve Owned Task Details

As an authenticated user, I want to view the details of one of my tasks so that I can inspect its full information.

Acceptance Criteria:

- Given an authenticated user and an owned task, when the user requests that task by identifier, then the API returns the task details.
- Given an authenticated user and a task owned by another user, when the user requests that task by identifier, then the API returns a not found response.
- Given an authenticated user and a missing task identifier, when the user requests that task, then the API returns a not found response.

Traceability: FR-016, FR-025, FR-029, FR-031, FR-032.

### BE-API-006: Update Owned Task

As an authenticated user, I want to update my task details so that I can keep my task information accurate.

Acceptance Criteria:

- Given an authenticated user and an owned task, when the user submits valid updates, then the API updates the editable task fields.
- Given an update request with an invalid status value, when the API validates the request, then the API returns a validation failure.
- Given an authenticated user and a task owned by another user, when the user attempts to update it, then the API returns a not found response.
- Given a successful update, when the API responds, then the response reflects the updated task state.

Traceability: FR-016, FR-026, FR-027, FR-029, FR-032, FR-033, NFR-030.

### BE-API-007: Delete Owned Task

As an authenticated user, I want to delete one of my tasks so that I can remove work that is no longer relevant.

Acceptance Criteria:

- Given an authenticated user and an owned task, when the user deletes the task, then the API removes it from the user's task list.
- Given an authenticated user and a task owned by another user, when the user attempts to delete it, then the API returns a not found response.
- Given a missing task identifier, when the user attempts to delete it, then the API returns a not found response.
- Given a successful delete operation, when the user requests the deleted task again, then the API returns a not found response.

Traceability: FR-016, FR-028, FR-029, FR-032.

### BE-API-008: Return Consistent API Responses

As an API consumer, I want consistent API responses so that frontend behavior and automated tests can rely on predictable outcomes.

Acceptance Criteria:

- Given a successful request, when the API responds, then it returns the appropriate success status code.
- Given invalid request data, when the API responds, then it returns a validation failure with useful error details.
- Given an unauthenticated request to a protected endpoint, when the API responds, then it returns an unauthorized response.
- Given a missing or inaccessible task, when the API responds, then it returns a not found response.
- Given any API response, when the payload is returned, then it does not expose password hashes or persistence implementation details.

Traceability: FR-032, FR-033, FR-034, FR-035, NFR-043.

## 3. UI Create User

### UI-USER-001: Register New Account

As a new user, I want to create an account from the Angular app so that I can start using the Task Management System.

Acceptance Criteria:

- Given the user opens the registration screen, when the screen loads, then the user can enter name, email, and password.
- Given valid registration data, when the user submits the form, then the Angular app sends the request to the IDP server.
- Given successful registration, when the IDP responds, then the UI confirms the account was created and guides the user to log in or continue.
- Given registration fails, when the IDP returns validation errors, then the UI displays helpful messages.

Traceability: FR-043, FR-044, FR-049.

### UI-USER-002: Validate Registration Form

As a new user, I want immediate feedback on registration form errors so that I can correct mistakes before submitting.

Acceptance Criteria:

- Given required fields are empty, when the user attempts to submit, then the UI shows required field messages.
- Given an invalid email format, when the user enters the email, then the UI indicates the email must be valid.
- Given a validation error from the IDP, when the response is received, then the UI displays the server-side validation message.

Traceability: FR-044, FR-049, NFR-026.

### UI-USER-003: Log In User

As a registered user, I want to log in from the Angular app so that I can access protected task screens.

Acceptance Criteria:

- Given the user opens the login screen, when the screen loads, then the user can enter email and password.
- Given valid credentials, when the user submits the login form, then the Angular app sends the request to the IDP server.
- Given a successful login response, when the IDP returns a token, then the Angular app stores the authentication state needed for protected API requests.
- Given a successful login, when authentication state is stored, then the user is routed to the task list.
- Given invalid credentials, when the IDP rejects the login, then the UI displays a user-friendly error message.

Traceability: FR-043, FR-050, FR-051, NFR-025.

### UI-USER-004: Protect Authenticated Routes

As an application user, I want protected screens to require login so that private task data is not exposed.

Acceptance Criteria:

- Given an unauthenticated user, when the user attempts to access the task list route, then the Angular app redirects the user to login.
- Given an authenticated user, when the user accesses a protected task route, then the Angular app allows navigation.
- Given the authentication token is missing or cleared, when the user attempts protected navigation, then access is denied.

Traceability: FR-051, NFR-019, NFR-022.

### UI-USER-005: Display Authenticated User Context

As an authenticated user, I want to see basic profile context so that I know which account is active.

Acceptance Criteria:

- Given a logged-in user, when the application loads protected screens, then the UI can request the authenticated user profile from the IDP.
- Given the profile request succeeds, when the UI renders account context, then it displays safe profile information such as name or email.
- Given the profile request fails because authentication is invalid, when the UI handles the response, then it clears authentication state and requires login again.

Traceability: FR-010, FR-050, FR-051.

## 4. Tasks UI

### UI-TASK-001: View Task List

As an authenticated user, I want to view my task list so that I can understand what work I need to manage.

Acceptance Criteria:

- Given an authenticated user, when the user opens the task list screen, then the Angular app requests tasks from the Task Management API.
- Given the API returns tasks, when the screen renders, then the UI displays the user's tasks.
- Given the API returns an empty list, when the screen renders, then the UI displays an empty state.
- Given the API returns an unauthorized response, when the screen handles the error, then the UI requires the user to log in again.

Traceability: FR-045, FR-050, FR-051, NFR-028.

### UI-TASK-002: Create Task

As an authenticated user, I want to create a task from the Angular app so that I can add work to my task list.

Acceptance Criteria:

- Given the user opens the create task form, when the form loads, then the user can enter title, description, status, and optional due date.
- Given valid task data, when the user submits the form, then the Angular app sends the create request to the Task Management API with the IDP-issued token.
- Given successful creation, when the API responds, then the UI adds the task to the visible task list or returns the user to the task list.
- Given invalid task data, when the API returns validation errors, then the UI displays helpful messages.

Traceability: FR-046, FR-049, FR-050, NFR-026, NFR-028.

### UI-TASK-003: Edit Task Details

As an authenticated user, I want to edit a task from the Angular app so that I can keep task information accurate.

Acceptance Criteria:

- Given the user selects an existing task, when the edit form opens, then the form displays the current task details.
- Given valid changes, when the user submits the edit form, then the Angular app sends the update request to the Task Management API with the IDP-issued token.
- Given successful update, when the API responds, then the UI displays the updated task information.
- Given the task no longer exists or is not accessible, when the API returns not found, then the UI displays an appropriate message.

Traceability: FR-047, FR-049, FR-050.

### UI-TASK-004: Change Task Status

As an authenticated user, I want to change a task's status so that I can track progress from pending to completed.

Acceptance Criteria:

- Given a visible task, when the user changes the task status, then the Angular app sends the status update to the Task Management API.
- Given a valid status transition, when the API responds successfully, then the UI reflects the new task status.
- Given an invalid status value is rejected, when the API returns a validation response, then the UI displays a helpful error message.

Traceability: FR-047, FR-049, FR-050, NFR-030.

### UI-TASK-005: Delete Task

As an authenticated user, I want to delete a task from the Angular app so that I can remove tasks that are no longer needed.

Acceptance Criteria:

- Given a visible task, when the user chooses to delete it, then the UI asks for confirmation before deleting.
- Given the user confirms deletion, when the Angular app calls the Task Management API, then the request includes the IDP-issued token.
- Given successful deletion, when the API responds, then the UI removes the task from the visible list.
- Given the task no longer exists or is not accessible, when the API returns not found, then the UI displays an appropriate message.

Traceability: FR-048, FR-049, FR-050, NFR-027.

### UI-TASK-006: Show Loading and Error States

As an authenticated user, I want clear loading and error states so that I understand what is happening while task actions are processed.

Acceptance Criteria:

- Given the task list is loading, when the API request is in progress, then the UI displays a loading state.
- Given a create, edit, status change, or delete action is in progress, when the request is being submitted, then the UI prevents duplicate submissions where appropriate.
- Given an API error occurs, when the UI handles the error, then the UI displays a user-friendly message.
- Given authentication is no longer valid, when a protected API call returns unauthorized, then the UI routes the user back to login or requires re-authentication.

Traceability: FR-049, FR-050, FR-051, NFR-028.