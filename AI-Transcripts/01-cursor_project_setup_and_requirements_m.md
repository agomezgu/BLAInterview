# Project setup and requirements management
_Exported on 4/27/2026 at 15:41:41 GMT-5 from Cursor (3.1.17)_

---

**User**

In this new project I´ll be creating a UI pp using Angular, WebApi using .NET following Clean Architecture and TDD , and I´ll need to create a backlog and a system to manage the requirement functional and non functional. did you get the initial context?

---

**Cursor**

Yes, I got it.

You’re starting a new project with:

- Angular UI application
- .NET Web API
- Clean Architecture
- TDD as a core development approach
- A backlog for delivery planning
- A requirements management system covering both functional and non-functional requirements

So the likely next step is to define the project structure and working system: requirements format, backlog hierarchy, architecture boundaries, Definition of Ready/Done, test strategy, and how requirements trace into user stories, tests, and implementation.

I’m in planning mode, so I won’t change files yet.

---

**User**

I want to split this inicial phase into 3 :

1 AI Enablement 
1.1 - Enable the repo to create the global AI practices and rules to follow
1.2-  Create the rules to follow for the BE API
1.3 - Create the rules to follow in the FE

2 - Scaffold BE .NET Cleant Architecture (TDD) API
3 - Scaffold FE Angular 

Did you get the next steps?

---

**Cursor**

I understand the split: first establish AI working conventions, then scaffold the .NET Clean Architecture API with TDD, then scaffold the Angular frontend. I’ll first load the rule-creation guidance so the plan matches Cursor’s expected structure.

Yes, I got the next steps.

You want the initial phase split into 3 workstreams:

1. **AI Enablement**
   - Set up repo-wide AI practices and rules.
   - Add backend API rules.
   - Add frontend Angular rules.

2. **Backend Scaffold**
   - Create a .NET Web API using Clean Architecture.
   - Follow TDD from the start.

3. **Frontend Scaffold**
   - Create the Angular UI application.

The intended order sounds like: define AI/project rules first, then scaffold the backend, then scaffold the frontend, so both apps are created under agreed conventions.

---

**User**

Let´s start with phase 1, enable the repo based in the requirement.

---

**Cursor**

I’ll scope phase 1 into concrete repo-enablement artifacts first: existing structure, any current Cursor config, and where backend/frontend rules should apply. After that I’ll present a concise plan for approval before creating files.

The workspace appears to be empty of project files and Cursor rules, so phase 1 can set the conventions without needing to preserve existing structure. The main decision is the folder layout/globs these rules should target.

**Phase 1 Rule Scope**

> **Which folder layout should the AI rules assume for the backend and frontend?**
> - Use `src/backend/**` for .NET API and `src/frontend/**` for Angular UI
> - Use `backend/**` for .NET API and `frontend/**` for Angular UI
> - Use a different layout; I will provide it
>
> Answer: Use the existing "BE" for the "BackEnd" project  and "FE" for the Angular UI project

Got it: phase 1 will assume `BE/**` for the backend and `FE/**` for the Angular app. I’ll turn that into a short implementation plan for the repo AI enablement files.

---

**User**

Let´s build this first phase, whit empty mdc files 

---

**Cursor**

Understood. I’ll switch to implementation mode and create only the phase 1 rule file placeholders under `.cursor/rules/`, leaving the `.mdc` contents empty as requested.

I’m creating just the directory and three placeholder rule files now: global, backend, and frontend. Afterward I’ll verify the files exist and haven’t picked up any unintended content.

Done. I created the empty Cursor rule placeholders:

- `.cursor/rules/global-ai-practices.mdc`
- `.cursor/rules/backend-api-clean-architecture.mdc`
- `.cursor/rules/frontend-angular.mdc`

Verified all three files exist and are empty. No linter issues reported.

---

**User**

Add another file for TDD rules in BE 

---

**Cursor**

I’ll add one more backend-scoped placeholder rule for TDD under `.cursor/rules/`, keeping it empty to match the current phase 1 pattern.

Done. Added the empty BE TDD rule file:

- `.cursor/rules/backend-tdd.mdc`

Verified it exists, is empty, and has no linter issues.

---

**User**

the idea is to follow as much the Devs can this rules based on "Dennis Doomen" explanation, add them in the TDD file

- Start designing responsibilities before you write your first test.
- Then use the tests to drive the design further.
- Organize code by functionalities or capabilities.
- Use the Dependency Inversion Principle to decouple boundaries.
- Apply DRY within those boundaries.
- Align your test scope with those boundaries.
- Sometimes testing smaller is fine too.
- Test things designed to be reusable separately, but test implementation details as part of a bigger scope.
- Treat adjacent folders as separate boundaries.
- Test the real surface area.
- It is fine to include the database.
- Clarify the Arrange, Act, Assert structure, but use a different style if that helps.
- Do not bother defining what “unit” versus “integration” is.
- Write your tests like you write your production code, and let them serve as documentation.
- It is fine to inject concrete classes inside boundaries.
- Use mocking between boundaries, but avoid using mocks internally.
- Do not return mocks from mocks.
- Hide things that are not important for that specific test.
- Ensure a test fails or succeeds for the right reason.
- Prefer inline literal strings and numbers over constants.
- Do not use production code in assertions.
- Only assert what is relevant for that specific test case.
- Stay out of debugger hell.
- Postfix with Specs to emphasize tests are specifications, and group tests by API or purpose using nested classes.
- Use short, fact-based names.

---

**Cursor**

I’ll populate `.cursor/rules/backend-tdd.mdc` with backend TDD guidance based on those Dennis Doomen principles, scoped to `BE/**` so it applies when working in the backend.

The TDD rule file is empty, so I’ll add the `.mdc` frontmatter plus a focused rule body that preserves your points as actionable backend guidance.

Done. I added the Dennis Doomen-inspired backend TDD rules to:

- `.cursor/rules/backend-tdd.mdc`

It is scoped to `BE/**/*`, includes `.mdc` frontmatter, and lints cleanly.

---

**User**

adjust the @.cursor/rules/backend-api-clean-architecture.mdc  file to be used intelligently just in BE folder 

---

**Cursor**

I’ll adjust the backend Clean Architecture rule so Cursor applies it only when working in the `BE/**` backend folder, and I’ll keep the existing guidance intact unless the frontmatter needs cleanup.

The file currently has `alwaysApply: false` but no `globs`, so it is not explicitly scoped. Since it also includes Angular guidance, I’m going to make this backend-only by adding the `BE/**/*` glob and removing frontend-specific sections from this backend rule.

I found the backend rule had frontend guidance embedded in it. I’m tightening it so this file is backend-only: `globs: BE/**/*`, backend-focused description, and no Angular/frontend sections.

Done. I updated `.cursor/rules/backend-api-clean-architecture.mdc` so it applies only to backend files:

```md
globs: BE/**/*
alwaysApply: false
```

I also removed the Angular/frontend guidance from that backend rule so it stays focused on the .NET API and Clean Architecture boundaries. No linter issues found.

