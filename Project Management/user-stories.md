# Task Management System User Stories

## Overview

This document defines the user stories for the Task Management System based on `Project Management/requirements.md`.

The stories are organized in the requested implementation order:

1. BE IDP
2. BE API
3. UI Create User
4. Tasks UI

Each story includes acceptance criteria and requirement traceability to the related functional requirements.

## 1. BE IDP

### BE-IDP-001: Validate Registration Request

As the IDP server, I want to validate registration data so that only complete and usable user information is accepted.

Acceptance Criteria:

- Given a registration request without a name, when the request is submitted, then the IDP returns a validation failure.
- Given a registration request without an email, when the request is submitted, then the IDP returns a validation failure.
- Given a registration request with an invalid email format, when the request is submitted, then the IDP returns a validation failure.
- Given a registration request without a password, when the request is submitted, then the IDP returns a validation failure.

Traceability: FR-001, FR-002, FR-032, FR-033.

### BE-IDP-002: Reject Duplicate Email

As the IDP server, I want to reject duplicate email addresses so that each user account has a unique login identifier.

Acceptance Criteria:

- Given an existing user email, when a registration request uses the same email, then the IDP rejects the request.
- Given a duplicate email registration attempt, when the IDP responds, then it returns a validation failure.
- Given a rejected duplicate registration, when the data store is checked, then no duplicate user record is created.

Traceability: FR-003, FR-032, FR-033, FR-036.

### BE-IDP-003: Create User Account

As a new user, I want the IDP to create my account after valid registration so that I can log in later.

Acceptance Criteria:

- Given a valid registration request with a unique email, when the request is submitted, then the IDP creates a user account.
- Given a successful registration, when the user is stored, then the IDP persists the user's identifier, name, email, password hash, and audit data.
- Given the account is created, when the IDP responds, then the response includes safe user identity fields.
- Given the account is created, when the IDP responds, then it does not return the password or password hash.

Traceability: FR-001, FR-002, FR-004, FR-030, FR-036, FR-038.

### BE-IDP-004: Hash User Password

As the IDP server, I want to store password hashes instead of plain text passwords so that user credentials are protected.

Acceptance Criteria:

- Given a successful registration, when the user record is persisted, then the password is stored as a secure hash.
- Given any API response from the IDP, when user information is returned, then the password hash is not included.
- Given the user data store is inspected, when password data exists, then it is not stored as the original plain text password.

Traceability: FR-005, FR-034, NFR-018.

### BE-IDP-005: Validate Login Request

As the IDP server, I want to validate login data so that authentication attempts include the required credentials.

Acceptance Criteria:

- Given a login request without an email, when the request is submitted, then the IDP returns a validation failure.
- Given a login request with an invalid email format, when the request is submitted, then the IDP returns a validation failure.
- Given a login request without a password, when the request is submitted, then the IDP returns a validation failure.
- Given validation fails, when the IDP responds, then no authentication token is issued.

Traceability: FR-006, FR-007, FR-032.

### BE-IDP-006: Authenticate Valid Credentials

As a registered user, I want to log in with valid credentials so that I can receive access to protected task features.

Acceptance Criteria:

- Given valid credentials for an existing user, when the user logs in, then the IDP authenticates the user.
- Given successful authentication, when the IDP compares the provided password, then it validates it against the stored password hash.
- Given successful authentication, when the IDP responds, then the response indicates the login was successful.

Traceability: FR-006, FR-030, NFR-018.

### BE-IDP-007: Reject Invalid Credentials

As the IDP server, I want to reject invalid credentials so that unauthorized users cannot receive valid access tokens.

Acceptance Criteria:

- Given an unknown email address, when login is attempted, then the IDP rejects the request.
- Given a known email address with an incorrect password, when login is attempted, then the IDP rejects the request.
- Given a failed login, when the IDP responds, then it does not reveal whether the email or password was incorrect.
- Given a failed login, when the IDP responds, then no authentication token is issued.

Traceability: FR-007, FR-032, NFR-021.

### BE-IDP-008: Issue Authentication Token

As the IDP server, I want to issue a signed authentication token after successful login so that the user can access protected Task Management API endpoints.

Acceptance Criteria:

- Given a successful login, when the IDP creates the authentication response, then it includes a signed token.
- Given the token is issued, when it is decoded by the Task Management API, then it includes the required user identifier claim.
- Given the token is issued, when it is validated by the Task Management API, then the issuer, audience, signature, and expiration can be verified.
- Given authentication fails, when the IDP responds, then no token is issued.

Traceability: FR-008, FR-009, NFR-019, NFR-020.

### BE-IDP-009: Return Authenticated User Profile

As an authenticated user, I want to retrieve my basic profile so that the application can display who is logged in.

Acceptance Criteria:

- Given a valid IDP-issued token, when the user requests the profile endpoint, then the IDP returns the authenticated user's basic profile.
- Given the profile response, when the payload is returned, then it includes safe identity fields such as user identifier, name, and email.
- Given the profile response, when the payload is returned, then it excludes password and password hash data.
- Given a missing or invalid token, when the user requests the profile endpoint, then the IDP returns an unauthorized response.

Traceability: FR-010, FR-030, FR-034.

### BE-IDP-010: Protect Profile Endpoint

As the IDP server, I want the profile endpoint to require a valid token so that user profile data is only returned to authenticated users.

Acceptance Criteria:

- Given a request without a token, when the user requests the profile endpoint, then the IDP returns an unauthorized response.
- Given a request with an invalid or expired token, when the user requests the profile endpoint, then the IDP returns an unauthorized response.
- Given a valid token, when the user requests the profile endpoint, then the IDP allows the request.

Traceability: FR-010, FR-032, NFR-019.

### BE-IDP-011: Seed Demo User

As a reviewer, I want seeded demo credentials so that I can evaluate the application quickly without manually creating an account first.

Acceptance Criteria:

- Given the local demo environment is initialized, when the IDP seed process runs, then a demo user is available.
- Given the README credentials, when a reviewer logs in with them, then the IDP authenticates the demo user.
- Given repeated local setup, when seeding is run again, then the demo user remains in a known usable state without creating duplicates.

Traceability: FR-011, FR-055, NFR-032.

## 2. BE API

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
