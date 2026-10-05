# HR Assistant Login Page — Requirements

## 1. Purpose
Add a login page to the existing HR Assistant application while preserving its current Angular UI theme, visual language, layout conventions, typography, colors, spacing, cards, buttons, form controls, and responsive behavior.

The login flow must support two demo identities:
- **Admin** — access to existing admin-appropriate features.
- **Employee** — access to existing employee-appropriate features.

For this POC, authentication verification will use an in-memory backend service. This is demo-only and must not be represented as production-grade authentication.

## 2. Existing Application Context
- Frontend: Angular.
- Backend: .NET API.
- AI integration: Microsoft Agent Framework.
- Existing HR question endpoint: `POST http://localhost:5118/api/hr/ask`.
- Existing UI and backend functionality must continue to work after the login feature is added.

Before coding, inspect the repository and identify the existing Angular version, standalone/component conventions, routing, services, theme tokens/styles, backend structure, API patterns, and tests. Reuse the current architecture and components wherever practical.

## 3. Goals
1. Create a login page that looks native to the existing HR Assistant UI.
2. Verify credentials through a .NET backend service using in-memory demo users.
3. Support Admin and Employee roles.
4. Navigate authenticated users to the existing HR Assistant landing/dashboard experience.
5. Preserve existing chat, HR API, AI-agent, and dashboard functionality.
6. Add validation, loading/error states, responsive layout, accessibility, and automated tests.

## 4. UI and Theme Requirements
- Inspect the existing application before designing the page.
- Reuse existing theme variables, CSS classes, typography, color palette, spacing, buttons, cards, icons, and form controls.
- Do not introduce a new design system or unrelated visual redesign.
- Match existing page widths, border radii, shadows, backgrounds, and header conventions.
- Keep the page responsive across desktop, tablet, and mobile.
- Use semantic HTML and visible keyboard focus states.
- Do not add dependencies unless necessary and approved by the existing project conventions.

### Login form
- Application branding/title consistent with the current HR Assistant UI.
- Username or email field, based on the existing project conventions.
- Password field with show/hide control if consistent with existing controls.
- Sign In button.
- Inline validation and authentication error message.
- Loading/disabled state while the request is in progress.
- Optional demo-account hints only in development/demo configuration; never display secrets in production configuration.

## 5. Authentication and Roles
### Demo users
Provide exactly two configurable in-memory demo accounts: one Admin and one Employee. Keep usernames, passwords, and roles in one clearly named demo-user configuration/service rather than scattering them through controllers or UI code.

- Admin account: configured demo username/password, role `Admin`.
- Employee account: configured demo username/password, role `Employee`.

Use safe example placeholders in source control and document how to configure local demo credentials. Do not use real credentials or personal information. Avoid logging passwords or returning them in API responses.

### Backend verification
- Add a dedicated login request/response contract and authentication service in the existing .NET API.
- The service verifies the submitted username/password against the in-memory demo-user collection.
- On success, return a success result and the user's safe profile/role.
- On failure, return a generic authentication error; do not disclose whether the username or password was incorrect.
- Validate empty/invalid input server-side as well as client-side.
- Use appropriate HTTP status codes and the project's existing error-response conventions.
- Do not claim that in-memory verification is secure production authentication. Do not store plaintext credentials in persistent storage. For this local POC, keep demo credentials configurable and clearly mark the implementation as development-only.
- Do not return a fabricated JWT or imply token-based security unless the repository already has a real token-validation implementation.

## 6. Session and Route Behavior
- On successful login, store only the minimum demo session state required by the existing app (for example, current user display name and role).
- Do not store the password.
- Follow the existing Angular routing and state-management approach.
- Add route protection only to the extent supported by the current demo architecture. Clearly document that client-side route guards are not a security boundary.
- Add a logout action if the application has a suitable shared header/navigation area. Logout clears demo session state and returns to login.
- Unauthenticated users visiting a protected HR Assistant route should be redirected to login, then returned to their intended route after successful login if feasible.
- Admin and Employee navigation/feature differences must be based only on existing, verified role requirements. Do not invent new HR features. If no existing role-specific feature rules exist, both roles can access the existing core HR Assistant experience, while the UI displays their role appropriately.

## 7. Existing Functionality Preservation
Verify that login integration does not break:
- Existing Angular application startup and routing.
- HR Assistant chat UI and message rendering.
- `POST /api/hr/ask` integration and response handling.
- Microsoft Agent Framework/backend service wiring.
- Existing agent tools, workflow dashboard, logs, and status views that are present in the repository.
- Existing responsive behavior and styles.
- Existing build and test scripts.

Do not rewrite unrelated files or replace working API/model integration. Keep changes focused and backward-compatible.

## 8. Error and Loading States
Handle:
- Required fields missing.
- Invalid credentials.
- Backend unavailable/network error.
- Unexpected server error.
- Repeated submit while a login request is in progress.
- Session missing/expired according to the demo's chosen session approach.

Show a concise user-friendly message. Never show stack traces, secrets, or internal exception details in the UI.

## 9. Security and POC Limitations
This is a local/demo authentication mechanism only:
- In-memory users are reset when the backend restarts.
- No production identity provider, persistent user store, password hashing policy, MFA, refresh tokens, or server-enforced authorization is implied.
- Do not use this design for a deployed production system without replacing it with a proper identity solution and server-side authorization.
- Keep CORS and configuration consistent with the existing application; do not broadly weaken them to make login work.

## 10. Acceptance Criteria
- [ ] Login page follows the existing HR Assistant theme and styles.
- [ ] Exactly one Admin and one Employee demo account can be configured and verified by the backend.
- [ ] Valid credentials return a safe user profile and role.
- [ ] Invalid credentials show a generic error and do not expose account details.
- [ ] Empty fields are validated in the UI and backend.
- [ ] Loading state prevents duplicate submissions.
- [ ] Successful login opens the existing HR Assistant experience.
- [ ] Role is available to the UI without exposing credentials.
- [ ] Logout clears demo session state if implemented.
- [ ] Protected route behavior works as documented.
- [ ] Existing chat, HR API, and agent/dashboard functionality still work.
- [ ] Responsive layout and keyboard accessibility are checked.
- [ ] Angular build, .NET build, and relevant tests pass.
- [ ] Documentation clearly labels in-memory authentication as demo-only.
